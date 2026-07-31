using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Passenger
{
    [Display(Name = "ID de pasajero")]
    public int PassengerId { get; set; }

    [Display(Name = "Número de pasaporte")]
    public string Passportno { get; set; } = null!;

    [Display(Name = "Nombre")]
    public string Firstname { get; set; } = null!;

    [Display(Name = "Apellido")]
    public string Lastname { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Passengerdetail? Passengerdetail { get; set; }
}
