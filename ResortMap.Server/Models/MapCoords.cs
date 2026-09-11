using System.ComponentModel.DataAnnotations;

namespace ResortMap.Server.Models;

public record MapCoords([Required] int? Row, [Required] int? Col);
