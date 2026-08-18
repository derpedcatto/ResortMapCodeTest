namespace ResortMap.Server.Common;

public enum Error
{
    CabanaAlreadyBooked,
    BookingNotFound,
    CabanaCoordsInvalid,
    InvalidBookingRequest,

    MapFileInvalid,
    BookingFileInvalid,

    InvalidRequest,
    InternalError,
}

public sealed record ApiError(string Code, string Message);

public sealed record ErrorInfo(string Message, int HttpStatus);


public static class ErrorCodeExtensions
{
    public static ErrorInfo ToErrorInfo(this Error code) 
        => code switch
    {
        Error.CabanaAlreadyBooked => new("This cabana is already booked.", 409),
        Error.BookingNotFound => new("Booking not found. Inputted data may be invalid.", 404),
        Error.CabanaCoordsInvalid => new("Cabana coordinates are invalid.", 400),
        Error.InvalidBookingRequest => new("Booking request is invalid.", 400),

        Error.MapFileInvalid => new("Resort map is invalid.", 500),
        Error.BookingFileInvalid => new("Booking file is invalid.", 500),

        Error.InvalidRequest => new("Invalid request.", 400),
        Error.InternalError => new("Internal server error.", 500),

        _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Unmapped error code."),
    };

    public static ApiError ToApiError(this Error code)
        => new(code.ToString(), code.ToErrorInfo().Message);
}
