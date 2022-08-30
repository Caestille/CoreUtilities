using CoreUtilities.HelperClasses;
using CoreUtilities.Interfaces;
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
	public class DatabaseWrapperService<TData, TTransaction> : IDatabaseWrapperService<TData> where TTransaction : DbTransaction
	{
		private readonly IDatabaseService<TTransaction> database;

		private const string tableName = "MainTable";
		private const string dateTimeColumnName = "DateTime";
		private const string primaryKeyColumnName = "Id";

		private int rowCount;

		private Func<TData, List<KeyValuePair<string, string>>> valueConverter;
		private Func<TData, DateTime> dateGetter;
		private Func<TData, string> primaryKeyGetter;
		private Func<IDataRecord, TData> dbItemConverter;

		private Dictionary<string, int> primaryKeyMappings = new Dictionary<string, int>();

		private int count = 0;
		private ConcurrentDictionary<int, SQLiteDataReader> rowReaders = new();

		private const string updateRowCommandName = "updateRow";
		private const string insertRowCommandName = "insertRow";

		private TTransaction writeTransaction;

		private bool breakOperation;

		public string DatabaseName { get; set; }

		public DatabaseWrapperService(string path, bool recreate, IDatabaseService<TTransaction> databaseService, KeyValuePair<string, ColumnType>[] columns, string[] columnsToIndex, Func<TData, List<KeyValuePair<string, string>>> valueConverter, Func<TData, DateTime> dateGetter, Func<TData, string> primaryKeyGetter, Func<IDataRecord, TData> dbItemConverter)
		{
			database = databaseService;

			DatabaseName = new FileInfo(path).Name;

			this.valueConverter = valueConverter;
			this.dateGetter = dateGetter;
			this.primaryKeyGetter = primaryKeyGetter;
			this.dbItemConverter = dbItemConverter;

			var columnsToAdd = columns.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.GetEnumDescription()))
				.Union(new List<KeyValuePair<string, string>>()
					{
						new KeyValuePair<string, string>(dateTimeColumnName, "TEXT"),
						new KeyValuePair<string, string>(primaryKeyColumnName, "INTEGER"),
					})
				.ToArray();

			database.AddTableAndColumns(tableName, columnsToAdd, columnsToIndex);
			database.SetUpUpdateCommand(tableName, updateRowCommandName, columnsToAdd.Select(x => x.Key).ToList(), primaryKeyColumnName);
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
			SQLiteDataReader reader = (database.GetRows(tableName, "", GenerateOrderingString(dateTimeColumnName, Ordering.Descending)) as SQLiteDataReader)!;
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

		public void ClearAllRows()
		{
			database.Clear(tableName);
		}

		public int RowCount(Func<TData, bool> selector = null)
		{
			if (selector == null)
			{
				return (int)database.RowCount(tableName, "");
			}
			else
			{
				int count = 0;

				SQLiteDataReader reader =
				(database.GetRows(
					tableName,
					"",
					GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
				as SQLiteDataReader)!;

				while (reader.Read())
				{
					if (selector(dbItemConverter(reader)))
					{
						count++;
					}
				}
				return count;
			}
		}

		public void AddRange(IEnumerable<TData> list)
		{
			TTransaction transaction = database.GetAndOpenWriteTransaction();
			foreach (TData row in list)
			{
				if (breakOperation)
					break;

				var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
					{
						new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).Ticks.ToString()),
						new KeyValuePair<string, string>(primaryKeyColumnName, rowCount.ToString()),
					}).ToList();

				primaryKeyMappings[primaryKeyGetter(row)] = rowCount;
				rowCount++;

				database.ExecuteInsertCommand(insertRowCommandName, itemValues,	transaction);
			}
			database.CommitAndCloseTransaction(transaction);
			transaction.Dispose();
		}

		public void Add(TData row)
		{
			var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).Ticks.ToString()),
					new KeyValuePair<string, string>(primaryKeyColumnName, rowCount.ToString()),
				}).ToList();

			database.ExecuteInsertCommand(insertRowCommandName, itemValues, writeTransaction);

			primaryKeyMappings[primaryKeyGetter(row)] = rowCount;
			rowCount++;
		}

		public IEnumerable<TData> GetConvertedRowsBetweenIndices(
			int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool> selector = null)
		{
			SQLiteDataReader reader = 
				(database.GetRows(
					tableName,
					"",
					GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
				as SQLiteDataReader)!;

			List<TData> list = new List<TData>();
			var doSelection = selector != null;

			int i = 0;
			while (reader.Read())
			{
				var item = dbItemConverter(reader);
				var allowed = !doSelection || selector(item);

				if (i < startIndex)
				{
					if (allowed)
					{
						i++;
					}
					continue;
				}

				if (i > endIndex)
				{
					break;
				}

				if (allowed)
				{
					list.Add(dbItemConverter(reader));
					i++;
				}
			}

			reader.Close();
			reader.Dispose();

			var count = list.Count();
			if (count < (endIndex - startIndex))
			{
				for (i = 0; i < endIndex - startIndex - count; i++)
				{
					list.Add(defaultCreator());
				}
			}

			return list;
		}

		public IEnumerable<TData> GetConvertedRows(Func<TData, bool> selector = null)
		{
			SQLiteDataReader reader =
				(database.GetReaderWithRowsBetweenIndices(
					tableName,
					0,
					rowCount,
					"",
					GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
				as SQLiteDataReader)!;

			List<TData> list = new List<TData>();
			var doSelection = selector != null;

			while (reader.Read())
			{
				var item = dbItemConverter(reader);
				if (!doSelection || selector(item))
				{
					list.Add(item);
				}
			}

			reader.Close();
			reader.Dispose();

			return list;
		}

		public void OpenWriteTransaction()
		{
			writeTransaction = database.GetAndOpenWriteTransaction();
		}

		public void UpdateRow(TData row)
		{
			if (breakOperation)
				return;

			var itemValues = valueConverter(row).Union(new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(dateTimeColumnName, dateGetter(row).Ticks.ToString()),
				}).ToList();

			database.ExecuteUpdateCommand(
				updateRowCommandName, 
				itemValues,
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