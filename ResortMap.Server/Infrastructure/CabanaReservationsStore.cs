using ResortMap.Server.Models;

namespace ResortMap.Server.Infrastructure;

public interface ICabanaReservationsStore
{
    IReadOnlyList<BookedCabana> GetAll();
    bool TryAdd(BookedCabana bookedCabana);
}

public class CabanaReservationsStore : ICabanaReservationsStore
{
    private readonly Lock _sync = new();
    private readonly List<BookedCabana> _bookedCabanas = [];

    public IReadOnlyList<BookedCabana> GetAll()
    {
        lock (_sync)
        {
            return _bookedCabanas.ToArray();
        }
    }

    public bool TryAdd(BookedCabana cabana)
    {
        lock (_sync)
        {
            if (_bookedCabanas.Any(b => b.Coords.Equals(cabana.Coords)))
            {
                return false;
            }

            _bookedCabanas.Add(cabana);
            return true;
        }
    }
}
