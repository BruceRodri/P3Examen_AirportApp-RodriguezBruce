using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P3Examen_AirportApp.Models;

/// <summary>
/// Flughafen DB by Stefan Pröll, Eva Zangerle, Wolfgang Gassler is licensed under CC BY 4.0. To view a copy of this license, visit https://creativecommons.org/licenses/by/4.0
/// </summary>
public partial class Employee
{
    [Display(Name = "ID de empleado")]
    public int EmployeeId { get; set; }

    [Display(Name = "Nombre")]
    public string Firstname { get; set; } = null!;

    [Display(Name = "Apellido")]
    public string Lastname { get; set; } = null!;

    [Display(Name = "Fecha de nacimiento")]
    public DateOnly Birthdate { get; set; }

    [Display(Name = "Sexo")]
    public char? Sex { get; set; }

    [Display(Name = "Calle")]
    public string Street { get; set; } = null!;

    [Display(Name = "Ciudad")]
    public string City { get; set; } = null!;

    [Display(Name = "Código postal")]
    public short Zip { get; set; }

    [Display(Name = "País")]
    public string Country { get; set; } = null!;

    [Display(Name = "Correo electrónico")]
    public string? Emailaddress { get; set; }

    [Display(Name = "Teléfono")]
    public string? Telephoneno { get; set; }

    [Display(Name = "Salario")]
    public decimal? Salary { get; set; }

    [Display(Name = "Usuario")]
    public string? Username { get; set; }

    [Display(Name = "Contraseña")]
    public string? Password { get; set; }

    [Display(Name = "Departamento")]
    public EmployeeDepartment? Department { get; set; }
}
