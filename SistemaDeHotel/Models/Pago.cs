using System;
using System.Collections.Generic;

namespace SistemaDeHotel.Models;

public partial class Pago
{
    public int Id { get; set; }

    public int? ReservaId { get; set; }

    public decimal? Monto { get; set; }

    public DateOnly? Fecha { get; set; }

    public string? FormaPago { get; set; }

    public string? Estado { get; set; }

    public virtual Reserva? Reserva { get; set; }
}
