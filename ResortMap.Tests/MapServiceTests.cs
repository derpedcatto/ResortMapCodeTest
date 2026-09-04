using NSubstitute;
using ResortMap.Server.Common;
using ResortMap.Server.Infrastructure;
using ResortMap.Server.Models;
using ResortMap.Server.Services;

namespace ResortMap.Tests;

public class MapServiceTests
{
    private const char Cabana = MapSymbol.Cabana;
    private const char Pool = MapSymbol.Pool;
    private const char Path = MapSymbol.Path;
    private const char Chalet = MapSymbol.Chalet;
    private const char EmptySpace = MapSymbol.EmptySpace;

    private static MapService CreateService(string[] mapGrid)
    {
        var fakeReader = Substitute.For<IMapFileReader>();

        fakeReader.GetMapData().Returns(mapGrid);

        return new MapService(fakeReader);
    }

    private static void AssertMapFileInvalid(params string[] grid)
    {
        var result = CreateService(grid).GetMap();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.MapFileInvalid, result.Error);
    }

    [Fact]
    public void GetMap_ValidGrid_ReturnsSuccessWithGrid()
    {
        var grid = new[]
        { 
            $"{Cabana}{EmptySpace}{Pool}",
            $"{Path}{Chalet}{EmptySpace}",
            $"{EmptySpace}{EmptySpace}{Cabana}"
        };

        var result = CreateService(grid).GetMap();

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.Grid.Length);
        Assert.Equal($"{Cabana}{EmptySpace}{Pool}", result.Value.Grid[0]);
    }

    [Fact]
    public void GetMap_IllegalSymbols_ReturnsMapFileInvalid()
    {
        AssertMapFileInvalid($"{Cabana}{EmptySpace}X");
    }

    [Fact]
    public void GetMap_InconsitentRowLengths_ReturnsMapFileInvalid()
    {
        AssertMapFileInvalid(
            $"{Cabana}{EmptySpace}{Pool}",
            $"{Path}{Chalet}",
            $"{EmptySpace}{EmptySpace}{Cabana}");
    }

    [Fact]
    public void GetMap_EmptyData_ReturnsMapFileInvalid()
    {
        AssertMapFileInvalid();
    }

    [Fact]
    public void GetMap_AllWhitespaceLines_ReturnsMapFileInvalid()
    {
        AssertMapFileInvalid("   ", "   ", "  ");
    }
}
