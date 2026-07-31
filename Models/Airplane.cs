using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Airplane
{
    [Display(Name = "ID de avión")]
    public int AirplaneId { get; set; }

    [Display(Name = "Capacidad")]
    public int Capacity { get; set; }

    [Display(Name = "ID de tipo")]
    public int TypeId { get; set; }

    [Display(Name = "ID de aerolínea")]
    public int AirlineId { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual AirplaneType Type { get; set; } = null!;
}
