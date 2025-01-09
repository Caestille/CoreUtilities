namespace CoreUtilities.Services.Database
{
    using CoreUtilities.Interfaces.Database;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    /// <summary>
    /// Implementation of <see cref="IDatabaseService{T}"/> using SQlite.
    /// </summary>
    public class SqLiteDatabaseService : IDatabaseService<SQLiteTransaction>
	{
		private readonly string connectionString;

		private readonly SQLiteConnection readConnection;
		private readonly SQLiteConnection writeConnection;

		private readonly Dictionary<string, List<string>> currentTablesAndColumns = new();

		private readonly Dictionary<string, SQLiteCommand> commands = new();
		private readonly Dictionary<string, List<SQLiteParameter>> commandParameters = new();
		private readonly Dictionary<string, SQLiteParameter> commandCondtionalParameters = new();

		private readonly string dbPath;

		/// <summary>
		/// Constructor for the <see cref="SqLiteDatabaseService"/>. Creates the database if needed and opens read and
		/// write connections.
		/// </summary>
		/// <param name="path">The path on which to create the database.</param>
		/// <param name="recreate">Whether initialisation should recreate the database (will overwrite if exists
		/// and true).</param>
		public SqLiteDatabaseService(string path, bool recreate)
		{
            this.connectionString = $"Data Source={path};Version=3;";
			var dir = Path.GetDirectoryName(path);
			if (dir!= null && !Directory.Exists(dir)) 
			{
				Directory.CreateDirectory(dir);
			}

			if (recreate)
				SQLiteConnection.CreateFile(path);

            this.dbPath = path;

            this.readConnection = new SQLiteConnection(this.connectionString);
            this.writeConnection = new SQLiteConnection(this.connectionString);
            this.readConnection.Open();
            this.writeConnection.Open();
		}

		/// <inheritdoc/>
		public void AddTableAndColumns(string tableName, KeyValuePair<string, string>[] columnNamesAndDataTypes,
			string[] columnsToIndex)
		{
            this.CreateTableIfNeeded(tableName);
			foreach (KeyValuePair<string, string> columnAndDataType in columnNamesAndDataTypes)
			{
                this.AddColumnToTableIfNeeded(tableName, columnAndDataType.Key, columnAndDataType.Value);
			}

			foreach (string columnToIndex in columnsToIndex)
			{
                this.IndexColumn(tableName, columnToIndex + "Index", columnToIndex);
			}
		}

		/// <inheritdoc/>
		public void SetUpUpdateCommand(
			string tableName, string commandName, List<string> parametersToAdd, string conditionalMatchParameter)
		{
			SQLiteCommand command = new SQLiteCommand(this.writeConnection);
			string updateText = string.Empty;
			foreach (string name in parametersToAdd)
            {
                updateText +=
					$"{name} = ${name}" + (parametersToAdd.IndexOf(name) != parametersToAdd.Count - 1 ? ", " : string.Empty);
            }

            string conditionalText = $"{conditionalMatchParameter} = ${conditionalMatchParameter}";
			string commandText = $"UPDATE {tableName} SET {updateText} WHERE {conditionalText};";
			command.CommandText = commandText;
			if (!this.commandParameters.ContainsKey(commandName))
                this.commandParameters[commandName] = new List<SQLiteParameter>();
			foreach (string name in parametersToAdd)
			{
				SQLiteParameter param = command.CreateParameter();
				param.ParameterName = name;
				command.Parameters.Add(param);
                this.commandParameters[commandName].Add(param);
			}

			SQLiteParameter conditionalParam = command.CreateParameter();
			conditionalParam.ParameterName = conditionalMatchParameter;
			command.Parameters.Add(conditionalParam);
            this.commandCondtionalParameters[commandName] = conditionalParam;

            this.commands[commandName] = command;
		}

		/// <inheritdoc/>
		public void SetUpInsertCommand(string tableName, string commandName, List<string> parametersToAdd)
		{
			SQLiteCommand command = new SQLiteCommand(this.writeConnection);
			string columnNames = string.Empty;
			foreach (string name in parametersToAdd)
				columnNames += $"{name}" + (parametersToAdd.IndexOf(name) != parametersToAdd.Count - 1 ? ", " : string.Empty);
			string values = string.Empty;
			foreach (string name in parametersToAdd)
				values += $"${name}" + (parametersToAdd.IndexOf(name) != parametersToAdd.Count - 1 ? ", " : string.Empty);
			string commandText = $"INSERT INTO {tableName} ({columnNames}) VALUES ({values});";
			command.CommandText = commandText;
			if (!this.commandParameters.ContainsKey(commandName))
                this.commandParameters[commandName] = new List<SQLiteParameter>();
			foreach (string name in parametersToAdd)
			{
				SQLiteParameter param = command.CreateParameter();
				param.ParameterName = name;
				command.Parameters.Add(param);
                this.commandParameters[commandName].Add(param);
			}

            this.commands[commandName] = command;
		}

		/// <inheritdoc/>
		public SQLiteTransaction GetAndOpenWriteTransaction()
		{
			return this.writeConnection.BeginTransaction();
		}

		/// <inheritdoc/>
		public long RowCount(string tableName, string condition)
		{
			SQLiteCommand cmd = new SQLiteCommand(this.readConnection);

			cmd.CommandText = $"SELECT COUNT(*) FROM {tableName} {condition};";
			return Convert.ToInt32(cmd.ExecuteScalar());
		}

		/// <inheritdoc/>
		public void ExecuteUpdateCommand(
			string commandName,
			List<KeyValuePair<string, string>> paramsToUpdate,
			KeyValuePair<string, string> conditionalParamToUpdate,
			SQLiteTransaction? transaction = null)
		{
			foreach (KeyValuePair<string, string> param in paramsToUpdate)
                this.commandParameters[commandName].Find(x => x.ParameterName == param.Key)!.Value = param.Value;

            this.commandCondtionalParameters[commandName].Value = conditionalParamToUpdate.Value;

			SQLiteCommand command = this.commands[commandName];
			if (transaction != null && command.Transaction != transaction)
				command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		/// <inheritdoc/>
		public void ExecuteInsertCommand(string commandName,
			List<KeyValuePair<string, string>> paramsToInsert,
			SQLiteTransaction? transaction = null)
		{
			foreach (KeyValuePair<string, string> param in paramsToInsert)
                this.commandParameters[commandName].Find(x => x.ParameterName == param.Key)!.Value = param.Value;

			SQLiteCommand command = this.commands[commandName];
			if (transaction != null && command.Transaction != transaction)
				command.Transaction = transaction;
			command.ExecuteNonQuery();
		}

		/// <inheritdoc/>
		public void Clear(string tableName)
		{
			SQLiteCommand cmd = new SQLiteCommand(this.writeConnection);

			cmd.CommandText = $"DELETE FROM {tableName};";
			cmd.ExecuteNonQuery();
		}

		/// <inheritdoc/>
		public void CommitAndCloseTransaction(SQLiteTransaction transaction)
		{
			transaction.Commit();
			transaction.Dispose();
		}

		/// <inheritdoc/>
		public IEnumerable GetRows(string tableName, string rowCondition, string ordering)
		{
			SQLiteCommand cmd = new SQLiteCommand(this.readConnection);

			cmd.CommandText = $"SELECT * FROM {tableName} {rowCondition} {ordering};";
			SQLiteDataReader reader = cmd.ExecuteReader();

			return reader;
		}

		/// <inheritdoc/>
		public void IndexColumn(string tableName, string indexName, string columnName)
		{
			SQLiteCommand cmd2 = new SQLiteCommand(this.writeConnection);
			cmd2.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS {indexName} ON {tableName} ({columnName});";
			cmd2.ExecuteNonQuery();
		}

		/// <inheritdoc/>
		public void Disconnect()
		{
			SQLiteConnection.ClearAllPools();
            this.readConnection.Close();
            this.writeConnection.Close();
			foreach (var command in this.commands.Values)
			{
				command.Dispose();
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		/// <inheritdoc/>
		public void Delete()
		{
			File.Delete(this.dbPath);
		}

		/// <summary>
		/// Creates a table if it does not already exist
		/// </summary>
		/// <param name="tableName">The name of the table to create.</param>
		private void CreateTableIfNeeded(string tableName)
		{
			if (!this.currentTablesAndColumns.ContainsKey(tableName))
			{
				SQLiteCommand cmd = new SQLiteCommand(this.writeConnection);
				cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName} (Id INTEGER);";
				cmd.ExecuteNonQuery();
                this.currentTablesAndColumns[tableName] = new List<string>();
			}
		}

		/// <summary>
		/// Adds a column of a given name to a table of a given name if it does not already exist.
		/// </summary>
		/// <param name="tableName">The name of the table to add a column to.</param>
		/// <param name="columnName">The name of the column to add.</param>
		/// <param name="dataType">The data type of the column to be added.</param>
		private void AddColumnToTableIfNeeded(string tableName, string columnName, string dataType)
		{
			if (!this.currentTablesAndColumns[tableName].Contains(columnName))
			{
				try
				{
					SQLiteCommand cmd = new SQLiteCommand(this.writeConnection);

					cmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {dataType};";
					cmd.ExecuteNonQuery();
				}
				catch (SQLiteException)
                {
					// column already existed in table. Do nothing
				}
			}

            this.currentTablesAndColumns[tableName].Add(columnName);
		}
	}
}
