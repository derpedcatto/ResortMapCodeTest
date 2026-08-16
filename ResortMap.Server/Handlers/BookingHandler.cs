using ResortMap.Server.Common;
using ResortMap.Server.Models;
using ResortMap.Server.Providers;

namespace ResortMap.Server.Handlers;

public interface IBookingHandler
{
    IReadOnlyList<MapCoords> GetAllBookedCabanas();
    Result AddBookedCabana(BookedCabana cabana);
}

public class BookingHandler(IBookingProvider bookingProvider, IMapHandler mapHandler)
    : IBookingHandler
{
    public IReadOnlyList<MapCoords> GetAllBookedCabanas()
    {
        return bookingProvider.GetBookedCabanas()
            .Select(bc => bc.Coords)
            .ToList();
    }

    public Result AddBookedCabana(BookedCabana cabana)
    {
        if (!IsRequestValid(cabana))
        {
            return Result.Failure(ErrorCode.InvalidBookingRequest);
        }

        var coordsResult = ValidateCabanaCoords(cabana.Coords);
        if (!coordsResult.IsSuccess)
        {
            return coordsResult;
        }

        var bookingResult = ValidateBooking(cabana.Booking);
        if (!bookingResult.IsSuccess)
        {
            return bookingResult;
        }

        if (IsCabanaBooked(cabana.Coords))
        {
            return Result.Failure(ErrorCode.CabanaAlreadyBooked);
        }

        bookingProvider.AddBookedCabana(cabana);
        return Result.Success();
    }

    private static bool IsRequestValid(BookedCabana? cabana)
    {
        if (cabana?.Coords == null || cabana.Booking == null)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(cabana.Booking.Room)
            && !string.IsNullOrWhiteSpace(cabana.Booking.GuestName);
    }

    private bool IsCabanaBooked(MapCoords coords)
    {
        return bookingProvider.GetBookedCabanas()
            .Any(bookedCabana => bookedCabana.Coords.Equals(coords));
    }

    private Result ValidateBooking(Booking booking)
    {
        var bookingsResult = bookingProvider.GetBookings();

        if (!bookingsResult.IsSuccess)
        {
            return Result.Failure(bookingsResult.Error!.Value);
        }

        var bookingExists = bookingsResult.Value!
            .Any(storedBooking => BookingsMatch(storedBooking, booking));

        if (!bookingExists)
        {
            return Result.Failure(ErrorCode.BookingNotFound);
        }

        return Result.Success();
    }

    private Result ValidateCabanaCoords(MapCoords coords)
    {
        var mapResult = mapHandler.GetMap();

        if (!mapResult.IsSuccess)
        {
            return Result.Failure(mapResult.Error!.Value);
        }

        var grid = mapResult.Value!.Grid;

        if (coords.Row < 0 || coords.Row >= grid.Length)
        {
            return Result.Failure(ErrorCode.CabanaCoordsInvalid);
        }

        var row = grid[coords.Row];

        if (row == null
            || (coords.Col < 0 || coords.Col >= row.Length)
            || row[coords.Col] != MapSymbol.Cabana)
        {
            return Result.Failure(ErrorCode.CabanaCoordsInvalid);
        }

        if (grid[coords.Row][coords.Col] != MapSymbol.Cabana)
        {
            return Result.Failure(ErrorCode.CabanaCoordsInvalid);
        }

        return Result.Success();
    }

    private static bool BookingsMatch(Booking storedBooking, Booking requestedBooking)
    {
        var roomsMatch = string.Equals(
            storedBooking.Room.Trim(),
            requestedBooking.Room.Trim(),
            StringComparison.OrdinalIgnoreCase);

        var guestsMatch = string.Equals(
            storedBooking.GuestName.Trim(),
            requestedBooking.GuestName.Trim(),
            StringComparison.OrdinalIgnoreCase);

        return roomsMatch && guestsMatch;
    }
}
