using CoreUtilities.Interfaces;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.Database
{
	public class WriteTransactionWrapper<T>
	{
		private IDatabaseWrapperService<T> database;

		public WriteTransactionWrapper(IDatabaseWrapperService<T> database)
		{
			this.database = database;
		}

		public WriteTransactionWrapper<T> WithEntry(T entry)
		{
			database.Add(entry);
			return this;
		}

		public WriteTransactionWrapper<T> WithEntryRange(IEnumerable<T> entries)
		{
			database.AddRange(entries);
			return this;
		}

		public void ExecuteWrite()
		{
			database.CloseWriteTransaction();
		}
	}
}
