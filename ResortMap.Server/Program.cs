using ResortMap.Server.Options;
using ResortMap.Server.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<DataFileOptions>()
    .Configure<IConfiguration>((opts, config) =>
    {
        opts.Map = config["map"] ?? opts.Map;
        opts.Bookings = config["bookings"] ?? opts.Bookings;
    })
    .Validate(opts => File.Exists(opts.Map), "Map file not found.")
    .Validate(opts => File.Exists(opts.Bookings), "Bookings file not found.")
    .ValidateOnStart();

builder.Services.AddSingleton<IMapProvider, MapProvider>();
builder.Services.AddSingleton<IBookingProvider, BookingProvider>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
