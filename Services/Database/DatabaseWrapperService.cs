using CoreUtilities.HelperClasses;
using CoreUtilities.Interfaces.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CoreUtilities.Services.Database
{
    /// <summary>
    /// Implementation of <see cref="IDatabaseWrapperService{TData}"/>. Wrapper service which wraps raw usage of a 
    /// database implementation, more convenient. Requires that the data type have some sort of datetime representation
    /// for storage.
    /// </summary>
    /// <typeparam name="TData">The type of data to be stored.</typeparam>
    /// <typeparam name="TTransaction">The transaction type to be used.</typeparam>
    public class DatabaseWrapperService<TData, TTransaction>
        : IDatabaseWrapperService<TData> where TTransaction : DbTransaction
    {
        private readonly IDatabaseService<TTransaction> database;

        private const string tableName = "MainTable";
        private const string dateTimeColumnName = "DateTime";
        private const string primaryKeyColumnName = "Id";

        private int rowCount;

        private IDatabaseWrapperContext<TData> context;

        private Dictionary<string, int> primaryKeyMappings = new Dictionary<string, int>();

        private int count = 0;
        private ConcurrentDictionary<int, SQLiteDataReader> rowReaders = new();

        private const string updateRowCommandName = "updateRow";
        private const string insertRowCommandName = "insertRow";

        private TTransaction? writeTransaction;

        private bool breakOperation;

        /// <inheritdoc/>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Constructor for the <see cref="DatabaseWrapperService"/>. Initialises the database and sets it up.
        /// </summary>
        /// <param name="path">The path of the database to be initialised/connected to.</param>
        /// <param name="recreate">Whether the database should be recreated/overwritten.</param>
        /// <param name="databaseService">The <see cref="IDatabaseService{T}"/> this wrapper, wraps.</param>
        /// <param name="context">Implementation of interface <see cref="IDatabaseWrapperContext{TData}"/> 
        /// which provides methods to get values and convert to and from <see cref="TData"/> and 
        /// <see cref="TTransaction"/> types.
        public DatabaseWrapperService(
            string path,
            bool recreate,
            IDatabaseService<TTransaction> databaseService,
            IDatabaseWrapperContext<TData> context)
        {
            database = databaseService;
            DatabaseName = new FileInfo(path).Name;
            this.context = context;

            var columnsToAdd = context.GetColumns().Select(x =>
                    new KeyValuePair<string, string>(x.Key, x.Value.GetEnumDescription()))
                .Union(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>(dateTimeColumnName, "TEXT"),
                        new KeyValuePair<string, string>(primaryKeyColumnName, "INTEGER"),
                    })
                .ToArray();

            database.AddTableAndColumns(tableName, columnsToAdd, context.GetColumnsToIndex());
            database.SetUpUpdateCommand(
                tableName, updateRowCommandName, columnsToAdd.Select(x => x.Key).ToList(), primaryKeyColumnName);
            database.SetUpInsertCommand(tableName, insertRowCommandName, columnsToAdd.Select(x => x.Key).ToList());

            if (recreate)
                return;

            foreach (object item in AllRows().rows)
            {
                var reader = item as IDataRecord;
                if (reader == null) continue;
                primaryKeyMappings[context.GetPrimaryKey(context.GetValueFromDbType(reader))] =
                    Convert.ToInt32(reader[primaryKeyColumnName]);
            }

            rowCount = primaryKeyMappings.Count;
        }

        /// <inheritdoc/>
        public (int reference, IEnumerable<object> rows) AllRows()
        {
            SQLiteDataReader reader = (database.GetRows(tableName, "",
                GenerateOrderingString(dateTimeColumnName, Ordering.Descending)) as SQLiteDataReader)!;
            var success = rowReaders.TryAdd(count, reader);
            if (!success)
            {
                Debug.WriteLine("Failed to add row reader");
            }
            var result = (count, reader.Cast<object>());
            count++;
            return result;
        }

        /// <inheritdoc/>
        public void CloseRowReader(int reference)
        {
            var reader = rowReaders[reference];
            reader.Close();
            reader.Dispose();
            rowReaders.Remove(reference, out _);
        }

        /// <inheritdoc/>
        public void ClearAllRows()
        {
            database.Clear(tableName);
        }

        /// <inheritdoc/>
        public int RowCount(Func<TData, bool>? selector = null)
        {
            if (selector == null)
            {
                return (int)database.RowCount(tableName, "");
            }
            else
            {
                int count = 0;

                SQLiteDataReader reader =
                (database.GetRows(
                    tableName,
                    "",
                    GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
                as SQLiteDataReader)!;

                while (reader.Read())
                {
                    if (selector(context.GetValueFromDbType(reader)))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<TData> list)
        {
            TTransaction transaction = database.GetAndOpenWriteTransaction();
            foreach (TData row in list)
            {
                if (breakOperation)
                    break;

                var itemValues = context.GetDbCompatibleItem(row).Union(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>(dateTimeColumnName, context.GetDate(row).Ticks.ToString()),
                        new KeyValuePair<string, string>(primaryKeyColumnName, rowCount.ToString()),
                    }).ToList();

                primaryKeyMappings[context.GetPrimaryKey(row)] = rowCount;
                rowCount++;

                database.ExecuteInsertCommand(insertRowCommandName, itemValues, transaction);
            }
            database.CommitAndCloseTransaction(transaction);
            transaction.Dispose();
        }

        /// <inheritdoc/>
        public void Add(TData row)
        {
            var itemValues = context.GetDbCompatibleItem(row).Union(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(dateTimeColumnName, context.GetDate(row).Ticks.ToString()),
                    new KeyValuePair<string, string>(primaryKeyColumnName, rowCount.ToString()),
                }).ToList();

            database.ExecuteInsertCommand(insertRowCommandName, itemValues, writeTransaction);

            primaryKeyMappings[context.GetPrimaryKey(row)] = rowCount;
            rowCount++;
        }

        /// <inheritdoc/>
        public IEnumerable<TData> GetConvertedRowsBetweenIndices(
            int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool>? selector = null)
        {
            SQLiteDataReader reader =
                (database.GetRows(
                    tableName,
                    "",
                    GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
                as SQLiteDataReader)!;

            List<TData> list = new List<TData>();
            var doSelection = selector != null;

            int i = 0;
            while (reader.Read())
            {
                var item = context.GetValueFromDbType(reader);
                var allowed = !doSelection || selector(item);

                if (i < startIndex)
                {
                    if (allowed)
                    {
                        i++;
                    }
                    continue;
                }

                if (i > endIndex)
                {
                    break;
                }

                if (allowed)
                {
                    list.Add(context.GetValueFromDbType(reader));
                    i++;
                }
            }

            reader.Close();
            reader.Dispose();

            var count = list.Count();
            if (count < endIndex - startIndex)
            {
                for (i = 0; i < endIndex - startIndex - count; i++)
                {
                    list.Add(defaultCreator());
                }
            }

            return list;
        }

        /// <inheritdoc/>
        public IEnumerable<TData> GetConvertedRows(Func<TData, bool>? selector = null)
        {
            SQLiteDataReader reader =
                (database.GetRows(
                    tableName,
                    "",
                    GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
                as SQLiteDataReader)!;

            List<TData> list = new List<TData>();
            var doSelection = selector != null;

            while (reader.Read())
            {
                var item = context.GetValueFromDbType(reader);
                if (!doSelection || selector(item))
                {
                    list.Add(item);
                }
            }

            reader.Close();
            reader.Dispose();

            return list;
        }

        /// <inheritdoc/>
        public void OpenWriteTransaction()
        {
            writeTransaction = database.GetAndOpenWriteTransaction();
        }

        /// <inheritdoc/>
        public void UpdateRow(TData row)
        {
            if (breakOperation)
                return;

            var itemValues = context.GetDbCompatibleItem(row).Union(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(dateTimeColumnName, context.GetDate(row).Ticks.ToString()),
                }).ToList();

            database.ExecuteUpdateCommand(
                updateRowCommandName,
                itemValues,
                new KeyValuePair<string, string>(
                    primaryKeyColumnName, primaryKeyMappings[context.GetPrimaryKey(row)].ToString()),
                writeTransaction);
        }

        /// <inheritdoc/>
        public void CloseWriteTransaction()
        {
            database.CommitAndCloseTransaction(writeTransaction);
            writeTransaction.Dispose();
            writeTransaction = null;
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            breakOperation = true;
            if (rowReaders.Any())
            {
                foreach (var reader in rowReaders.Values)
                {
                    reader.Close();
                    reader.Dispose();
                }
                rowReaders.Clear();
            }
            // Closing connections with transactions open should simply roll them back
            database.Disconnect();
        }

        private string GenerateOrderingString(string columnName, Ordering order)
        {
            string orderingString = order == Ordering.Ascending ? "ASC" : "DESC";
            return $"ORDER BY {columnName} {orderingString}";
        }
    }
}