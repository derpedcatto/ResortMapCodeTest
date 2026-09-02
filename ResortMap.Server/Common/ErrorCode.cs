using Microsoft.AspNetCore.Mvc;

namespace ResortMap.Server.Common;

public enum ErrorCode
{
    CabanaAlreadyBooked,
    BookingNotFound,
    CabanaCoordsInvalid,
    InvalidBookingRequest,

    MapFileInvalid,
    BookingsFileInvalid,

    InternalError,
}

public static class ErrorCodeExtensions
{
    public static ProblemDetails ToProblemDetails(this ErrorCode code) => code switch
    {
        ErrorCode.CabanaAlreadyBooked => new()
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Cabana already booked",
            Detail = "This cabana is already booked.",
        },
        ErrorCode.BookingNotFound => new()
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Booking not found",
            Detail = "Booking not found. Inputted data may be invalid.",
        },
        ErrorCode.CabanaCoordsInvalid => new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid cabana coordinates",
            Detail = "Cabana coordinates are invalid.",
        },
        ErrorCode.InvalidBookingRequest => new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid booking request",
            Detail = "Booking request is invalid. Input may be incorrect.",
        },
        ErrorCode.MapFileInvalid => new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error",
            Detail = "Resort map is invalid.",
        },
        ErrorCode.BookingsFileInvalid => new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error",
            Detail = "Bookings file is invalid.",
        },
        ErrorCode.InternalError => new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error",
            Detail = "An unexpected error occurred.",
        },
        _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Unmapped error code."),
    };
}
