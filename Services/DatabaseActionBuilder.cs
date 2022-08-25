using CoreUtilities.HelperClasses.Database;
using CoreUtilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace CoreUtilities.Services
{
	public class DatabaseActionBuilder<TData> : IDatabaseActionBuilder<TData>
	{
		private IDatabaseWrapperService<TData> database;

		public DatabaseActionBuilder(IDatabaseWrapperService<TData> databaseWrapper)
		{
			database = databaseWrapper;
		}

		public WriteTransactionWrapper<TData> GetWriteTransaction()
		{
			database.OpenWriteTransaction();
			return new WriteTransactionWrapper<TData>(database);
		}

		public UpdateTransactionWrapper<TData> GetUpdateTransaction()
		{
			database.OpenWriteTransaction();
			return new UpdateTransactionWrapper<TData>(database);
		}

		public ReaderInstanceWrapper<TData> GetReader()
		{
			return new ReaderInstanceWrapper<TData>(database);
		}

		public ReaderInstanceWrapper<TData, TReturn> GetReader<TReturn>()
		{
			return new ReaderInstanceWrapper<TData, TReturn>(database);
		}

		public IEnumerable<TData> GetConvertedInstancesBetweenIndices(int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool> selectionCriteria = null)
		{
			var result = database.GetConvertedRowsBetweenIndices(startIndex, endIndex, defaultCreator, selectionCriteria);
			return result;
		}

		public IEnumerable<TData> GetConvertedInstances(Func<TData, bool> selectionCriteria = null)
		{
			return database.GetConvertedRows(selectionCriteria);
		}

		public int RowCount(Func<TData, bool> selector = null)
		{
			return database.RowCount(selector);
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
