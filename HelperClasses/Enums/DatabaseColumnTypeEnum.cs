using System.ComponentModel;

namespace CoreUtilities.HelperClasses
{
	/// <summary>
	/// Database storage column types which can be stored.
	/// </summary>
	public enum ColumnType
	{
		/// <summary>
		/// Text column.
		/// </summary>
		[Description("TEXT")]
		Text,

		/// <summary>
		/// Integer column.
		/// </summary>
		[Description("INTEGER")]
		Int,
	}
}
