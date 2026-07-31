using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class FlightLog
{
    [Display(Name = "ID de bitácora")]
    public long FlightLogId { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime LogDate { get; set; }

    [Display(Name = "Usuario")]
    public string User { get; set; } = null!;

    [Display(Name = "ID de vuelo")]
    public int FlightId { get; set; }

    [Display(Name = "Número de vuelo anterior")]
    public string FlightnoOld { get; set; } = null!;

    [Display(Name = "Número de vuelo nuevo")]
    public string FlightnoNew { get; set; } = null!;

    [Display(Name = "Origen anterior")]
    public short FromOld { get; set; }

    [Display(Name = "Destino anterior")]
    public short ToOld { get; set; }

    [Display(Name = "Origen nuevo")]
    public short FromNew { get; set; }

    [Display(Name = "Destino nuevo")]
    public short ToNew { get; set; }

    [Display(Name = "Salida anterior")]
    public DateTime DepartureOld { get; set; }

    [Display(Name = "Llegada anterior")]
    public DateTime ArrivalOld { get; set; }

    [Display(Name = "Salida nueva")]
    public DateTime DepartureNew { get; set; }

    [Display(Name = "Llegada nueva")]
    public DateTime ArrivalNew { get; set; }

    [Display(Name = "Avión anterior")]
    public int AirplaneIdOld { get; set; }

    [Display(Name = "Avión nuevo")]
    public int AirplaneIdNew { get; set; }

    [Display(Name = "Aerolínea anterior")]
    public short AirlineIdOld { get; set; }

    [Display(Name = "Aerolínea nueva")]
    public short AirlineIdNew { get; set; }

    [Display(Name = "Comentario")]
    public string? Comment { get; set; }

    public virtual Flight Flight { get; set; } = null!;
}
