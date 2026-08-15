using Microsoft.Extensions.Options;
using ResortMap.Server.Models;
using ResortMap.Server.Options;
using System.Text.Json;

namespace ResortMap.Server.Providers;

public interface IBookingProvider
{
    IReadOnlyList<Booking> GetBookings();
    IReadOnlyList<BookedCabana> GetBookedCabanas();
    void AddBookedCabana(BookedCabana cabana);
}

public class BookingProvider : IBookingProvider
{
    private readonly Booking[] _bookings;
    private readonly List<BookedCabana> _bookedCabanas = [];

    public BookingProvider(IOptions<DataFileOptions> options)
    {
        string json = File.ReadAllText(options.Value.Bookings);
        _bookings = JsonSerializer.Deserialize<Booking[]>(json) ?? [];
    }

    public IReadOnlyList<Booking> GetBookings() => _bookings;
    public IReadOnlyList<BookedCabana> GetBookedCabanas() => _bookedCabanas;

    public void AddBookedCabana(BookedCabana cabana)
    {
        _bookedCabanas.Add(cabana);
    }
}
