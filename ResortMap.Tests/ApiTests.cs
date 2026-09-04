using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ResortMap.Server.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ResortMap.Tests;

public class ResortMapFactory : WebApplicationFactory<Program>, IDisposable
{
    private const char Cabana = MapSymbol.Cabana;
    private const char EmptySpace = MapSymbol.EmptySpace;

    private readonly string _tempDir = Path.Combine(
        Path.GetTempPath(), $"resortmap-tests-{Guid.NewGuid()}");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_tempDir);

        var mapPath = Path.Combine(_tempDir, "map.ascii");
        var bookingsPath = Path.Combine(_tempDir, "bookings.json");

        var map =
            $"{Cabana}{EmptySpace}{Cabana}\n" +
            $"{EmptySpace}{EmptySpace}{EmptySpace}\n" + 
            $"{Cabana}{EmptySpace}{Cabana}";

        File.WriteAllText(mapPath, map);

        File.WriteAllText(bookingsPath, JsonSerializer.Serialize(new[]
        {
            new { room = "101", guestName = "Alice Smith" },
            new { room = "102", guestName = "Bob Johnson" }
        }));

        builder.UseSetting("map", mapPath);
        builder.UseSetting("bookings", bookingsPath);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }
}

// read and write tests separated because CabanaReservationsStore is singleton
// and IClassFixture shares state across tests, would break test suite

public class ApiReadTests : IClassFixture<ResortMapFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;

    private const char Cabana = MapSymbol.Cabana;
    private const char EmptySpace = MapSymbol.EmptySpace;

    public ApiReadTests(ResortMapFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetMap_Returns200WithGrid()
    {
        var response = await _httpClient.GetAsync("/api/map", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var map = await response.Content.ReadFromJsonAsync<Map>(JsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(map);
        Assert.Equal(3, map.Grid.Length);
        Assert.Equal($"{Cabana}{EmptySpace}{Cabana}", map.Grid[0]);
    }

    [Fact]
    public async Task GetBookings_Returns200WithEmptyArray()
    {
        var response = await _httpClient.GetAsync("/api/booking", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var coords = await response.Content.ReadFromJsonAsync<MapCoords[]>(JsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(coords);
        Assert.Empty(coords);
    }
}

public class ApiWriteTests
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private static readonly MapCoords DefaultCoords = new(0, 0);
    private static readonly Booking DefaultBooking = new("101", "Alice Smith");
    private static readonly BookedCabana DefaultBookedCabana = new(DefaultCoords, DefaultBooking);

    private sealed class TestServer : IDisposable
    {
        public ResortMapFactory Factory { get; } = new();
        public HttpClient HttpClient { get; }

        public TestServer() => HttpClient = Factory.CreateClient();

        public void Dispose()
        {
            HttpClient.Dispose();
            Factory.Dispose();
        }
    }

    private static TestServer CreateServer() => new();

    [Fact]
    public async Task AddBookedCabana_ValidData_Returns200AndCoords()
    {
        using var server = CreateServer();

        var addResponse = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            DefaultBookedCabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

        var coords = await server.HttpClient.GetFromJsonAsync<MapCoords[]>(
            "/api/booking",
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.NotNull(coords);
        Assert.Single(coords);
        Assert.Equal(0, coords[0].Row);
        Assert.Equal(0, coords[0].Col);
    }

    [Fact]
    public async Task AddBookedCabana_UnknownGuestOrRoom_Returns404()
    {
        using var server = CreateServer();

        var cabana = new BookedCabana(DefaultCoords, new Booking("999", "Unknown"));
        var response = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            cabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddBookedCabana_CoordsOutOfBounds_Returns400()
    {
        using var server = CreateServer();

        var cabana = new BookedCabana(new MapCoords(10, 10), DefaultBooking);
        var response = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            cabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddBookedCabana_CoordsNotCabana_Returns400()
    {
        using var server = CreateServer();

        var cabana = new BookedCabana(new MapCoords(1, 1), DefaultBooking);
        var response = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            cabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddBookedCabana_AlreadyBooked_Returns409()
    {
        using var server = CreateServer();

        var firstResponse = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            DefaultBookedCabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        var secondResponse = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            DefaultBookedCabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task AddBookedCabana_NullGuestName_Returns400()
    {
        using var server = CreateServer();

        var cabana = new BookedCabana(DefaultCoords, new Booking("101", null!));
        var response = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            cabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddBookedCabana_NullRoom_Returns400()
    {
        using var server = CreateServer();

        var cabana = new BookedCabana(DefaultCoords, new Booking(null!, "Alice Smith"));
        var response = await server.HttpClient.PostAsJsonAsync(
            "/api/booking",
            cabana,
            JsonOpts,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("")]
    public async Task AddBookedCabana_NullOrEmptyBody_Returns400(string body)
    {
        using var server = CreateServer();

        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await server.HttpClient.PostAsync(
            "/api/booking",
            content,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
