using CoreUtilities.Interfaces;

namespace CoreUtilities.HelperClasses.Database
{
	/// <summary>
	/// Wrapper class for enabling builder pattern usage of update transactions into a 
	/// <see cref="IDatabaseWrapperService{TData}"/> instance.
	/// </summary>
	/// <typeparam name="T">The data type to be stored.</typeparam>
	public class UpdateTransactionWrapper<T>
	{
		private IDatabaseWrapperService<T> database;

		/// <summary>
		/// Constructor for the <see cref="UpdateTransactionWrapper{T}"/>.
		/// </summary>
		/// <param name="database">The <see cref="IDatabaseWrapperService{TData}"/> this wrapper should interact 
		/// with.</param>
		public UpdateTransactionWrapper(IDatabaseWrapperService<T> database)
		{
			this.database = database;
		}

		/// <summary>
		/// Updates an entry in the <see cref="IDatabaseWrapperService{TData}"/> with the values of the provided
		/// entry.
		/// Should be called after something provides and instance of a <see cref="UpdateTransactionWrapper{T}"/>.
		/// </summary>
		/// <param name="entry">The entry to update and get values from.</param>
		/// <returns>This <see cref="UpdateTransactionWrapper{T}"/> class, enabling the builder pattern to continue.</returns>
		public UpdateTransactionWrapper<T> UpdateEntry(T entry)
		{
			database.UpdateRow(entry);
			return this;
		}

		/// <summary>
		/// Executes all pending updates and closes the transaction. Should be performed after 
		/// <see cref="UpdateEntry(T)"/> calls.
		/// </summary>
		public void ExecuteWrite()
		{
			database.CloseWriteTransaction();
		}
	}
}
