using Microsoft.AspNetCore.Mvc;
using ResortMap.Server.Common;
using ResortMap.Server.Models;
using ResortMap.Server.Services;

namespace ResortMap.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingService bookingHandler) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<MapCoords>> GetAllBookedCabanas()
    {
        return Ok(bookingHandler.GetAllBookedCabanas());
    }
    
    [HttpPost]
    public ActionResult AddBookedCabana([FromBody] BookedCabana cabana)
    {
        return bookingHandler.AddBookedCabana(cabana).ToActionResult();
    }
}
