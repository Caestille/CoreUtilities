using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace CoreUtilities.Services
{
	public interface IDatabaseService<T> where T : DbTransaction
	{
		void AddTableAndColumns(string tableName, KeyValuePair<string, string>[] columnNamesAndDataTypes, string[] columnsToIndex);

		void SetUpUpdateCommand(string tableName, string commandName, List<string> parametersToAdd, string conditionalMatchParameter);

		void SetUpInsertCommand(string tableName, string commandName, List<string> parametersToAdd);

		T GetAndOpenWriteTransaction();

		long RowCount(string tableName, string condition);

		void ExecuteUpdateCommand(string commandName, List<KeyValuePair<string, string>> paramsToUpdate, KeyValuePair<string, string> conditionalParamToUpdate, T transaction = null);

		void ExecuteInsertCommand(string commandName, List<KeyValuePair<string, string>> paramsToInsert, T transaction = null);

		void Clear(string tableName);

		void AddRowToTable(string tableName, List<KeyValuePair<string, string>> namedValues, T transaction = null);

		void CommitAndCloseTransaction(T transaction);

		IEnumerable GetReaderWithRowsBetweenIndices(string tableName, int startIndex, int endIndex, string condition, string ordering);

		IEnumerable GetRows(string tableName, string rowCondition, string ordering);

		void IndexColumn(string tableName, string indexName, string columnName);

		void Disconnect();
	}
}
