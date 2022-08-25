using CoreUtilities.HelperClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CoreUtilities.Services
{
	public class DatabaseWrapperService<TReturn, TTransaction> : IDatabaseWrapperService<TReturn> where TTransaction : DbTransaction
	{
		private readonly IDatabaseService<TTransaction> database;

		private const string tableName = "MainTable";
		public const string IsFilteredOutColumnName = IDatabaseWrapperService<TReturn>.IsFilteredOutColumnName;
		private const string dateTimeColumnName = "DateTime";
		private const string primaryKeyColumnName = "Id";

		private int rowCount;

		private Func<TReturn, List<KeyValuePair<string, string>>> valueConverter;
		private Func<TReturn, DateTime> dateGetter;
		private Func<TReturn, bool> isFilteredOutGetter;
		private Func<TReturn, string> primaryKeyGetter;
		private Func<IDataRecord, TReturn> dbItemConverter;

		private Dictionary<string, int> primaryKeyMappings = new Dictionary<string, int>();

		private int count = 0;
		private ConcurrentDictionary<int, SQLiteDataReader> rowReaders = new();

		private const string updateRowCommandName = "updateRow";
		private const string insertRowCommandName = "insertRow";
		private const string updateRowFilterStatusCommandName = "updateRowFilterStatus";

		private TTransaction writeTransaction;

		private bool breakOperation;

		public string DatabaseName { get; set; }

		public DatabaseWrapperService(string path, bool recreate, IDatabaseService<TTransaction> databaseService, KeyValuePair<string, ColumnType>[] columns, string[] columnsToIndex, Func<TReturn, List<KeyValuePair<string, string>>> valueConverter, Func<TReturn, DateTime> dateGetter, Func<TReturn, bool> isFilteredOutGetter, Func<TReturn, string> primaryKeyGetter, Func<IDataRecord, TReturn> dbItemConverter)
		{
			database = databaseService;

			DatabaseName = new FileInfo(path).Name;

			this.valueConverter = valueConverter;
			this.dateGetter = dateGetter;
			this.isFilteredOutGetter = isFilteredOutGetter;
			this.primaryKeyGetter = primaryKeyGetter;
			this.dbItemConverter = dbItemConverter;

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

			if (recreate)
				return;

			foreach (object item in AllRows().rows)
			{
				var reader = item as IDataRecord;
				primaryKeyMappings[primaryKeyGetter(dbItemConverter(reader))] = Convert.ToInt32(reader[primaryKeyColumnName]);
			}

			rowCount = primaryKeyMappings.Count();
		}

		public (int reference, IEnumerable<object> rows) AllRows()
		{
			SQLiteDataReader reader = (database.GetRows(tableName, "", GenerateOrderingString(dateTimeColumnName, Ordering.Ascending)) as SQLiteDataReader)!;
			var success = rowReaders.TryAdd(count, reader);
			if (!success)
			{
				Debug.WriteLine("Failed to add row reader");
			}
			var result = (count, reader.Cast<object>());
			count++;
			return result;
		}

		public void CloseRowReader(int reference)
		{
			var reader = rowReaders[reference];
			reader.Close();
			reader.Dispose();
			rowReaders.Remove(reference, out _);
		}

		public (int reference, IEnumerable<object> rows) FilteredRows()
		{
			SQLiteDataReader reader = (database.GetRows(tableName, $"WHERE NOT({IsFilteredOutColumnName})", GenerateOrderingString(dateTimeColumnName, Ordering.Ascending)) as SQLiteDataReader)!;
			var success = rowReaders.TryAdd(count, reader);
			if (!success)
			{
				Debug.WriteLine("Failed to add row reader");
			}
			var result = (count, reader.Cast<object>());
			count++;
			return result;
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

		public void AddRange(IEnumerable<TReturn> list)
		{
			TTransaction transaction = database.GetAndOpenWriteTransaction();
			foreach (TReturn row in list)
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

		public void Add(TReturn row)
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

		public IEnumerable<TReturn> GetConvertedRowsBetweenIndices(int startIndex, int endIndex, bool isFiltered, Func<TReturn> defaultCreator, Func<TReturn, bool> selectionCriteria = null)
		{
			if (selectionCriteria == null)
			{
				selectionCriteria = (item) => true;
			}

			SQLiteDataReader reader = 
				(database.GetRows(
					tableName,
					isFiltered ? $"WHERE NOT({IsFilteredOutColumnName})" : "",
					GenerateOrderingString(dateTimeColumnName, Ordering.Ascending))
				as SQLiteDataReader)!;

			List<TReturn> list = new List<TReturn>();

			int i = 0;
			while (reader.Read())
			{
				if (i < startIndex)
				{
					i++;
					continue;
				}

				if (i > endIndex)
				{
					break;
				}

				var item = dbItemConverter(reader);
				var allowed = selectionCriteria(item);
				if (allowed)
				{
					list.Add(dbItemConverter(reader));
					i++;
				}
			}

			if (list.Count() < (endIndex - startIndex))
			{
				for (i = 0; i < endIndex - startIndex; i++)
				{
					list.Add(defaultCreator());
				}
			}

			reader.Close();
			reader.Dispose();

			return list;
		}

		public IEnumerable<TReturn> GetConvertedRows(bool isFiltered)
		{
			SQLiteDataReader reader =
				(database.GetReaderWithRowsBetweenIndices(
					tableName,
					0,
					rowCount,
					isFiltered ? $"WHERE NOT({IsFilteredOutColumnName})" : "",
					GenerateOrderingString(dateTimeColumnName, Ordering.Ascending))
				as SQLiteDataReader)!;

			List<TReturn> list = new List<TReturn>();

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

		public void UpdateRow(TReturn row)
		{
			if (breakOperation)
				return;

			var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).ToString()),
					new KeyValuePair<string, string>(IsFilteredOutColumnName, Convert.ToInt16(isFilteredOutGetter(row)).ToString()),
				}).ToList();

			database.ExecuteUpdateCommand(
				updateRowCommandName, 
				itemValues,
				new KeyValuePair<string, string>(primaryKeyColumnName, primaryKeyMappings[primaryKeyGetter(row)].ToString()),
				writeTransaction);
		}

		public void UpdateRowFilterStatus(TReturn row)
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
			if (rowReaders.Any())
			{
				foreach (var reader in rowReaders.Values)
				{
					reader.Close();
					reader.Dispose();
				}
				rowReaders.Clear();
			}
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