using System.Collections.Generic;

namespace CoreUtilities.Services
{
	public interface IDatabaseWrapperService<TReturn>
	{
		const string IsFilteredOutColumnName = "IsFilteredOut";

		void Init(string path, bool recreate);

		string DatabaseName { get; set; }

		IEnumerable<object> AllRows();
		void CloseRowReader();

		IEnumerable<object> FilteredRows();

		void CloseFilteredRowReader();

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