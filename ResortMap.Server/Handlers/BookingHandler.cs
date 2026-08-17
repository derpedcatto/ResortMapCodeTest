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

        return bookingProvider.TryAddBookedCabana(cabana)
            ? Result.Success()
            : Result.Failure(ErrorCode.CabanaAlreadyBooked);
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

    private Result ValidateBooking(Booking booking)
    {
        var bookingExists = bookingProvider.GetBookings()
            .Any(storedBooking => BookingsMatch(storedBooking, booking));

        return bookingExists
            ? Result.Success()
            : Result.Failure(ErrorCode.BookingNotFound);
    }

    private Result ValidateCabanaCoords(MapCoords coords)
    {
        if (coords.Row == null || coords.Col == null)
        {
            return Result.Failure(ErrorCode.CabanaCoordsInvalid);
        }

        var mapResult = mapHandler.GetMap();
        if (!mapResult.IsSuccess)
        {
            return Result.Failure(mapResult.Error!.Value);
        }

        var grid = mapResult.Value!.Grid;
        var coordsRow = coords.Row.Value;
        var coordsCol = coords.Col.Value;

        if (coordsRow < 0 || coordsRow >= grid.Length)
        {
            return Result.Failure(ErrorCode.CabanaCoordsInvalid);
        }

        var gridRow = grid[coordsCol];

        if (gridRow == null
            || coordsCol < 0 || coordsCol >= gridRow.Length
            || gridRow[coordsCol] != MapSymbol.Cabana)
        {
            return Result.Failure(ErrorCode.CabanaCoordsInvalid);
        }

        return Result.Success();
    }

    private static bool BookingsMatch(Booking storedBooking, Booking requestedBooking)
    {
        if (storedBooking?.Room == null || storedBooking.GuestName == null)
        {
            return false;
        }

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
