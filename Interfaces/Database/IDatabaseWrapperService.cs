using System;
using System.Collections.Generic;

namespace CoreUtilities.Interfaces.Database
{
    /// <summary>
    /// An interface for classes wrapping databases with common interactions to inherit.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDatabaseWrapperService<TData>
    {
        /// <summary>
        /// The name of the database on disk.
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> which can be iterated over to get all rows, along with a reference
        /// which should be retained for closing the read operation.
        /// </summary>
        /// <returns>A tuple containing the reference and <see cref="IEnumerable{T}"/>.</returns>
        (int reference, IEnumerable<object> rows) AllRows();

        /// <summary>
        /// Closes the read operation of a given reference, which should have been obtained when opening the read
        /// operation.
        /// </summary>
        /// <param name="reference">The refernce to the reader to close.</param>
        void CloseRowReader(int reference);

        /// <summary>
        /// Removes all rows from the database.
        /// </summary>
        void ClearAllRows();

        /// <summary>
        /// Gets the row count of rows in the database which can optionally match a <see cref="Func{T, TResult}"/>
        /// selection functor.
        /// </summary>
        /// <param name="selector">A <see cref="Func{T, TResult}"/> which each row is evaluated against (if set).
        /// If not the row count is not incremented for that row.</param>
        /// <returns>An <see cref="int"/> which is the row count of all rows matching the selector (if set), or
        /// all rows if now.</returns>
        int RowCount(Func<TData, bool> selector = null);

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of values to the database.
        /// </summary>
        /// <param name="list">The <see cref="IEnumerable{T}"/> of values to insert.</param>
        void AddRange(IEnumerable<TData> list);

        /// <summary>
        /// Adds a single value to the database.
        /// </summary>
        /// <param name="row">The value to add.</param>
        void Add(TData row);

        /// <summary>
        /// Gets rows between two indices optionally matching a <see cref="Func{T, TResult}"/> selector. Rows not
        /// matching the selector do not count towards the row count, so you will always receive startIndex-endIndex
        /// rows. If no rows exist for a given index, a <see cref="Func{T}"/> provides the default to insert.
        /// </summary>
        /// <param name="startIndex">The beginning index to start adding matching (or all) rows to the output.</param>
        /// <param name="endIndex">The final index at which to stop adding matching (or all) rows to the 
        /// output.</param>
        /// <param name="defaultCreator">A <see cref="Func{T}"/> to create a default should a value not exist at
        /// a given index.</param>
        /// <param name="selector">A <see cref="Func{T, TResult}"/> which each row is evaluated against to add
        /// to the output.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing results converted to the output type.</returns>
        IEnumerable<TData> GetConvertedRowsBetweenIndices(
            int startIndex, int endIndex, Func<TData> defaultCreator, Func<TData, bool>? selector = null);

        /// <summary>
        /// Gets all rows matching an optional selection criteria.
        /// </summary>
        /// <param name="selector">A <see cref="Func{T, TResult}"/> which each row is evaluated against to add
        /// to the output.</param>
        /// <returns></returns>
        IEnumerable<TData> GetConvertedRows(Func<TData, bool> selector = null);

        /// <summary>
        /// Begins a write transaction in the database, which can have insertions added to and then committed/closed.
        /// </summary>
        void OpenWriteTransaction();

        /// <summary>
        /// Updates a row (identified automatically) with the given row.
        /// </summary>
        /// <param name="row">The row to update.</param>
        void UpdateRow(TData row);

        /// <summary>
        /// Closes an open write transaction. These are not concurrent so no reference is returned.
        /// </summary>
        void CloseWriteTransaction();

        /// <summary>
        /// Closes all pending reads and writes (uncommitted writes will be rolled back) and disconnects from the
        /// database, disposing related resources.
        /// </summary>
        void Disconnect();

		/// <summary>
		/// Deletes the database file on disk
		/// </summary>
		void Delete();
	}
}