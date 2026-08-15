using Microsoft.Extensions.Options;
using ResortMap.Server.Models;
using ResortMap.Server.Options;

namespace ResortMap.Server.Providers;

public interface IMapProvider
{
    string[] GetMapData();
}

public class MapProvider : IMapProvider
{
    private readonly string[] _mapData;

    public MapProvider(IOptions<DataFileOptions> options)
    {
        _mapData = File.ReadAllLines(options.Value.Map);
    }

    public string[] GetMapData() => _mapData;
}
