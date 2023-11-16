using CoreUtilities.Interfaces.Database;
using System;
using System.Collections.Generic;

namespace CoreUtilities.HelperClasses.Database
{
	/// <summary>
	/// Wrapper class for enabling builder pattern usage of read operations into a 
	/// <see cref="IDatabaseWrapperService{TData}"/> instance.
	/// This is a wrapper capable of
	/// reading data of type <see cref="TData"/> and returning a result from the <see cref="Close"/> method of type
	/// <see cref="TReturn"/>.
	/// </summary>
	/// <typeparam name="TData">The data type to be stored.</typeparam>
	/// <typeparam name="TReturn">The data type to be returned from the read operation, if required.</typeparam>
	public class ReaderInstanceWrapper<TData, TReturn>
	{
		private readonly IDatabaseWrapperService<TData> database;
		private readonly IEnumerable<object> rows;
		private readonly int reference;

		private TReturn result;

		/// <summary>
		/// Constructor for the <see cref="ReaderInstanceWrapper{TData, TReturn}"/>. 
		/// </summary>
		/// <param name="database">The <see cref="IDatabaseWrapperService{TData}"/> this wrapper should interact 
		/// with.</param>
		public ReaderInstanceWrapper(IDatabaseWrapperService<TData> database)
		{
			this.database = database;
			(reference, rows) = database.AllRows();
		}

		/// <summary>
		/// Adds an action to be executed while providing access to the rows in the database, meaning the action can
		/// be executed using those rows. This version allows the action to return a value when the reader is closed
		/// using <see cref="Close"/>. Should be called after something provides an instance of
		/// <see cref="ReaderInstanceWrapper{TData, TReturn}"/>.
		/// </summary>
		/// <param name="execute">The action to execute</param>
		/// <returns>This instance of the class, allowing the builder pattern to continue.</returns>
		public ReaderInstanceWrapper<TData, TReturn> WithAction(Func<IEnumerable<object>, TReturn> execute)
		{
			result = execute(rows);
			return this;
		}

		/// <summary>
		/// Closes the reader and returns the return value from the action provided in
		/// <see cref="WithAction(Func{IEnumerable{object}, TReturn})"/>. 
		/// Should be called after something interacts with the rows in
		/// <see cref="WithAction(Func{IEnumerable{object}, TReturn})"/>. 
		/// </summary>
		/// <returns>A value of type <see cref="TReturn"/>.</returns>
		public TReturn Close()
		{
			database.CloseRowReader(reference);
			return result;
		}
	}

	/// <summary>
	/// Wrapper class for enabling builder pattern usage of read operations into a 
	/// <see cref="IDatabaseWrapperService{TData}"/> instance.
	/// This is a wrapper does not return a value
	/// from the <see cref="Close"/> method unlike the <see cref="ReaderInstanceWrapper{TData, TReturn}"/> class.
	/// </summary>
	/// <typeparam name="TData">The data type to be stored.</typeparam>
	public class ReaderInstanceWrapper<TData>
	{
		private readonly IDatabaseWrapperService<TData> database;
		private readonly IEnumerable<object> rows;
		private readonly int reference;

		/// <summary>
		/// Constructor for the <see cref="ReaderInstanceWrapper{TData}"/>.
		/// </summary>
		/// <param name="database">The <see cref="IDatabaseWrapperService{TData}"/> this wrapper should interact 
		/// with.</param>
		public ReaderInstanceWrapper(IDatabaseWrapperService<TData> database)
		{
			this.database = database;
			(reference, rows) = database.AllRows();
		}

		/// <summary>
		/// Adds an action to be executed while providing access to the rows in the database, meaning the action can
		/// be executed using those rows. Should be called after something provides an instance of
		/// <see cref="ReaderInstanceWrapper{TData}"/>.
		/// </summary>
		/// <param name="execute">The action to execute</param>
		/// <returns>This instance of the class, allowing the builder pattern to continue.</returns>
		public ReaderInstanceWrapper<TData> WithAction(Action<IEnumerable<object>> execute)
		{
			execute(rows);
			return this;
		}

		/// <summary>
		/// Closes the reader. Should be called after something interacts with the rows in
		/// <see cref="WithAction(Action{IEnumerable{object}})"/>.
		/// </summary>
		public void Close()
		{
			database.CloseRowReader(reference);
		}
	}
}
