using Microsoft.Extensions.Options;
using ResortMap.Server.Common;

namespace ResortMap.Server.Infrastructure;

public interface IMapFileReader
{
    string[] GetMapData();
}

public class MapFileReader : IMapFileReader
{
    private readonly string[] _mapData;

    public MapFileReader(IOptions<DataFileOptions> options)
    {
        _mapData = File.ReadAllLines(options.Value.Map);
    }

    public string[] GetMapData() => _mapData;
}
