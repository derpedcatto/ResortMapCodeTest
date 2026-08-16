using Microsoft.Extensions.Options;
using ResortMap.Server.Common;
using ResortMap.Server.Models;
using System.Text.Json;

namespace ResortMap.Server.Providers;

public interface IBookingProvider
{
    Result<IReadOnlyList<Booking>> GetBookings();
    IReadOnlyList<BookedCabana> GetBookedCabanas();
    void AddBookedCabana(BookedCabana cabana);
}

public class BookingProvider : IBookingProvider
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private readonly Lock _sync = new();

    private readonly Result<IReadOnlyList<Booking>> _bookings;
    private readonly List<BookedCabana> _bookedCabanas = [];

    public BookingProvider(IOptions<DataFileOptions> options)
    {
        var path = options.Value.Bookings;

        _bookings = ReadBookingsFile(path);
    }

    public Result<IReadOnlyList<Booking>> GetBookings() => _bookings;

    public IReadOnlyList<BookedCabana> GetBookedCabanas()
    {
        lock (_sync)
        {
            return _bookedCabanas.ToArray();
        }
    }

    public void AddBookedCabana(BookedCabana cabana)
    {
        lock (_sync)
        {
            _bookedCabanas.Add(cabana);
        }
    }

    private static Result<IReadOnlyList<Booking>> ReadBookingsFile(string path)
    {
        try
        {
            using var stream = File.OpenRead(path);
            var bookings = JsonSerializer.Deserialize<Booking[]>(stream, _jsonOptions);

            if (bookings == null)
            {
                return Result<IReadOnlyList<Booking>>
                    .Failure(ErrorCode.BookingFileInvalid);
            }
            
            return Result<IReadOnlyList<Booking>>.Success(bookings);
        }
        catch (FileNotFoundException)
        {
            return Result<IReadOnlyList<Booking>>
                .Failure(ErrorCode.BookingFileNotFound);
        }
        catch (UnauthorizedAccessException)
        {
            return Result<IReadOnlyList<Booking>>
                .Failure(ErrorCode.BookingFileNotPermitted);
        }
        catch (IOException)
        {
            return Result<IReadOnlyList<Booking>>
                .Failure(ErrorCode.BookingFileNotFound);
        }
        catch (JsonException)
        {
            return Result<IReadOnlyList<Booking>>
                .Failure(ErrorCode.BookingFileInvalid);
        }
        catch (Exception)
        {
            return Result<IReadOnlyList<Booking>>
                .Failure(ErrorCode.BookingFileNotFound);
        }
    }
}
