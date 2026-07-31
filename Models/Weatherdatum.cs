using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Weatherdatum
{
    [Display(Name = "Fecha")]
    public DateOnly LogDate { get; set; }

    [Display(Name = "Hora")]
    public TimeOnly Time { get; set; }

    [Display(Name = "Estación")]
    public int Station { get; set; }

    [Display(Name = "Temperatura")]
    public decimal Temp { get; set; }

    [Display(Name = "Humedad")]
    public decimal Humidity { get; set; }

    [Display(Name = "Presión")]
    public decimal Airpressure { get; set; }

    [Display(Name = "Viento")]
    public decimal Wind { get; set; }

    [Display(Name = "Dirección del viento")]
    public short Winddirection { get; set; }

    [Display(Name = "Clima")]
    public WeatherCondition? Weather { get; set; }
}
