namespace InventoryDashboard.Application.Mappings;

internal static class DateTimeHelper
{
    /// <summary>
    /// PostgreSQL timestamptz requires UTC. JSON date strings deserialize as Unspecified.
    /// </summary>
    public static DateTime ToUtcDate(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value.Date,
            DateTimeKind.Local => value.ToUniversalTime().Date,
            _ => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc)
        };
    }

    public static DateTime ToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
