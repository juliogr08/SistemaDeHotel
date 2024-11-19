using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeHotel.Models;

public partial class Reserva
{
    public int Id { get; set; }
    public int? ClienteId { get; set; }
    public int? HabitacionId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [ValidateReservaFechas]
    public DateOnly? FechaEntrada { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [ValidateReservaFechas]
    public DateOnly? FechaSalida { get; set; }

    public string? Estado { get; set; }
    public string? FormaPago { get; set; }

    public virtual Cliente? Cliente { get; set; }
    public virtual Habitacione? Habitacion { get; set; }
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}

public class ValidateReservaFechasAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var reserva = (Reserva)validationContext.ObjectInstance;
        if (reserva.FechaEntrada < DateOnly.FromDateTime(DateTime.Today) || reserva.FechaSalida < DateOnly.FromDateTime(DateTime.Today))
        {
            return new ValidationResult("La fecha de entrada y/o salida no pueden ser anteriores a la fecha actual.");
        }
        return ValidationResult.Success;
    }
}

