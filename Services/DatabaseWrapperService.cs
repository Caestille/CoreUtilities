using CoreUtilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace CoreUtilities.Services
{
	public enum ColumnType
	{
		[Description("TEXT")]
		Text,

		[Description("INTEGER")]
		Int,
	}

	public class DatabaseWrapperService<T>
	{
		private readonly SqLiteDatabaseService database;

		private const string tableName = "MainTable";
		public const string IsFilteredOutColumnName = "IsFilteredOut";
		private const string dateTimeColumnName = "DateTime";
		private const string primaryKeyColumnName = "Id";

		private int rowCount;

		private Func<T, List<KeyValuePair<string, string>>> valueConverter;
		private Func<T, DateTime> dateGetter;
		private Func<T, bool> isFilteredOutGetter;
		private Func<T, string> primaryKeyGetter;
		private Func<IDataRecord, T> dbItemConverter;

		private Dictionary<string, int> primaryKeyMappings = new Dictionary<string, int>();

		private SQLiteDataReader lastRowReader;
		private SQLiteDataReader lastFilteredRowReader;

		private const string updateRowCommandName = "updateRow";
		private const string insertRowCommandName = "insertRow";
		private const string updateRowFilterStatusCommandName = "updateRowFilterStatus";

		private SQLiteTransaction writeTransaction;

		private bool breakOperation;

		private enum Ordering
		{
			Ascending,
			Descending
		}

		public string DatabaseName { get; private set; }

		public DatabaseWrapperService(string fullPath, bool recreate, KeyValuePair<string, ColumnType>[] columns, string[] columnsToIndex, Func<T, List<KeyValuePair<string, string>>> valueConverter, Func<T, DateTime> dateGetter, Func<T, bool> isFilteredOutGetter, Func<T, string> primaryKeyGetter, Func<IDataRecord, T> dbItemConverter)
		{
			DatabaseName = new FileInfo(fullPath).Name;
			database = new SqLiteDatabaseService(fullPath, recreate);

			var columnsToAdd = columns.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.GetEnumDescription()))
				.Union(new List<KeyValuePair<string, string>>()
					{
						new KeyValuePair<string, string>(dateTimeColumnName, "TEXT"),
						new KeyValuePair<string, string>(IsFilteredOutColumnName, "INTEGER"),
						new KeyValuePair<string, string>(primaryKeyColumnName, "INTEGER"),
					})
				.ToArray();

			database.AddTableAndColumns(tableName, columnsToAdd, columnsToIndex);
			database.SetUpUpdateCommand(tableName, updateRowCommandName, columnsToAdd.Select(x => x.Key).ToList(), primaryKeyColumnName);
			database.SetUpUpdateCommand(tableName, updateRowFilterStatusCommandName, new List<string>() { IsFilteredOutColumnName }, primaryKeyColumnName);
			database.SetUpInsertCommand(tableName, insertRowCommandName, columnsToAdd.Select(x => x.Key).ToList());

			this.valueConverter = valueConverter;
			this.dateGetter = dateGetter;
			this.isFilteredOutGetter = isFilteredOutGetter;
			this.primaryKeyGetter = primaryKeyGetter;
			this.dbItemConverter = dbItemConverter;

			if (recreate)
				return;

			foreach (object item in AllRows())
			{
				var reader = item as IDataRecord;
				primaryKeyMappings[primaryKeyGetter(dbItemConverter(reader))] = Convert.ToInt32(reader[primaryKeyColumnName]);
			}

			rowCount = primaryKeyMappings.Count();
		}

		public IEnumerable<object> AllRows()
		{
			SQLiteDataReader reader = (database.GetRows(tableName, "", GenerateOrderingString(dateTimeColumnName, Ordering.Ascending)) as SQLiteDataReader)!;
			lastRowReader = reader;
			return reader.Cast<object>();
		}

		public void CloseRowReader()
		{
			lastRowReader.Close();
			lastRowReader.Dispose();
			lastRowReader = null;
		}

		public IEnumerable<object> FilteredRows()
		{
			SQLiteDataReader reader = (database.GetRows(tableName, $"WHERE NOT({IsFilteredOutColumnName})", GenerateOrderingString(dateTimeColumnName, Ordering.Ascending)) as SQLiteDataReader)!;
			lastFilteredRowReader = reader;
			return reader.Cast<object>();
		}

		public void CloseFilteredRowReader()
		{
			lastFilteredRowReader.Close();
			lastFilteredRowReader.Dispose();
			lastFilteredRowReader = null;
		}

		public void ClearAllRows()
		{
			database.Clear(tableName);
		}

		public int RowCount()
		{
			return (int)database.RowCount(tableName, "");
		}

		public int FilteredRowCount()
		{
			return (int)database.RowCount(tableName, $"WHERE NOT({IsFilteredOutColumnName})");
		}

		public void AddRange(IEnumerable<T> list)
		{
			SQLiteTransaction transaction = database.GetAndOpenWriteTransaction();
			foreach (T row in list)
			{
				if (breakOperation)
					break;

				var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
					{
						new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).ToString()),
						new KeyValuePair<string, string>(IsFilteredOutColumnName, Convert.ToInt16(isFilteredOutGetter(row)).ToString()),
						new KeyValuePair<string, string>(primaryKeyColumnName, rowCount.ToString()),
					}).ToList();

				primaryKeyMappings[primaryKeyGetter(row)] = rowCount;
				rowCount++;

				database.ExecuteInsertCommand(insertRowCommandName, itemValues,	transaction);
			}
			database.CommitAndCloseTransaction(transaction);
			transaction.Dispose();
		}

		public void Add(T row)
		{
			var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).ToString()),
					new KeyValuePair<string, string>(IsFilteredOutColumnName, Convert.ToInt16(isFilteredOutGetter(row)).ToString()),
					new KeyValuePair<string, string>(primaryKeyColumnName, rowCount.ToString()),
				}).ToList();

			database.ExecuteInsertCommand(insertRowCommandName, itemValues, writeTransaction);

			primaryKeyMappings[primaryKeyGetter(row)] = rowCount;
			rowCount++;
		}

		public IEnumerable<T> GetConvertedRowsBetweenIndices(int startIndex, int endIndex, bool isFiltered)
		{
			SQLiteDataReader reader = 
				(database.GetReaderWithRowsBetweenIndices(
					tableName,
					startIndex,
					endIndex,
					isFiltered ? $"WHERE NOT({IsFilteredOutColumnName})" : "",
					GenerateOrderingString(dateTimeColumnName, Ordering.Ascending))
				as SQLiteDataReader)!;

			List<T> list = new List<T>();

			while (reader.Read())
			{
				list.Add(dbItemConverter(reader));
			}

			reader.Close();
			reader.Dispose();

			return list;
		}

		public IEnumerable<T> GetConvertedRows(bool isFiltered)
		{
			SQLiteDataReader reader =
				(database.GetReaderWithRowsBetweenIndices(
					tableName,
					0,
					rowCount,
					isFiltered ? $"WHERE NOT({IsFilteredOutColumnName})" : "",
					GenerateOrderingString(dateTimeColumnName, Ordering.Ascending))
				as SQLiteDataReader)!;

			List<T> list = new List<T>();

			while (reader.Read())
			{
				list.Add(dbItemConverter(reader));
			}

			reader.Close();
			reader.Dispose();

			return list;
		}

		public void OpenWriteTransaction()
		{
			writeTransaction = database.GetAndOpenWriteTransaction();
		}

		public void UpdateRow(T row)
		{
			if (breakOperation)
				return;

			var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).ToString()),
					new KeyValuePair<string, string>(IsFilteredOutColumnName, Convert.ToInt16(isFilteredOutGetter(row)).ToString()),
				}).ToList();

			database.ExecuteUpdateCommand(updateRowCommandName, itemValues, new KeyValuePair<string, string>(primaryKeyColumnName, primaryKeyGetter(row)), writeTransaction);
		}

		public void UpdateRowFilterStatus(T row)
		{
			if (breakOperation)
				return;

			database.ExecuteUpdateCommand(
				updateRowFilterStatusCommandName, 
				new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(IsFilteredOutColumnName, Convert.ToInt16(isFilteredOutGetter(row)).ToString())
				},
				new KeyValuePair<string, string>(primaryKeyColumnName, primaryKeyMappings[primaryKeyGetter(row)].ToString()),
				writeTransaction);
		}

		public void CloseWriteTransaction()
		{
			database.CommitAndCloseTransaction(writeTransaction);
			writeTransaction.Dispose();
			writeTransaction = null;
		}

		public void Disconnect()
		{
			breakOperation = true;
			if (lastRowReader != null) CloseRowReader();
			if (lastFilteredRowReader != null) CloseFilteredRowReader();
			// Closing connections with transactions open should simply roll them back
			database.Disconnect();
		}

		private string GenerateOrderingString(string columnName, Ordering order)
		{
			string orderingString = order == Ordering.Ascending ? "ASC" : "DESC";
			return $"ORDER BY {columnName} {orderingString}";
		}
	}
}