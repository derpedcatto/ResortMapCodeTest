using Microsoft.AspNetCore.Mvc;
using ResortMap.Server.Handlers;
using ResortMap.Server.Models;

namespace ResortMap.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BookingController(IBookingHandler bookingHandler) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<MapCoords>> GetAllBookedCabanas()
    {
        return Ok(bookingHandler.GetAllBookedCabanas());
    }
    
    [HttpPost]
    public ActionResult AddBookedCabana([FromBody] BookedCabana cabana)
    {
        bookingHandler.AddBookedCabana(cabana);
        return Ok();
    }
}
