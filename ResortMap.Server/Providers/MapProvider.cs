using Microsoft.Extensions.Options;
using ResortMap.Server.Models;
using ResortMap.Server.Options;

namespace ResortMap.Server.Providers;

public interface IMapProvider
{
    Map GetMap();
}

public class MapProvider : IMapProvider
{
    private readonly Map _map;

    public MapProvider(IOptions<DataFileOptions> options)
    {
        _map = Parse(File.ReadAllLines(options.Value.Map));
    }

    public Map GetMap() => _map;

    private static Map Parse(string[] lines)
    {
        var grid = lines
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToArray();

        return new Map(grid);
    }
}
