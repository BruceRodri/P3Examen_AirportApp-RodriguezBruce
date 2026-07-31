using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Booking
{
    [Display(Name = "ID de reserva")]
    public int BookingId { get; set; }

    [Display(Name = "ID de vuelo")]
    public int FlightId { get; set; }

    [Display(Name = "Asiento")]
    public string? Seat { get; set; }

    [Display(Name = "ID de pasajero")]
    public int PassengerId { get; set; }

    [Display(Name = "Precio")]
    public decimal Price { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Passenger Passenger { get; set; } = null!;
}
