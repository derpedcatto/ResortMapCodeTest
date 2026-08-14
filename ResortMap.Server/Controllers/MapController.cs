using Microsoft.AspNetCore.Mvc;
using ResortMap.Server.Models;
using ResortMap.Server.Providers;

namespace ResortMap.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController(IMapProvider mapProvider) : ControllerBase
    {
        [HttpGet]
        public ActionResult<Map> Get() => Ok(mapProvider.GetMap().Grid);
    }
}
