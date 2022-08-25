using System;
using System.Collections.Generic;

namespace CoreUtilities.Interfaces
{
	public interface IDatabaseWrapperService<TData>
	{
		string DatabaseName { get; set; }

		(int reference, IEnumerable<object> rows) AllRows();

		void CloseRowReader(int reference);

		void ClearAllRows();

		int RowCount(Func<TData, bool> selector = null);

		void AddRange(IEnumerable<TData> list);

		void Add(TData row);

		IEnumerable<TData> GetConvertedRowsBetweenIndices(int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool> selector = null);

		IEnumerable<TData> GetConvertedRows(Func<TData, bool> selector = null);

		void OpenWriteTransaction();

		void UpdateRow(TData row);

		void CloseWriteTransaction();

		void Disconnect();
	}
}