using CoreUtilities.Interfaces;

namespace CoreUtilities.HelperClasses.Database
{
	public class UpdateTransactionWrapper<T>
	{
		private IDatabaseWrapperService<T> database;

		public UpdateTransactionWrapper(IDatabaseWrapperService<T> database)
		{
			this.database = database;
		}

		public UpdateTransactionWrapper<T> UpdateEntry(T entry)
		{
			database.UpdateRow(entry);
			return this;
		}

		public void ExecuteWrite()
		{
			database.CloseWriteTransaction();
		}
	}
}
