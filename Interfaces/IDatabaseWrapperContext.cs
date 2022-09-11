using CoreUtilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data;

namespace CoreUtilities.Interfaces
{
	public interface IDatabaseWrapperContext<TData>
	{
		/// <summary>
		/// Gets the columns that should be present in the database.
		/// </summary>
		/// <returns>A <see cref="KeyValuePair{TKey, TValue}"/> where the key 
		/// is the column name, and the value is a <see cref="ColumnType"/>
		/// enum which describes what the column should store.</returns>
		KeyValuePair<string, ColumnType>[] GetColumns();

		/// <summary>
		/// Gets an array of column names (which should be present in <see cref="GetColumns"/>
		/// which will be indexed for database searching capability.
		/// </summary>
		/// <returns>A <see cref="string[]"/>.</returns>
		string[] GetColumnsToIndex();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemIn"></param>
		/// <returns></returns>
		List<KeyValuePair<string, string>> GetDbCompatibleItem(TData itemIn);

		/// <summary>
		/// Gets a <see cref="DateTime"/> from the <see cref="TData"/> object. If your storage model
		/// does not need a <see cref="DateTime"/>, return something sensible.
		/// </summary>
		/// <param name="itemIn"><see cref="TData"/> object to get a <see cref="DateTime"/> object from.</param>
		/// <returns>A <see cref="DateTime"/> object to be stored in the database.</returns>
		DateTime GetDate(TData itemIn);

		/// <summary>
		/// Gets a unique primary key string from the <see cref="TData"/> object to be stored.
		/// </summary>
		/// <param name="itemIn"><see cref="TData"/> object to get a <see cref="string"/> object from.</param>
		/// <returns>A <see cref="string"/> which is a unique identifier for the item to be stored.</returns>
		string GetPrimaryKey(TData itemIn);

		/// <summary>
		/// Gets a <see cref="TData"/> object from a <see cref="IDataRecord"/> which will be the
		/// database representation of the stored <see cref="TData"/> object.
		/// </summary>
		/// <param name="dbItemIn">A <see cref="IDataRecord"/> object from the database.</param>
		/// <returns>A <see cref="TData"/> object which is the value converted from the database item.</returns>
		TData GetValueFromDbType(IDataRecord dbItemIn);
	}
}
