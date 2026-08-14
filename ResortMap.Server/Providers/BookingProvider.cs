using Microsoft.Extensions.Options;
using ResortMap.Server.Models;
using ResortMap.Server.Options;
using System.Text.Json;

namespace ResortMap.Server.Providers;

public interface IBookingProvider
{
    bool IsBookingAvailable(Booking booking);
}

public class BookingProvider : IBookingProvider
{
    private Booking[] _bookings;

    public BookingProvider(IOptions<DataFileOptions> options)
    {
        string json = File.ReadAllText(options.Value.Bookings);
        _bookings = JsonSerializer.Deserialize<Booking[]>(json) ?? [];
    }

    public bool IsBookingAvailable(Booking booking)
    {
        return true;
    }
}
