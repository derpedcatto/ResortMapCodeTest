using NSubstitute;
using ResortMap.Server.Common;
using ResortMap.Server.Infrastructure;
using ResortMap.Server.Models;
using ResortMap.Server.Services;

namespace ResortMap.Tests;

public class BookingServiceTests
{
    private readonly IBookingFileReader _bookingReader = Substitute.For<IBookingFileReader>();
    private readonly ICabanaReservationsStore _cabanaStore = Substitute.For<ICabanaReservationsStore>();
    private readonly IMapService _mapService = Substitute.For<IMapService>();

    private const char Cabana = MapSymbol.Cabana;
    private const char Pool = MapSymbol.Pool;
    private const char Path = MapSymbol.Path;
    private const char Chalet = MapSymbol.Chalet;
    private const char EmptySpace = MapSymbol.EmptySpace;

    private static readonly string[] DefaultGrid =
    [
        $"{Cabana}{EmptySpace}{Pool}",
        $"{Path}{Chalet}{EmptySpace}",
        $"{EmptySpace}{EmptySpace}{Cabana}"
    ];

    private static readonly MapCoords DefaultCoords = new(0, 0);
    private static readonly Booking DefaultBooking = new("101", "Alice Smith");
    private static readonly BookedCabana DefaultBookedCabana = new(DefaultCoords, DefaultBooking);

    private BookingService CreateService() =>
        new(_bookingReader, _cabanaStore, _mapService);

    private void SetupValidDefaultMocks()
    {
        _bookingReader.GetBookings()
            .Returns(Result<IReadOnlyList<Booking>>.Success([DefaultBooking]));

        _mapService.GetMap()
            .Returns(Result<Map>.Success(new Map(DefaultGrid)));

        _cabanaStore.TryAdd(Arg.Any<BookedCabana>()).Returns(true);
    }

    [Fact]
    public void AddBookedCabana_ValidRequest_ReturnsSuccess()
    {
        SetupValidDefaultMocks();

        var result = CreateService().AddBookedCabana(DefaultBookedCabana);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddBookedCabana_BookingDataUnknown_ReturnBookingNotFound()
    {
        SetupValidDefaultMocks();

        var cabana = new BookedCabana(DefaultCoords, new Booking("999", "Unknown Guest"));

        var result = CreateService().AddBookedCabana(cabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BookingNotFound, result.Error);
    }

    [Fact]
    public void AddBookedCabana_CabanaAlreadyBooked_ReturnCabanaAlreadyBooked()
    {
        SetupValidDefaultMocks();

        _cabanaStore.TryAdd(Arg.Any<BookedCabana>()).Returns(false);

        var result = CreateService().AddBookedCabana(DefaultBookedCabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.CabanaAlreadyBooked, result.Error);
    }

    [Fact]
    public void AddBookedCabana_NullCoords_ReturnsInvalidRequest()
    {
        var cabana = new BookedCabana(null!, DefaultBooking);

        var result = CreateService().AddBookedCabana(cabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InvalidBookingRequest, result.Error);
    }

    [Fact]
    public void AddBookedCabana_NullBooking_ReturnsInvalidRequest()
    {
        var cabana = new BookedCabana(DefaultCoords, null!);

        var result = CreateService().AddBookedCabana(cabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InvalidBookingRequest, result.Error);
    }

    [Theory]
    [InlineData(null, "Alice Smith")]
    [InlineData("101", null)]
    [InlineData("101", "")]
    [InlineData("", "Alice Smith")]
    [InlineData("   ", "Alice Smith")]
    [InlineData("101", "   ")]
    public void AddBookedCabana_EmptyOrWhitespaceRoomData_ReturnsInvalidRequest(string? room, string? guestName)
    {
        var cabana = new BookedCabana(DefaultCoords, new Booking(room!, guestName!));
        
        var result = CreateService().AddBookedCabana(cabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InvalidBookingRequest, result.Error);
    }

    [Fact]
    public void AddBookedCabana_CoordsOfNonCabanaTile_ReturnsCabanaCordsInvalid()
    {
        SetupValidDefaultMocks();

        var cabana = new BookedCabana(new MapCoords(0, 1), DefaultBooking);

        var result = CreateService().AddBookedCabana(cabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.CabanaCoordsInvalid, result.Error);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(99, 0)]
    [InlineData(0, 99)]
    public void AddBookedCabana_CoordsOutOfBounds_ReturnsCabanaCoordsInvalid(int row, int col)
    {
        SetupValidDefaultMocks();

        var cabana = new BookedCabana(new MapCoords(row, col), DefaultBooking);

        var result = CreateService().AddBookedCabana(cabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.CabanaCoordsInvalid, result.Error);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData(0, null)]
    [InlineData(null, null)]
    public void AddBookedCabana_NullCoords_ReturnsCabanaCoordsInvalid(int? x, int? y)
    {
        SetupValidDefaultMocks();
        
        var cabana = new BookedCabana(new MapCoords(x, y), DefaultBooking);
        
        var result = CreateService().AddBookedCabana(cabana);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.CabanaCoordsInvalid, result.Error);
    }

    [Fact]
    public void AddBookedCabana_CaseInsensitiveMatching_ReturnsSuccess()
    {
        SetupValidDefaultMocks();

        var cabana = new BookedCabana(DefaultCoords, new Booking("101", "ALICE SMITH"));

        var result = CreateService().AddBookedCabana(cabana);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddBookedCabana_WhitespaceTrimmedMatching_ReturnsSuccess()
    {
        SetupValidDefaultMocks();

        var cabana = new BookedCabana(DefaultCoords, new Booking("    101      ", "      Alice Smith     "));

        var result = CreateService().AddBookedCabana(cabana);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void AddBookedCabana_BookingProviderFail_ReturnsError()
    {
        SetupValidDefaultMocks();

        _bookingReader.GetBookings()
            .Returns(Result<IReadOnlyList<Booking>>
                .Failure(ErrorCode.BookingsFileInvalid));

        var result = CreateService().AddBookedCabana(DefaultBookedCabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BookingsFileInvalid, result.Error);
    }

    [Fact]
    public void AddBookedCabana_MapProviderFail_ReturnsError()
    {
        SetupValidDefaultMocks();

        _mapService.GetMap()
            .Returns(Result<Map>.Failure(ErrorCode.MapFileInvalid));

        var result = CreateService().AddBookedCabana(DefaultBookedCabana);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.MapFileInvalid, result.Error);
    }

    [Fact]
    public void GetAllBookedCabanas_ReturnsAllCoords()
    {
        var secondCoords = new MapCoords(2, 2);

        var booked = new List<BookedCabana>
        {
            DefaultBookedCabana,
            new(secondCoords, DefaultBooking)
        };

        _cabanaStore.GetAll().Returns(booked);

        var result = CreateService().GetAllBookedCabanas();

        Assert.Equal(2, result.Count);
        Assert.Contains(DefaultCoords, result);
        Assert.Contains(secondCoords, result);
    }

    [Fact]
    public void GetAllBookedCabanas_EmptyStore_ReturnsEmptyList()
    {
        _cabanaStore.GetAll().Returns([]);

        var result = CreateService().GetAllBookedCabanas();

        Assert.Empty(result);
    }
}
