using CoreUtilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.Data;

namespace CoreUtilities.Interfaces
{
	public interface IDatabaseWrapperContext<TData>
	{
		KeyValuePair<string, ColumnType>[] GetColumns();
		string[] GetColumnsToIndex();
		List<KeyValuePair<string, string>> GetDbCompatibleItem(TData itemIn);
		DateTime GetDate(TData itemIn);
		string GetPrimaryKey(TData itemIn);
		TData GetValueFromDbType(IDataRecord dbItemIn);
	}
}
