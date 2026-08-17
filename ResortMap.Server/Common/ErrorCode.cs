namespace ResortMap.Server.Common;

public enum ErrorCode
{
    CabanaAlreadyBooked,
    BookingNotFound,
    CabanaCoordsInvalid,
    InvalidBookingRequest,

    MapFileInvalid,
    BookingFileInvalid,
}

public static class ErrorCodeExtensions
{
    public static string ToMessage(this ErrorCode code) => code switch
    {
        ErrorCode.CabanaAlreadyBooked => "This cabana is already booked.",
        ErrorCode.BookingNotFound => "Booking not found. Inputted data may be invalid.",
        ErrorCode.CabanaCoordsInvalid => "Cabana coordinates are invalid.",
        ErrorCode.InvalidBookingRequest => "Booking request is invalid.",

        ErrorCode.MapFileInvalid => "Resort map is invalid.",
        ErrorCode.BookingFileInvalid => "Booking file is invalid.",

        _ => throw new ArgumentOutOfRangeException(nameof(code), code, "Unmapped error code."),
    };
}
