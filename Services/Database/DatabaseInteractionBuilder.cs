namespace CoreUtilities.Services.Database
{
    using CoreUtilities.HelperClasses.Database;
    using CoreUtilities.Interfaces.Database;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of <see cref="IDatabaseInteractionBuilder{TData}"/>. Class provides clean build pattern type ways of
    /// interacting with a <see cref="IDatabaseWrapperService{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">The data type the <see cref="IDatabaseWrapperService{TData}"/> is storing.</typeparam>
    public class DatabaseInteractionBuilder<TData> : IDatabaseInteractionBuilder<TData>
    {
        private readonly IDatabaseWrapperService<TData> database;

        /// <summary>
        /// Constructor for <see cref="DatabaseInteractionBuilder{TData}"/>.
        /// </summary>
        /// <param name="databaseWrapper">The <see cref="IDatabaseWrapperService{TData}"/> this class should use for
        /// building commands to interact with.</param>
        public DatabaseInteractionBuilder(IDatabaseWrapperService<TData> databaseWrapper)
        {
            this.database = databaseWrapper;
        }

        /// <inheritdoc/>
        public WriteTransactionWrapper<TData> GetWriteTransaction()
        {
            this.database.OpenWriteTransaction();
            return new WriteTransactionWrapper<TData>(this.database);
        }

        /// <inheritdoc/>
        public UpdateTransactionWrapper<TData> GetUpdateTransaction()
        {
            this.database.OpenWriteTransaction();
            return new UpdateTransactionWrapper<TData>(this.database);
        }

        /// <inheritdoc/>
        public ReaderInstanceWrapper<TData> GetReader()
        {
            return new ReaderInstanceWrapper<TData>(this.database);
        }

        /// <inheritdoc/>
        public ReaderInstanceWrapper<TData, TReturn> GetReader<TReturn>()
        {
            return new ReaderInstanceWrapper<TData, TReturn>(this.database);
        }

        /// <inheritdoc/>
        public IEnumerable<TData> GetConvertedInstancesBetweenIndices(
            int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool>? selectionCriteria = null)
        {
            var result = this.database.GetConvertedRowsBetweenIndices(
                startIndex, endIndex, defaultCreator, selectionCriteria);
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<TData> GetConvertedInstances(Func<TData, bool>? selectionCriteria = null)
        {
            return this.database.GetConvertedRows(selectionCriteria);
        }

        /// <inheritdoc/>
        public int RowCount(Func<TData, bool>? selector = null)
        {
            return this.database.RowCount(selector);
        }

        /// <inheritdoc/>
        public void ClearDatabase()
        {
            this.database.ClearAllRows();
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            this.database.Disconnect();
        }

		/// <inheritdoc/>
		public void Delete()
        {
            this.Disconnect();
            this.database.Delete();
        }
    }
}
