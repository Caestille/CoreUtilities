using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace CoreUtilities.Interfaces.Database
{
	/// <summary>
	/// An interface for a low level database service.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDatabaseService<T> where T : DbTransaction
	{
		/// <summary>
		/// Adds a table with the given column name of given data types to a database, as well as sets which columns to
		/// index.
		/// </summary>
		/// <param name="tableName">The name of the table to add.</param>
		/// <param name="columnNamesAndDataTypes">An <see cref="KeyValuePair{TKey, TValue}[]"/> containing the column
		/// names and data types.</param>
		/// <param name="columnsToIndex">The names of the columns for the database to index for searching.</param>
		void AddTableAndColumns(
			string tableName, KeyValuePair<string, string>[] columnNamesAndDataTypes, string[] columnsToIndex);

		/// <summary>
		/// Sets up an update command able to update rows matching the conditional match parameter.
		/// </summary>
		/// <param name="tableName">The table name to search for rows to update from.</param>
		/// <param name="commandName">The name of the command (for internal referencing).</param>
		/// <param name="parametersToAdd">The parameter (column) names to update.</param>
		/// <param name="conditionalMatchParameter">The name of the parameter </param>
		void SetUpUpdateCommand(
			string tableName, string commandName, List<string> parametersToAdd, string conditionalMatchParameter);

		/// <summary>
		/// Sets up a command to insert a row into a given table.
		/// </summary>
		/// <param name="tableName">The name of the table to insert a row into.</param>
		/// <param name="commandName">The name of the command (for internal referencing).</param>
		/// <param name="parametersToAdd">A <see cref="List{T}"/> of parameter names to be added.</param>
		void SetUpInsertCommand(string tableName, string commandName, List<string> parametersToAdd);

		/// <summary>
		/// Gets a write transaction which can have updates or inserts added, then committed.
		/// </summary>
		/// <returns>A instance of a <see cref="DbTransaction"/></returns>
		T GetAndOpenWriteTransaction();

		/// <summary>
		/// Gets the row count from a given table of all rows matching a condition.
		/// </summary>
		/// <param name="tableName">The table name to search.</param>
		/// <param name="condition">The condition the rows must match to be included.</param>
		/// <returns>A <see cref="long"/> which is the count of all rows matching the condition from the
		/// table.</returns>
		long RowCount(string tableName, string condition);

		/// <summary>
		/// Executes a previously set up update command of a given name with the given parameteters.
		/// </summary>
		/// <param name="commandName">The name of the command to be executed. A command of the same name should
		/// have been set up earlier using <see cref="SetUpUpdateCommand(string, string, List{string}, string)"/>.
		/// </param>
		/// <param name="paramsToUpdate">A <see cref="List{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/>s
		/// which are the parameters to update, and their corresponding values. These should be a subset (or all of)
		/// the parameters added in <see cref="SetUpUpdateCommand(string, string, List{string}, string)"/>.</param>
		/// <param name="conditionalParamToUpdate">The value the conditional parameters should be in order for
		/// the row to be updated. This should be the same as was added in
		/// <see cref="SetUpUpdateCommand(string, string, List{string}, string)"/>.</param>
		/// <param name="transaction">The transaction this command belongs to, if any. If null, it will be
		/// executed immediately.</param>
		void ExecuteUpdateCommand(
			string commandName,
			List<KeyValuePair<string, string>> paramsToUpdate,
			KeyValuePair<string, string> conditionalParamToUpdate,
			T? transaction = null);

		/// <summary>
		/// Executes a previously set up insert command of a given name with the given parameters. 
		/// </summary>
		/// <param name="commandName">The name of the command to be executed.A command of the same name should have 
		/// been set up earlier using <see cref="SetUpInsertCommand(string, string, List{string})"/>.</param>
		/// <param name="paramsToInsert">A <see cref="List{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/>s
		/// which are the parameters to insert, and their corresponding values. These should be all of
		/// the parameters added in <see cref="SetUpInsertCommand(string, string, List{string})"/>.</param>
		/// <param name="transaction">The transaction this command belongs to, if any. If null, it will be
		/// executed immediately.</param>
		void ExecuteInsertCommand(
			string commandName, List<KeyValuePair<string, string>> paramsToInsert, T? transaction = null);

		/// <summary>
		/// Clears all rows from a given table.
		/// </summary>
		/// <param name="tableName">The table to clear.</param>
		void Clear(string tableName);

		/// <summary>
		/// Commits all pending inserts/updates and closes the transaction.
		/// </summary>
		/// <param name="transaction">The transaction to close.</param>
		void CommitAndCloseTransaction(T transaction);

		/// <summary>
		/// Gets all rows in the database matching a condition, in a order based on the orderingString.
		/// </summary>
		/// <param name="tableName">The name of the table to query.</param>
		/// <param name="rowCondition">The row condition that returned rows will match.</param>
		/// <param name="ordering">The ordering string determining the order rows are returned in.</param>
		/// <returns></returns>
		IEnumerable GetRows(string tableName, string rowCondition, string ordering);

		/// <summary>
		/// Applies and index to a given column in a given table.
		/// </summary>
		/// <param name="tableName">The name of the table containing the column to index.</param>
		/// <param name="indexName">The name of the index (for internal referencing)</param>
		/// <param name="columnName">The name of the column to apply the index to.</param>
		void IndexColumn(string tableName, string indexName, string columnName);

		/// <summary>
		/// Disconnects from the database. Anything pending will be rolled back.
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Deletes the database file on disk
		/// </summary>
		void Delete();
	}
}
