using CoreUtilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data;

namespace CoreUtilities.Services
{
	public class DatabaseActionBuilder<TReturn> : IDatabaseActionBuilder<TReturn>
	{
		private IDatabaseWrapperService<TReturn> database;

		public DatabaseActionBuilder(IDatabaseWrapperService<TReturn> databaseWrapper)
		{
			database = databaseWrapper;
		}

		public void Init(string path, bool recreate)
		{
			database.Init(path, recreate);
		}

		public WriteTransactionWrapper<TReturn> GetWriteTransaction()
		{
			database.OpenWriteTransaction();
			return new WriteTransactionWrapper<TReturn>(database);
		}

		public UpdateTransactionWrapper<TReturn> GetUpdateTransaction()
		{
			database.OpenWriteTransaction();
			return new UpdateTransactionWrapper<TReturn>(database);
		}

		public ReaderInstanceWrapper<TReturn> GetReader(ReaderType readerType)
		{
			return new ReaderInstanceWrapper<TReturn>(database, readerType == ReaderType.Filtered);
		}

		public ReaderInstanceWrapper<TReturn, T> GetReader<T>(ReaderType readerType)
		{
			return new ReaderInstanceWrapper<TReturn, T>(database, readerType == ReaderType.Filtered);
		}

		public IEnumerable<TReturn> GetConvertedInstancesBetweenRows(ReaderType readerType, int startIndex, int endIndex)
		{
			var result = database.GetConvertedRowsBetweenIndices(startIndex, endIndex, readerType == ReaderType.Filtered);
			return result;
		}

		public IEnumerable<TReturn> GetConvertedInstances(ReaderType readerType)
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
	}
}
