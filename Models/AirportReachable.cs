using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class AirportReachable
{
    [Display(Name = "ID de aeropuerto")]
    public int AirportId { get; set; }

    [Display(Name = "Escalas")]
    public int? Hops { get; set; }

    public virtual Airport Airport { get; set; } = null!;
}
