using Microsoft.Extensions.Options;
using ResortMap.Server.Common;

namespace ResortMap.Server.Providers;

public interface IMapProvider
{
    string[]? GetMapData();
}

public class MapProvider : IMapProvider
{
    private readonly string[]? _mapData;

    public MapProvider(IOptions<DataFileOptions> options)
    {
        var path = options.Value.Map;
        _mapData = File.Exists(path) ? File.ReadAllLines(path) : null;
    }

    public string[]? GetMapData() => _mapData;
}
