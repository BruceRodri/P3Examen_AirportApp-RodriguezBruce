using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class AirplaneType
{
    [Display(Name = "ID de tipo")]
    public int TypeId { get; set; }

    [Display(Name = "Identificador")]
    public string? Identifier { get; set; }

    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    public virtual ICollection<Airplane> Airplanes { get; set; } = new List<Airplane>();
}
