namespace CoreUtilities.Services.Database
{
    using CoreUtilities.HelperClasses;
    using CoreUtilities.HelperClasses.Extensions;
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

        private readonly IDatabaseWrapperContext<TData> context;

        private readonly Dictionary<string, int> primaryKeyMappings = new Dictionary<string, int>();

        private int count = 0;
        private readonly ConcurrentDictionary<int, SQLiteDataReader> rowReaders = new();

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
            this.database = databaseService;
            this.DatabaseName = new FileInfo(path).Name;
            this.context = context;

            var columnsToAdd = context.GetColumns().Select(x =>
                    new KeyValuePair<string, string>(x.Key, x.Value.GetEnumDescription()))
                .Union(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>(dateTimeColumnName, "TEXT"),
                        new KeyValuePair<string, string>(primaryKeyColumnName, "INTEGER"),
                    })
                .ToArray();

            this.database.AddTableAndColumns(tableName, columnsToAdd, context.GetColumnsToIndex());
            this.database.SetUpUpdateCommand(
                tableName, updateRowCommandName, columnsToAdd.Select(x => x.Key).ToList(), primaryKeyColumnName);
            this.database.SetUpInsertCommand(tableName, insertRowCommandName, columnsToAdd.Select(x => x.Key).ToList());

            if (recreate)
                return;

            foreach (object item in this.AllRows().rows)
            {
                var reader = item as IDataRecord;
                if (reader == null) continue;
                this.primaryKeyMappings[context.GetPrimaryKey(context.GetValueFromDbType(reader))] =
                    Convert.ToInt32(reader[primaryKeyColumnName]);
            }

            this.rowCount = this.primaryKeyMappings.Count;
        }

        /// <inheritdoc/>
        public (int reference, IEnumerable<object> rows) AllRows()
        {
            SQLiteDataReader reader = (this.database.GetRows(tableName, "",
                this.GenerateOrderingString(dateTimeColumnName, Ordering.Descending)) as SQLiteDataReader)!;
            var success = this.rowReaders.TryAdd(this.count, reader);
            if (!success)
            {
                Debug.WriteLine("Failed to add row reader");
            }
            var result = (this.count, reader.Cast<object>());
            this.count++;
            return result;
        }

        /// <inheritdoc/>
        public void CloseRowReader(int reference)
        {
            var reader = this.rowReaders[reference];
            reader.Close();
            reader.Dispose();
            this.rowReaders.Remove(reference, out _);
        }

        /// <inheritdoc/>
        public void ClearAllRows()
        {
            this.database.Clear(tableName);
        }

        /// <inheritdoc/>
        public int RowCount(Func<TData, bool>? selector = null)
        {
            if (selector == null)
            {
                return (int)this.database.RowCount(tableName, "");
            }
            else
            {
                int count = 0;

                SQLiteDataReader reader =
                (this.database.GetRows(
                    tableName,
                    "",
                    this.GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
                as SQLiteDataReader)!;

                while (reader.Read())
                {
                    if (selector(this.context.GetValueFromDbType(reader)))
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
            TTransaction transaction = this.database.GetAndOpenWriteTransaction();
            foreach (TData row in list)
            {
                if (this.breakOperation)
                    break;

                var itemValues = this.context.GetDbCompatibleItem(row).Union(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>(dateTimeColumnName, this.context.GetDate(row).Ticks.ToString()),
                        new KeyValuePair<string, string>(primaryKeyColumnName, this.rowCount.ToString()),
                    }).ToList();

                this.primaryKeyMappings[this.context.GetPrimaryKey(row)] = this.rowCount;
                this.rowCount++;

                this.database.ExecuteInsertCommand(insertRowCommandName, itemValues, transaction);
            }
            this.database.CommitAndCloseTransaction(transaction);
            transaction.Dispose();
        }

        /// <inheritdoc/>
        public void Add(TData row)
        {
            var itemValues = this.context.GetDbCompatibleItem(row).Union(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(dateTimeColumnName, this.context.GetDate(row).Ticks.ToString()),
                    new KeyValuePair<string, string>(primaryKeyColumnName, this.rowCount.ToString()),
                }).ToList();

            this.database.ExecuteInsertCommand(insertRowCommandName, itemValues, this.writeTransaction);

            this.primaryKeyMappings[this.context.GetPrimaryKey(row)] = this.rowCount;
            this.rowCount++;
        }

        /// <inheritdoc/>
        public IEnumerable<TData> GetConvertedRowsBetweenIndices(
            int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool>? selector = null)
        {
            SQLiteDataReader reader =
                (this.database.GetRows(
                    tableName,
                    "",
                    this.GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
                as SQLiteDataReader)!;

            List<TData> list = new List<TData>();

            int i = 0;
            while (reader.Read())
            {
                var item = this.context.GetValueFromDbType(reader);
                var allowed = selector != null ? selector(item) : true;

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
                    list.Add(this.context.GetValueFromDbType(reader));
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
                (this.database.GetRows(
                    tableName,
                    "",
                    this.GenerateOrderingString(dateTimeColumnName, Ordering.Descending))
                as SQLiteDataReader)!;

            List<TData> list = new List<TData>();

            while (reader.Read())
            {
                var item = this.context.GetValueFromDbType(reader);
                if (selector == null || selector(item))
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
            this.writeTransaction = this.database.GetAndOpenWriteTransaction();
        }

        /// <inheritdoc/>
        public void UpdateRow(TData row)
        {
            if (this.breakOperation)
                return;

            var itemValues = this.context.GetDbCompatibleItem(row).Union(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(dateTimeColumnName, this.context.GetDate(row).Ticks.ToString()),
                }).ToList();

            this.database.ExecuteUpdateCommand(
                updateRowCommandName,
                itemValues,
                new KeyValuePair<string, string>(
                    primaryKeyColumnName, this.primaryKeyMappings[this.context.GetPrimaryKey(row)].ToString()),
                this.writeTransaction);
        }

        /// <inheritdoc/>
        public void CloseWriteTransaction()
        {
            if (this.writeTransaction == null) return;

            this.database.CommitAndCloseTransaction(this.writeTransaction);
            this.writeTransaction.Dispose();
            this.writeTransaction = null;
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            this.breakOperation = true;
            if (this.rowReaders.Any())
            {
                foreach (var reader in this.rowReaders.Values)
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.rowReaders.Clear();
            }
            // Closing connections with transactions open should simply roll them back
            this.database.Disconnect();
		}

		/// <inheritdoc/>
		public void Delete()
		{
            this.database.Delete();
		}

		private string GenerateOrderingString(string columnName, Ordering order)
        {
            string orderingString = order == Ordering.Ascending ? "ASC" : "DESC";
            return $"ORDER BY {columnName} {orderingString}";
        }
    }
}