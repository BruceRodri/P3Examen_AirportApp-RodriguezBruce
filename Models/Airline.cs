using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Airline
{
    [Display(Name = "ID de aerolínea")]
    public int AirlineId { get; set; }

    [Display(Name = "Código IATA")]
    public string Iata { get; set; } = null!;

    [Display(Name = "Nombre")]
    public string? Airlinename { get; set; }

    [Display(Name = "Aeropuerto base")]
    public short BaseAirport { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual ICollection<Flightschedule> Flightschedules { get; set; } = new List<Flightschedule>();
}
