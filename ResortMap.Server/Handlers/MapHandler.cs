using ResortMap.Server.Models;
using ResortMap.Server.Providers;

namespace ResortMap.Server.Handlers;

public interface IMapHandler
{
    Map GetMap();
}

public class MapHandler(IMapProvider mapProvider) : IMapHandler
{
    public Map GetMap() => Parse(mapProvider.GetMapData());

    private static Map Parse(string[] lines)
    {
        var grid = lines
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToArray();

        return new Map(grid);
    }
}
