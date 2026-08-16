using ResortMap.Server.Common;
using ResortMap.Server.Models;
using ResortMap.Server.Providers;

namespace ResortMap.Server.Handlers;

public interface IMapHandler
{
    Result<Map> GetMap();
}

public class MapHandler(IMapProvider mapProvider) : IMapHandler
{
    public Result<Map> GetMap()
    {
        var mapData = mapProvider.GetMapData();

        if (mapData == null || mapData.Length == 0)
            return Result<Map>.Failure(ErrorCode.MapFileNotFound);

        var grid = mapData
            .Select(row => row.Trim())
            .Where(row => row.Length > 0)
            .ToArray();

        if (grid.Length == 0 || MapSymbol.HasIllegalSymbols(grid))
            return Result<Map>.Failure(ErrorCode.MapFileInvalid);

        return Result<Map>.Success(new Map(grid));
    }
}
