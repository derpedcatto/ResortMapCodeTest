using Microsoft.Extensions.Options;
using ResortMap.Server.Common;
using ResortMap.Server.Models;
using System.Text.Json;

namespace ResortMap.Server.Infrastructure;

public interface IBookingFileReader
{
    IReadOnlyList<Booking> GetBookings();
}

public class BookingFileReader : IBookingFileReader
{
    private static readonly JsonSerializerOptions _jsonOptions = 
        new(JsonSerializerDefaults.Web);

    private readonly IReadOnlyList<Booking> _bookings;

    public BookingFileReader(IOptions<DataFileOptions> options)
    {
        try
        {
            using var stream = File.OpenRead(options.Value.Bookings);
            _bookings = JsonSerializer.Deserialize<Booking[]>(stream, _jsonOptions)
                ?? throw new InvalidOperationException("Bookings file deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Bookings file is not valid JSON: {options.Value.Bookings}", ex);
        }
    }

    public IReadOnlyList<Booking> GetBookings() => _bookings;
}
