using System.Collections.Generic;

namespace CoreUtilities.Services
{
	public interface IDatabaseWrapperService<TReturn>
	{
		const string IsFilteredOutColumnName = "IsFilteredOut";

		string DatabaseName { get; set; }

		(int reference, IEnumerable<object> rows) AllRows();

		void CloseRowReader(int reference);

		(int reference, IEnumerable<object> rows) FilteredRows();

		void ClearAllRows();

		int RowCount();

		int FilteredRowCount();

		void AddRange(IEnumerable<TReturn> list);

		void Add(TReturn row);

		IEnumerable<TReturn> GetConvertedRowsBetweenIndices(int startIndex, int endIndex, bool isFiltered);

		IEnumerable<TReturn> GetConvertedRows(bool isFiltered);

		void OpenWriteTransaction();

		void UpdateRow(TReturn row);

		void UpdateRowFilterStatus(TReturn row);

		void CloseWriteTransaction();

		void Disconnect();
	}
}