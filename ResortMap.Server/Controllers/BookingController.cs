using Microsoft.AspNetCore.Mvc;
using ResortMap.Server.Common;
using ResortMap.Server.Handlers;
using ResortMap.Server.Models;

namespace ResortMap.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingHandler bookingHandler) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<MapCoords>> GetAllBookedCabanas()
    {
        return Ok(bookingHandler.GetAllBookedCabanas());
    }
    
    [HttpPost]
    public ActionResult AddBookedCabana([FromBody] BookedCabana cabana)
    {
        var result = bookingHandler.AddBookedCabana(cabana);

        if (result.IsSuccess)
        {
            return Ok(new { });
        }

        var errorCode = result.Error!.Value;
        var error = errorCode.ToApiError();

        if (errorCode == ErrorCode.MapFileInvalid ||
            errorCode == ErrorCode.BookingFileInvalid)
        {
            return StatusCode(500, error);
        }

        return BadRequest(error);

    }
}
