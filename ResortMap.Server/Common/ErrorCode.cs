namespace ResortMap.Server.Common;

public enum ErrorCode
{
    CabanaAlreadyBooked,
    BookingNotFound,
    CabanaCoordsInvalid,
    InvalidBookingRequest,

    MapFileNotFound,
    BookingFileNotFound,
    MapFileInvalid,
    BookingFileInvalid,
    MapFileNotPermitted,
    BookingFileNotPermitted,
}

public static class ErrorCodeExtensions
{
    public static string ToMessage(this ErrorCode code) => code switch
    {
        ErrorCode.CabanaAlreadyBooked => "This cabana is already booked.",
        ErrorCode.BookingNotFound => "Booking not found. Inputted data may be invalid.",
        ErrorCode.CabanaCoordsInvalid => "Cabana coordinates are invalid.",
        ErrorCode.InvalidBookingRequest => "Booking request is invalid.",

        ErrorCode.MapFileNotFound => "Resort map could not be loaded.",
        ErrorCode.BookingFileNotFound => "Booking list could not be loaded.",
        ErrorCode.MapFileInvalid => "Resort map is invalid.",
        ErrorCode.BookingFileInvalid => "Booking file is invalid.",
        ErrorCode.MapFileNotPermitted => "Resort map file is not permitted.",
        ErrorCode.BookingFileNotPermitted => "Booking file is not permitted.",

        _ => throw new NotImplementedException(),
    };
}
