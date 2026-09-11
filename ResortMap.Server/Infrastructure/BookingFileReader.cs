using Microsoft.Extensions.Options;
using ResortMap.Server.Common;
using ResortMap.Server.Models;
using System.Text.Json;

namespace ResortMap.Server.Infrastructure;

public interface IBookingFileReader
{
    Result<IReadOnlyList<Booking>> GetBookings();
}

public class BookingFileReader : IBookingFileReader
{
    private static readonly JsonSerializerOptions _jsonOptions = 
        new(JsonSerializerDefaults.Web);

    private readonly IReadOnlyList<Booking>? _bookings;
    private readonly bool _loadFailed;

    public BookingFileReader(IOptions<DataFileOptions> options)
    {
        try
        {
            using var stream = File.OpenRead(options.Value.Bookings);
            _bookings = JsonSerializer.Deserialize<Booking[]>(stream, _jsonOptions);
            _loadFailed = _bookings == null;
        } catch (Exception)
        {
            _loadFailed = true;
        }
    }

    public Result<IReadOnlyList<Booking>> GetBookings()
    {
        return _loadFailed
            ? Result<IReadOnlyList<Booking>>.Failure(ErrorCode.BookingsFileInvalid)
            : Result<IReadOnlyList<Booking>>.Success(_bookings!);
    }
}
