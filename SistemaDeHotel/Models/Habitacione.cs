using System;
using System.Collections.Generic;

namespace SistemaDeHotel.Models;

public partial class Habitacione
{
    public int Id { get; set; }

    public string? Tipo { get; set; }

    public int? Capacidad { get; set; }

    public decimal? Precio { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
