using CoreUtilities.HelperClasses.Database;
using CoreUtilities.Interfaces;
using System;
using System.Collections.Generic;

namespace CoreUtilities.Services
{
	/// <summary>
	/// Implementation of <see cref="IDatabaseActionHelper{TData}"/>. Class provides clean build pattern type ways of
	/// interacting with a <see cref="IDatabaseWrapperService{TData}"/>.
	/// </summary>
	/// <typeparam name="TData">The data type the <see cref="IDatabaseWrapperService{TData}"/> is storing.</typeparam>
	public class DatabaseActionHelper<TData> : IDatabaseActionHelper<TData>
	{
		private IDatabaseWrapperService<TData> database;

		/// <summary>
		/// Constructor for <see cref="DatabaseActionHelper{TData}"/>.
		/// </summary>
		/// <param name="databaseWrapper">The <see cref="IDatabaseWrapperService{TData}"/> this class should use for
		/// building commands to interact with.</param>
		public DatabaseActionHelper(IDatabaseWrapperService<TData> databaseWrapper)
		{
			database = databaseWrapper;
		}

		/// <inheritdoc/>
		public WriteTransactionWrapper<TData> GetWriteTransaction()
		{
			database.OpenWriteTransaction();
			return new WriteTransactionWrapper<TData>(database);
		}

		/// <inheritdoc/>
		public UpdateTransactionWrapper<TData> GetUpdateTransaction()
		{
			database.OpenWriteTransaction();
			return new UpdateTransactionWrapper<TData>(database);
		}

		/// <inheritdoc/>
		public ReaderInstanceWrapper<TData> GetReader()
		{
			return new ReaderInstanceWrapper<TData>(database);
		}

		/// <inheritdoc/>
		public ReaderInstanceWrapper<TData, TReturn> GetReader<TReturn>()
		{
			return new ReaderInstanceWrapper<TData, TReturn>(database);
		}

		/// <inheritdoc/>
		public IEnumerable<TData> GetConvertedInstancesBetweenIndices(int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool> selectionCriteria = null)
		{
			var result = database.GetConvertedRowsBetweenIndices(startIndex, endIndex, defaultCreator, selectionCriteria);
			return result;
		}

		/// <inheritdoc/>
		public IEnumerable<TData> GetConvertedInstances(Func<TData, bool>? selectionCriteria = null)
		{
			return database.GetConvertedRows(selectionCriteria);
		}

		/// <inheritdoc/>
		public int RowCount(Func<TData, bool>? selector = null)
		{
			return database.RowCount(selector);
		}

		/// <inheritdoc/>
		public void ClearDatabase()
		{
			database.ClearAllRows();
		}

		/// <inheritdoc/>
		public void Disconnect()
		{
			database.Disconnect();
		}
	}
}
