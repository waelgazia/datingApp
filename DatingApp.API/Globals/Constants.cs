namespace DatingApp.API.Globals;

public static class Constants
{
    /// <remarks>
    /// Using STRING_COLUMN_TYPE in HasColumnType() without setting HasMaxLength()
    /// will result in a NVARCHAR(1) by entity framework.
    /// </remarks>
    public const string STRING_COLUMN_TYPE = "NVARCHAR";
    public const string DEFAULT_STRING_COLUMN_TYPE = "NVARCHAR(500)";
    public const string MAX_STRING_COLUMN_TYPE = "nvarchar(max)";
    public const string DATETIME_COLUMN_TYPE = "DATETIME2";
}
