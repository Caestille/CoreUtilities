﻿using CoreUtilities.Interfaces.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;

namespace CoreUtilities.Services.Database
{
    /// <summary>
    /// Implementation of <see cref="IDatabaseService{T}"/> using SQlite.
    /// </summary>
    public class SqLiteDatabaseService : IDatabaseService<SQLiteTransaction>
    {
        private string connectionString;

        private SQLiteConnection readConnection;
        private SQLiteConnection writeConnection;

        private Dictionary<string, List<string>> currentTablesAndColumns = new();

        private Dictionary<string, SQLiteCommand> commands = new();
        private Dictionary<string, List<SQLiteParameter>> commandParameters = new();
        private Dictionary<string, SQLiteParameter> commandCondtionalParameters = new();

        /// <summary>
        /// Constructor for the <see cref="SqLiteDatabaseService"/>. Creates the database if needed and opens read and
        /// write connections.
        /// </summary>
        /// <param name="path">The path on which to create the database.</param>
        /// <param name="recreate">Whether initialisation should recreate the database (will overwrite if exists
        /// and true).</param>
        public SqLiteDatabaseService(string path, bool recreate)
        {
            connectionString = $"Data Source={path};Version=3;";

            if (recreate)
                SQLiteConnection.CreateFile(path);

            readConnection = new SQLiteConnection(connectionString);
            writeConnection = new SQLiteConnection(connectionString);
            readConnection.Open();
            writeConnection.Open();
        }

        /// <inheritdoc/>
        public void AddTableAndColumns(string tableName, KeyValuePair<string, string>[] columnNamesAndDataTypes,
            string[] columnsToIndex)
        {
            CreateTableIfNeeded(tableName);
            foreach (KeyValuePair<string, string> columnAndDataType in columnNamesAndDataTypes)
            {
                AddColumnToTableIfNeeded(tableName, columnAndDataType.Key, columnAndDataType.Value);
            }

            foreach (string columnToIndex in columnsToIndex)
            {
                IndexColumn(tableName, columnToIndex + "Index", columnToIndex);
            }
        }

        /// <inheritdoc/>
        public void SetUpUpdateCommand(
            string tableName, string commandName, List<string> parametersToAdd, string conditionalMatchParameter)
        {
            SQLiteCommand command = new SQLiteCommand(writeConnection);
            string updateText = "";
            foreach (string name in parametersToAdd)
                updateText +=
                    $"{name} = ${name}" + (parametersToAdd.IndexOf(name) != parametersToAdd.Count - 1 ? ", " : "");
            string conditionalText = $"{conditionalMatchParameter} = ${conditionalMatchParameter}";
            string commandText = $"UPDATE {tableName} SET {updateText} WHERE {conditionalText};";
            command.CommandText = commandText;
            if (!commandParameters.ContainsKey(commandName))
                commandParameters[commandName] = new List<SQLiteParameter>();
            foreach (string name in parametersToAdd)
            {
                SQLiteParameter param = command.CreateParameter();
                param.ParameterName = name;
                command.Parameters.Add(param);
                commandParameters[commandName].Add(param);
            }

            SQLiteParameter conditionalParam = command.CreateParameter();
            conditionalParam.ParameterName = conditionalMatchParameter;
            command.Parameters.Add(conditionalParam);
            commandCondtionalParameters[commandName] = conditionalParam;

            commands[commandName] = command;
        }

        /// <inheritdoc/>
        public void SetUpInsertCommand(string tableName, string commandName, List<string> parametersToAdd)
        {
            SQLiteCommand command = new SQLiteCommand(writeConnection);
            string columnNames = "";
            foreach (string name in parametersToAdd)
                columnNames += $"{name}" + (parametersToAdd.IndexOf(name) != parametersToAdd.Count - 1 ? ", " : "");
            string values = "";
            foreach (string name in parametersToAdd)
                values += $"${name}" + (parametersToAdd.IndexOf(name) != parametersToAdd.Count - 1 ? ", " : "");
            string commandText = $"INSERT INTO {tableName} ({columnNames}) VALUES ({values});";
            command.CommandText = commandText;
            if (!commandParameters.ContainsKey(commandName))
                commandParameters[commandName] = new List<SQLiteParameter>();
            foreach (string name in parametersToAdd)
            {
                SQLiteParameter param = command.CreateParameter();
                param.ParameterName = name;
                command.Parameters.Add(param);
                commandParameters[commandName].Add(param);
            }

            commands[commandName] = command;
        }

        /// <inheritdoc/>
        public SQLiteTransaction GetAndOpenWriteTransaction()
        {
            return writeConnection.BeginTransaction();
        }

        /// <inheritdoc/>
        public long RowCount(string tableName, string condition)
        {
            SQLiteCommand cmd = new SQLiteCommand(readConnection);

            cmd.CommandText = $"SELECT COUNT(*) FROM {tableName} {condition};";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <inheritdoc/>
        public void ExecuteUpdateCommand(
            string commandName,
            List<KeyValuePair<string, string>> paramsToUpdate,
            KeyValuePair<string, string> conditionalParamToUpdate,
            SQLiteTransaction transaction = null)
        {
            foreach (KeyValuePair<string, string> param in paramsToUpdate)
                commandParameters[commandName].Find(x => x.ParameterName == param.Key)!.Value = param.Value;

            commandCondtionalParameters[commandName].Value = conditionalParamToUpdate.Value;

            SQLiteCommand command = commands[commandName];
            if (transaction != null && command.Transaction != transaction)
                command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        /// <inheritdoc/>
        public void ExecuteInsertCommand(string commandName,
            List<KeyValuePair<string, string>> paramsToInsert,
            SQLiteTransaction transaction = null)
        {
            foreach (KeyValuePair<string, string> param in paramsToInsert)
                commandParameters[commandName].Find(x => x.ParameterName == param.Key)!.Value = param.Value;

            SQLiteCommand command = commands[commandName];
            if (transaction != null && command.Transaction != transaction)
                command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        /// <inheritdoc/>
        public void Clear(string tableName)
        {
            SQLiteCommand cmd = new SQLiteCommand(writeConnection);

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
            SQLiteCommand cmd = new SQLiteCommand(readConnection);

            cmd.CommandText = $"SELECT * FROM {tableName} {rowCondition} {ordering};";
            SQLiteDataReader reader = cmd.ExecuteReader();

            return reader;
        }

        /// <inheritdoc/>
        public void IndexColumn(string tableName, string indexName, string columnName)
        {
            SQLiteCommand cmd2 = new SQLiteCommand(writeConnection);
            cmd2.CommandText = $"CREATE UNIQUE INDEX IF NOT EXISTS {indexName} ON {tableName} ({columnName});";
            cmd2.ExecuteNonQuery();
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            SQLiteConnection.ClearAllPools();
            readConnection.Close();
            writeConnection.Close();
            foreach (var command in commands.Values)
            {
                command.Dispose();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Creates a table if it does not already exist
        /// </summary>
        /// <param name="tableName">The name of the table to create.</param>
        private void CreateTableIfNeeded(string tableName)
        {
            if (!currentTablesAndColumns.ContainsKey(tableName))
            {
                SQLiteCommand cmd = new SQLiteCommand(writeConnection);
                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName} (Id INTEGER);";
                cmd.ExecuteNonQuery();
                currentTablesAndColumns[tableName] = new List<string>();
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
            if (!currentTablesAndColumns[tableName].Contains(columnName))
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(writeConnection);

                    cmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {dataType};";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    // column already existed in table. Do nothing
                }
            }

            currentTablesAndColumns[tableName].Add(columnName);
        }
    }
}
