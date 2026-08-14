namespace ResortMap.Server.Constants;

// use this class only for validating loaded ascii file?
public static class MapSymbol
{
    public const char Cabana = 'W';
    public const char Pool = 'p';
    public const char Path = '#';
    public const char Chalet = 'c';
    public const char EmptySpace = '.';

    // Get rid of this implementation and have a function to check a map for illegal symbols?
    public static bool IsValid(char symbol) => symbol is Cabana or Pool or Path or Chalet or EmptySpace;
}
