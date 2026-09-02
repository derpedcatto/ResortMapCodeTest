using Microsoft.Extensions.Options;
using ResortMap.Server.Common;
using ResortMap.Server.Infrastructure;
using ResortMap.Server.Services;

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

builder.Services.AddSingleton<IMapFileReader, MapFileReader>();
builder.Services.AddSingleton<IBookingFileReader, BookingFileReader>();
builder.Services.AddSingleton<ICabanaReservationsStore, CabanaReservationsStore>();
builder.Services.AddScoped<IMapHandler, MapService>();
builder.Services.AddScoped<IBookingHandler, BookingService>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (OptionsValidationException ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    logger?.LogCritical(ex, "Application startup failed");
    Console.Error.WriteLine($"Startup failed: {ex.Message}");

    Environment.ExitCode = 1;
}
