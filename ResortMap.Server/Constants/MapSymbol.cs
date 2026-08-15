namespace ResortMap.Server.Constants;

public static class MapSymbol
{
    public const char Cabana = 'W';
    public const char Pool = 'p';
    public const char Path = '#';
    public const char Chalet = 'c';
    public const char EmptySpace = '.';

    public static bool IsValid(char symbol) => symbol is Cabana or Pool or Path or Chalet or EmptySpace;
}
