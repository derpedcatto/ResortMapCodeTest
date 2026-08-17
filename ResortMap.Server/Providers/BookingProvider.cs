using Microsoft.Extensions.Options;
using ResortMap.Server.Common;
using ResortMap.Server.Models;
using System.Text.Json;

namespace ResortMap.Server.Providers;

public interface IBookingProvider
{
    IReadOnlyList<Booking> GetBookings();
    IReadOnlyList<BookedCabana> GetBookedCabanas();
    bool TryAddBookedCabana(BookedCabana cabana);
}

public class BookingProvider : IBookingProvider
{
    private static readonly JsonSerializerOptions _jsonOptions = 
        new(JsonSerializerDefaults.Web);
    private readonly Lock _sync = new();

    private readonly IReadOnlyList<Booking> _bookings;
    private readonly List<BookedCabana> _bookedCabanas = [];

    public BookingProvider(IOptions<DataFileOptions> options)
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

    public IReadOnlyList<BookedCabana> GetBookedCabanas()
    {
        lock (_sync)
        {
            return _bookedCabanas.ToArray();
        }
    }

    public bool TryAddBookedCabana(BookedCabana cabana)
    {
        lock (_sync)
        {
            if (_bookedCabanas.Any(bc => bc.Coords.Equals(cabana.Coords)))
                return false;

            _bookedCabanas.Add(cabana);
            return true;
        }
    }
}
