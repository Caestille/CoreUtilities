using CoreUtilities.HelperClasses.Database;
using System;
using System.Collections.Generic;

namespace CoreUtilities.Interfaces.Database
{
    /// <summary>
    /// An interface for a high level wrapper providing friendly builder pattern type database interactions.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDatabaseInteractionBuilder<TData>
    {
        /// <summary>
        /// A friendly way to get a write transaction, add any number of writes, and then execute it.
        /// </summary>
        /// <returns>A <see cref="WriteTransactionWrapper{T}"/>.</returns>
        WriteTransactionWrapper<TData> GetWriteTransaction();

        /// <summary>
        /// A friendly way to get an update transaction, add any number of updates, and then execute it.
        /// </summary>
        /// <returns>An <see cref="UpdateTransactionWrapper{T}"/>.</returns>
        UpdateTransactionWrapper<TData> GetUpdateTransaction();

        /// <summary>
        /// A friendly way to read rows in a database, with no return data.
        /// </summary>
        /// <returns>A <see cref="ReaderInstanceWrapper{TData}"/>.</returns>
        ReaderInstanceWrapper<TData> GetReader();

        /// <summary>
        /// A friendly way to read rows in a database and return something.
        /// </summary>
        /// <typeparam name="TReturn">The type of data/object to be returned.</typeparam>
        /// <returns>A <see cref="ReaderInstanceWrapper{TData, TReturn}"/>.</returns>
        ReaderInstanceWrapper<TData, TReturn> GetReader<TReturn>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="defaultCreator"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEnumerable<TData> GetConvertedInstancesBetweenIndices(
            int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool>? selector = null);

        /// <summary>
        /// Gets all rows which (optionally) match a <see cref="Func{T, TResult}"/> selection criteria.
        /// </summary>
        /// <param name="selector">A <see cref="Func{T, TResult}"/> selector which given a row, returns an indication
        /// of whether the row passes the selection criteria.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> which is all rows matching the selection criteria (or all
        /// if null), converted using the database conversion func.</returns>
        IEnumerable<TData> GetConvertedInstances(Func<TData, bool>? selector = null);

        /// <summary>
        /// Gets the row count of all rows which (optionally) match a <see cref="Func{T, TResult}"/> selection 
        /// criteria.
        /// </summary>
        /// <param name="selector">A <see cref="Func{T, TResult}"/> selector which given a row, returns an indication
        /// of whether the row passes the selection criteria.</param>
        /// <returns>A <see cref="int"/> which is the count of all rows matching the selection criteria (or all
        /// if null).</returns>
        int RowCount(Func<TData, bool>? selector = null);

        /// <summary>
        /// Removes all rows from the database.
        /// </summary>
        void ClearDatabase();

        /// <summary>
        /// Disconnects from the database. Any pending transactions are rolled back automatically.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Deletes the database file on disk
        /// </summary>
        void Delete();
    }
}
