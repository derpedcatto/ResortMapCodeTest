using System.ComponentModel.DataAnnotations;

namespace ResortMap.Server.Models;

public record MapCoords([property: Required] int? Row, [property: Required]int? Col);
