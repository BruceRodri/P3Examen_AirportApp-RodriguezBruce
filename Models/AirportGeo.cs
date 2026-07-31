using System;
using System.Collections.Generic;
using NpgsqlTypes;

using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class AirportGeo
{
    [Display(Name = "ID de aeropuerto")]
    public int AirportId { get; set; }

    [Display(Name = "Nombre")]
    public string Name { get; set; } = null!;

    [Display(Name = "Ciudad")]
    public string? City { get; set; }

    [Display(Name = "País")]
    public string? Country { get; set; }

    [Display(Name = "Latitud")]
    public decimal Latitude { get; set; }

    [Display(Name = "Longitud")]
    public decimal Longitude { get; set; }

    [Display(Name = "Ubicación")]
    public NpgsqlPoint Geolocation { get; set; }

    public virtual Airport Airport { get; set; } = null!;
}
