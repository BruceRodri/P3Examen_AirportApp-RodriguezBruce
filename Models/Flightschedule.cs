using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Flightschedule
{
    [Display(Name = "Número de vuelo")]
    public string Flightno { get; set; } = null!;

    [Display(Name = "Origen")]
    public int From { get; set; }

    [Display(Name = "Destino")]
    public int To { get; set; }

    [Display(Name = "Salida")]
    public TimeOnly Departure { get; set; }

    [Display(Name = "Llegada")]
    public TimeOnly Arrival { get; set; }

    [Display(Name = "ID de aerolínea")]
    public int AirlineId { get; set; }

    [Display(Name = "Lunes")]
    public bool? Monday { get; set; }

    [Display(Name = "Martes")]
    public bool? Tuesday { get; set; }

    [Display(Name = "Miércoles")]
    public bool? Wednesday { get; set; }

    [Display(Name = "Jueves")]
    public bool? Thursday { get; set; }

    [Display(Name = "Viernes")]
    public bool? Friday { get; set; }

    [Display(Name = "Sábado")]
    public bool? Saturday { get; set; }

    [Display(Name = "Domingo")]
    public bool? Sunday { get; set; }

    public virtual Airline Airline { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Airport FromNavigation { get; set; } = null!;

    public virtual Airport ToNavigation { get; set; } = null!;
}
