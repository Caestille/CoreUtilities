using CoreUtilities.Interfaces;
using System;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.Database
{
	public class ReaderInstanceWrapper<TData, TReturn>
	{
		private IDatabaseWrapperService<TData> database;
		private IEnumerable<object> rows;
		private int reference;

		private TReturn result;

		public ReaderInstanceWrapper(IDatabaseWrapperService<TData> database)
		{
			this.database = database;
			(reference, rows) = database.AllRows();
		}

		public ReaderInstanceWrapper<TData, TReturn> WithAction(Func<IEnumerable<object>, TReturn> execute)
		{
			result = execute(rows);
			return this;
		}

		public TReturn Close()
		{
			database.CloseRowReader(reference);
			return result;
		}
	}

	public class ReaderInstanceWrapper<TData>
	{
		private IDatabaseWrapperService<TData> database;
		private IEnumerable<object> rows;
		private int reference;

		public ReaderInstanceWrapper(IDatabaseWrapperService<TData> database)
		{
			this.database = database;
			(reference, rows) = database.AllRows();
		}

		public ReaderInstanceWrapper<TData> WithAction(Action<IEnumerable<object>> execute)
		{
			execute(rows);
			return this;
		}

		public ReaderInstanceWrapper<TData> WithAction<X>(Func<IEnumerable<object>, X> execute)
		{
			var result = execute(rows);
			return this;
		}

		public void Close()
		{
			database.CloseRowReader(reference);
		}
	}
}
