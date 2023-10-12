using CoreUtilities.Interfaces.Database;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.Database
{
	/// <summary>
	/// Wrapper class for enabling builder pattern usage of write transactions into a 
	/// <see cref="IDatabaseWrapperService{TData}"/> instance.
	/// </summary>
	/// <typeparam name="T">The data type to be stored.</typeparam>
	public class WriteTransactionWrapper<T>
	{
		private IDatabaseWrapperService<T> database;

		/// <summary>
		/// Constructor for the <see cref="WriteTransactionWrapper{T}"/>.
		/// </summary>
		/// <param name="database">The <see cref="IDatabaseWrapperService{TData}"/> this wrapper should interact 
		/// with.</param>
		public WriteTransactionWrapper(IDatabaseWrapperService<T> database)
		{
			this.database = database;
		}

		/// <summary>
		/// Adds an entry to the <see cref="IDatabaseWrapperService{TData}"/>.
		/// Should be called after something provides and instance of a <see cref="WriteTransactionWrapper{T}"/>.
		/// </summary>
		/// <param name="entry">The entry to add of type <see cref="T"/>.</param>
		/// <returns>This <see cref="WriteTransactionWrapper{T}"/> class, enabling the builder pattern to continue.</returns>
		public WriteTransactionWrapper<T> WithEntry(T entry)
		{
			database.Add(entry);
			return this;
		}

		/// <summary>
		/// Adds a <see cref="IEnumerable{T}"/> of entries to add to the <see cref="IDatabaseWrapperService{TData}"/>.
		/// Should be called after something provides and instance of a <see cref="WriteTransactionWrapper{T}"/>.
		/// </summary>
		/// <param name="entries">The list of entries to add.</param>
		/// <returns></returns>
		public WriteTransactionWrapper<T> WithEntrys(IEnumerable<T> entries)
		{
			database.AddRange(entries);
			return this;
		}

		/// <summary>
		/// Executes all pending writes and closes the transaction. Should be performed after 
		/// <see cref="WithEntrys(IEnumerable{T})"/> or <see cref="WithEntry(T)"/> calls.
		/// </summary>
		public void ExecuteWrite()
		{
			database.CloseWriteTransaction();
		}
	}
}
