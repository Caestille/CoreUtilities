using CoreUtilities.HelperClasses;
using System;
using System.Collections.Generic;

namespace CoreUtilities.Services
{
	public class WriteTransactionWrapper<T>
	{
		private IDatabaseWrapperService<T> database;

		public WriteTransactionWrapper(IDatabaseWrapperService<T> database)
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
		private IDatabaseWrapperService<T> database;

		public UpdateTransactionWrapper(IDatabaseWrapperService<T> database)
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
		private IDatabaseWrapperService<T> database;
		private IEnumerable<object> rows;
		private bool isFiltered;

		private object result;

		public ReaderInstanceWrapper(IDatabaseWrapperService<T> database, bool isFiltered)
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
		private IDatabaseWrapperService<T> database;
		private IEnumerable<object> rows;
		private bool isFiltered;

		private X result;

		public ReaderInstanceWrapper(IDatabaseWrapperService<T> database, bool isFiltered)
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

	public interface IDatabaseActionBuilder<T>
	{
		WriteTransactionWrapper<T> GetWriteTransaction();

		UpdateTransactionWrapper<T> GetUpdateTransaction();

		ReaderInstanceWrapper<T> GetReader(ReaderType readerType);

		ReaderInstanceWrapper<T, TReturn> GetReader<TReturn>(ReaderType readerType);

		IEnumerable<T> GetConvertedInstancesBetweenRows(ReaderType readerType, int startIndex, int endIndex);

		IEnumerable<T> GetConvertedInstances(ReaderType readerType);

		int RowCount(ReaderType readerType);

		void ClearDatabase();

		void Disconnect();
	}
}
