using Microsoft.AspNetCore.Mvc;
using ResortMap.Server.Common;
using ResortMap.Server.Handlers;
using ResortMap.Server.Models;

namespace ResortMap.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MapController(IMapHandler mapHandler) : ControllerBase
{
    [HttpGet]
    public ActionResult<Map> Get()
    {
        return mapHandler.GetMap().ToActionResult();
    }
}
