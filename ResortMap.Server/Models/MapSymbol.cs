namespace ResortMap.Server.Models;

public static class MapSymbol
{
    public const char Cabana = 'W';
    public const char Pool = 'p';
    public const char Path = '#';
    public const char Chalet = 'c';
    public const char EmptySpace = '.';

    public static bool IsValid(char symbol) => symbol is Cabana or Pool or Path or Chalet or EmptySpace;

    public static bool HasIllegalSymbols(string[] map)
    {
        foreach (var row in map)
        {
            if (string.IsNullOrEmpty(row)|| row.Any(s => !MapSymbol.IsValid(s)))
                return true;
        }

        return false;
    }
}
