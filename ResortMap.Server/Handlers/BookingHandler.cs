using ResortMap.Server.Constants;
using ResortMap.Server.Models;
using ResortMap.Server.Providers;

namespace ResortMap.Server.Handlers;

public interface IBookingHandler
{
    List<MapCoords> GetAllBookedCabanas();
    void AddBookedCabana(BookedCabana cabana);
}

public class BookingHandler(IBookingProvider bookingProvider, IMapHandler mapHandler) : IBookingHandler
{
    public List<MapCoords> GetAllBookedCabanas()
    {
        return bookingProvider.GetBookedCabanas()
            .Select(bc => bc.Coords)
            .ToList();
    }

    public void AddBookedCabana(BookedCabana cabana)
    {
        if (!IsKabanaBooked(cabana.Coords))
        {
            if (IsBookingValid(cabana.Booking))
            {
                if (IsKabanaCoordsValid(cabana.Coords))
                {
                    bookingProvider.AddBookedCabana(cabana);
                }
            }
        }
    }

    bool IsBookingValid(Booking booking)
    {
        return bookingProvider.GetBookings()
            .Any(b => b.Equals(booking));
    }

    bool IsKabanaBooked(MapCoords coords)
    {
        return bookingProvider.GetBookedCabanas()
            .Any(bc => bc.Coords.Equals(coords));
    }

    bool IsKabanaCoordsValid(MapCoords coords)
    {
        var grid = mapHandler.GetMap().Grid;

        if (coords.Row < 0 || coords.Row >= grid.Length)
            return false;

        var row = grid[coords.Row];
        if (row == null)
            return false;

        if (coords.Col < 0 || coords.Col >= row.Length)
            return false;

        return row[coords.Col] == MapSymbol.Cabana;
    }
}
