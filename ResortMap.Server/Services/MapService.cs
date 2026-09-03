using ResortMap.Server.Common;
using ResortMap.Server.Infrastructure;
using ResortMap.Server.Models;

namespace ResortMap.Server.Services;

public interface IMapHandler
{
    Result<Map> GetMap();
}

public class MapService(IMapFileReader mapProvider) : IMapHandler
{
    public Result<Map> GetMap()
    {
        var mapData = mapProvider.GetMapData();

        if (mapData.Length == 0)
            return Result<Map>.Failure(ErrorCode.MapFileInvalid);

        var grid = mapData
            .Select(row => row.Trim())
            .Where(row => row.Length > 0)
            .ToArray();

        if (grid.Length == 0 || MapSymbol.HasIllegalSymbols(grid))
            return Result<Map>.Failure(ErrorCode.MapFileInvalid);

        if (grid.Any(row => row.Length != grid[0].Length))
            return Result<Map>.Failure(ErrorCode.MapFileInvalid);

        return Result<Map>.Success(new Map(grid));
    }
}
