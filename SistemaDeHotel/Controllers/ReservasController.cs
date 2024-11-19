using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeHotel.Models;

namespace SistemaDeHotel.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ReservasHotelContext _context;

        public ReservasController(ReservasHotelContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var reservasHotelContext = _context.Reservas.Include(r => r.Cliente).Include(r => r.Habitacion);
            return View(await reservasHotelContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Id");
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,HabitacionId,FechaEntrada,FechaSalida,Estado,FormaPago")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe una reserva con el mismo cliente y habitación en las mismas fechas
                var existeReserva = await _context.Reservas
                    .Where(r => r.ClienteId == reserva.ClienteId && r.HabitacionId == reserva.HabitacionId)
                    .Where(r =>
                        (reserva.FechaEntrada >= r.FechaEntrada && reserva.FechaEntrada <= r.FechaSalida) ||
                        (reserva.FechaSalida >= r.FechaEntrada && reserva.FechaSalida <= r.FechaSalida) ||
                        (reserva.FechaEntrada <= r.FechaEntrada && reserva.FechaSalida >= r.FechaSalida)
                    )
                    .FirstOrDefaultAsync();
                if (existeReserva != null)
                {
                    // Si ya existe una reserva, mostrar un mensaje de error
                    ModelState.AddModelError("", "Ya existe una reserva con este cliente y habitación en las fechas seleccionadas.");
                    ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
                    ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Id", reserva.HabitacionId);
                    return View(reserva);
                }
                // Si no existe una reserva duplicada y las fechas son válidas, guardar la nueva reserva
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Id", reserva.HabitacionId);
            return View(reserva);
        }


        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Id", reserva.HabitacionId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,HabitacionId,FechaEntrada,FechaSalida,Estado,FormaPago")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Id", reserva.HabitacionId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
