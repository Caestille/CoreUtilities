using System;
using System.Collections.Generic;
using System.Data;

namespace CoreUtilities.Services
{
	public enum ReaderType
	{
		All,
		Filtered
	}

	public class DatabaseActionBuilder<T>
	{
		private DatabaseWrapperService<T> database;

		public DatabaseActionBuilder(string fullPath, bool recreate, KeyValuePair<string, ColumnType>[] columns, string[] columnsToIndex, Func<T, List<KeyValuePair<string, string>>> valueConverter, Func<T, DateTime> dateGetter, Func<T, bool> isFilteredOutGetter, Func<T, string> primaryKeyGetter, Func<IDataRecord, T> dbItemConverter)
		{
			database = new DatabaseWrapperService<T>(fullPath, recreate, columns, columnsToIndex, valueConverter, dateGetter, isFilteredOutGetter, primaryKeyGetter, dbItemConverter);
		}

		public WriteTransactionWrapper<T> GetWriteTransaction()
		{
			database.OpenWriteTransaction();
			return new WriteTransactionWrapper<T>(database);
		}

		public UpdateTransactionWrapper<T> GetUpdateTransaction()
		{
			database.OpenWriteTransaction();
			return new UpdateTransactionWrapper<T>(database);
		}

		public ReaderInstanceWrapper<T> GetReader(ReaderType readerType)
		{
			return new ReaderInstanceWrapper<T>(database, readerType == ReaderType.Filtered);
		}

		public ReaderInstanceWrapper<T, TReturn> GetReader<TReturn>(ReaderType readerType)
		{
			return new ReaderInstanceWrapper<T, TReturn>(database, readerType == ReaderType.Filtered);
		}

		public IEnumerable<T> GetConvertedInstancesBetweenRows(ReaderType readerType, int startIndex, int endIndex)
		{
			var result = database.GetConvertedRowsBetweenIndices(startIndex, endIndex, readerType == ReaderType.Filtered);
			return result;
		}

		public IEnumerable<T> GetConvertedInstances(ReaderType readerType)
		{
			return database.GetConvertedRows(readerType == ReaderType.Filtered);
		}

		public int RowCount(ReaderType readerType)
		{
			var result = readerType == ReaderType.Filtered ? database.RowCount() : database.FilteredRowCount();
			return result;
		}

		public void ClearDatabase()
		{
			database.ClearAllRows();
		}

		public void Disconnect()
		{
			database.Disconnect();
		}

		public class WriteTransactionWrapper<T>
		{
			private DatabaseWrapperService<T> database;

			public WriteTransactionWrapper(DatabaseWrapperService<T> database)
			{
				this.database = database;
			}

			public WriteTransactionWrapper<T> WithEntry(T entry)
			{
				database.Add(entry);
				return this;
			}

			public WriteTransactionWrapper<T> WithEntryRange(IEnumerable<T> entries)
			{
				database.AddRange(entries);
				return this;
			}

			public void ExecuteWrite()
			{
				database.CloseWriteTransaction();
			}
		}

		public class UpdateTransactionWrapper<T>
		{
			private DatabaseWrapperService<T> database;

			public UpdateTransactionWrapper(DatabaseWrapperService<T> database)
			{
				this.database = database;
			}

			public UpdateTransactionWrapper<T> UpdateEntry(T entry)
			{
				database.UpdateRow(entry);
				return this;
			}

			public UpdateTransactionWrapper<T> UpdateIsEntryFiltered(T entry)
			{
				database.UpdateRowFilterStatus(entry);
				return this;
			}

			public void ExecuteWrite()
			{
				database.CloseWriteTransaction();
			}
		}

		public class ReaderInstanceWrapper<T>
		{
			private DatabaseWrapperService<T> database;
			private IEnumerable<object> rows;
			private bool isFiltered;

			private object result;

			public ReaderInstanceWrapper(DatabaseWrapperService<T> database, bool isFiltered)
			{
				this.database = database;
				rows = isFiltered ? database.FilteredRows() : database.AllRows();
			}

			public ReaderInstanceWrapper<T> WithAction(Action<IEnumerable<object>> execute)
			{
				execute(rows);
				return this;
			}

			public ReaderInstanceWrapper<T> WithAction<X>(Func<IEnumerable<object>, X> execute)
			{
				result = execute(rows);
				return this;
			}

			public void Close()
			{
				if (isFiltered)
				{
					database.CloseFilteredRowReader();
				}
				else
				{
					database.CloseRowReader();
				}
			}
		}

		public class ReaderInstanceWrapper<T, X>
		{
			private DatabaseWrapperService<T> database;
			private IEnumerable<object> rows;
			private bool isFiltered;

			private X result;

			public ReaderInstanceWrapper(DatabaseWrapperService<T> database, bool isFiltered)
			{
				this.database = database;
				rows = isFiltered ? database.FilteredRows() : database.AllRows();
			}

			public ReaderInstanceWrapper<T, X> WithAction(Func<IEnumerable<object>, X> execute)
			{
				result = execute(rows);
				return this;
			}

			public X Close()
			{
				if (isFiltered)
				{
					database.CloseFilteredRowReader();
				}
				else
				{
					database.CloseRowReader();
				}
				return result;
			}
		}
	}
}
