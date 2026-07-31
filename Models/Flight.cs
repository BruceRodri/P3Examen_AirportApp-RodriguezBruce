using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Flight
{
    [Display(Name = "ID de vuelo")]
    public int FlightId { get; set; }

    [Display(Name = "Número de vuelo")]
    public string Flightno { get; set; } = null!;

    [Display(Name = "Origen")]
    public int From { get; set; }

    [Display(Name = "Destino")]
    public int To { get; set; }

    [Display(Name = "Salida")]
    public DateTime Departure { get; set; }

    [Display(Name = "Llegada")]
    public DateTime Arrival { get; set; }

    [Display(Name = "ID de aerolínea")]
    public int AirlineId { get; set; }

    [Display(Name = "ID de avión")]
    public int AirplaneId { get; set; }

    public virtual Airline Airline { get; set; } = null!;

    public virtual Airplane Airplane { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<FlightLog> FlightLogs { get; set; } = new List<FlightLog>();

    public virtual Flightschedule FlightnoNavigation { get; set; } = null!;

    public virtual Airport FromNavigation { get; set; } = null!;

    public virtual Airport ToNavigation { get; set; } = null!;
}
