using CoreUtilities.HelperClasses.Database;
using System;
using System.Collections.Generic;

namespace CoreUtilities.Interfaces
{
	public interface IDatabaseActionBuilder<TData>
	{
		WriteTransactionWrapper<TData> GetWriteTransaction();

		UpdateTransactionWrapper<TData> GetUpdateTransaction();

		ReaderInstanceWrapper<TData> GetReader();

		ReaderInstanceWrapper<TData, TReturn> GetReader<TReturn>();

		IEnumerable<TData> GetConvertedInstancesBetweenIndices(int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool> selector = null);

		IEnumerable<TData> GetConvertedInstances(Func<TData, bool> selector = null);

		int RowCount(Func<TData, bool> selector = null);

		void ClearDatabase();

		void Disconnect();
	}
}
