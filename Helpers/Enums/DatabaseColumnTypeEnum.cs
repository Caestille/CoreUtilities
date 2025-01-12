namespace CoreUtilities.Helpers.Enums;

using System.ComponentModel;

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
