namespace ResortMap.Server.Options;

public sealed class DataFileOptions
{
    public string Map { get; set; } = "map.ascii";
    public string Bookings { get; set; } = "bookings.json";
}
