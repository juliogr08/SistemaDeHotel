using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeHotel.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace SistemaDeHotel.Controllers
{
    public class PagosController : Controller
    {
        private readonly ReservasHotelContext _context;

        public PagosController(ReservasHotelContext context)
        {
            _context = context;
        }
        // GET: Pagos/GenerateInvoice/5
        public async Task<IActionResult> GenerateInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtener el pago y los detalles de la reserva asociados
            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Cliente) // Si tienes un modelo Cliente relacionado con la Reserva
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pago == null)
            {
                return NotFound();
            }

            // Generar el contenido de la factura (por ejemplo, un archivo PDF)
            var invoiceDetails = $"Factura ID: {pago.Id}\n" +
                                 $"Monto: {pago.Monto}\n" +
                                 $"Fecha: {pago.Fecha}\n" +
                                 $"Forma de Pago: {pago.FormaPago}\n" +
                                 $"Estado: {pago.Estado}\n" +
                                 $"Reserva ID: {pago.Reserva.Id}\n";

            // Aquí se puede usar una librería para generar el archivo PDF, como iTextSharp o QuestPDF.
            byte[] pdfBytes = System.Text.Encoding.UTF8.GetBytes(invoiceDetails); // Simulación del PDF

            // Descargar el archivo como respuesta
            return File(pdfBytes, "application/pdf", $"Factura_Pago_{pago.Id}.pdf");
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {
            var reservasHotelContext = _context.Pagos.Include(p => p.Reserva);
            return View(await reservasHotelContext.ToListAsync());
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservaId,Monto,Fecha,FormaPago,Estado")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un pago con la misma reserva y monto
                var existePago = await _context.Pagos
                    .Where(p => p.ReservaId == pago.ReservaId && p.Monto == pago.Monto && p.Fecha == pago.Fecha)
                    .FirstOrDefaultAsync();

                if (existePago != null)
                {
                    // Si ya existe un pago duplicado, mostrar un mensaje de error
                    ModelState.AddModelError("", "Ya existe un pago con los mismos detalles para esta reserva.");
                    ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
                    return View(pago);
                }

                // Si no existe un pago duplicado, guardar el nuevo pago
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
            return View(pago);
        }


        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
            return View(pago);
        }

        // POST: Pagos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Monto,Fecha,FormaPago,Estado")] Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.Id))
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
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }


        public async Task<IActionResult> DownloadPaymentsPDF()
        {
            var pagos = await _context.Pagos
                .Include(p => p.Reserva)
                .ToListAsync();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Crear documento PDF
                Document document = new Document(PageSize.A4, 50, 50, 80, 80);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                // Asignar evento para la marca de agua
                writer.PageEvent = new WatermarkEvent("HOTEL JULIO");

                document.Open();

                // Añadir logo
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("https://th.bing.com/th/id/R.1bcf1801775af2029e3b4ad32495a61f?rik=cHzOysa3OxVKPQ&riu=http%3a%2f%2fpluspng.com%2fimg-png%2fpng-lobo-lobo-3600.png&ehk=nel0WpvKTrcctai3M9bgTmD7r5HQrTj4xvUt1DmOe98%3d&risl=&pid=ImgRaw&r=0");
                logo.ScaleToFit(100, 100);
                logo.Alignment = Element.ALIGN_LEFT;
                document.Add(logo);

                // Añadir título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.BLACK);
                Paragraph title = new Paragraph("Factura de Pagos", titleFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Crear tabla
                PdfPTable table = new PdfPTable(5); // 5 columnas
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 2, 2, 2, 2 }); // Ancho de columnas

                // Encabezados de la tabla
                string[] headers = { "Monto", "Fecha", "Forma de Pago", "Estado", "Reserva ID" };
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);

                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = new BaseColor(55, 71, 79), // Color del encabezado
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 10
                    };
                    table.AddCell(cell);
                }

                // Añadir filas de la tabla
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                foreach (var pago in pagos)
                {
                    // Formatear el monto
                    string montoFormateado = pago.Monto.HasValue ? pago.Monto.Value.ToString("C") : "N/A";

                    // Formatear la fecha
                    string fechaFormateada = pago.Fecha.HasValue ? pago.Fecha.Value.ToString("d") : "N/A";

                    table.AddCell(new PdfPCell(new Phrase(montoFormateado, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(fechaFormateada, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(pago.FormaPago, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(pago.Estado, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(pago.Reserva.Id.ToString(), cellFont)));
                }

                // Añadir el número de la factura y la fecha
                Font infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                Paragraph invoiceInfo = new Paragraph($"Factura #: {DateTime.Now.ToString("yyyyMMdd")}", infoFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 20,
                    SpacingAfter = 10
                };
                document.Add(invoiceInfo);

                document.Add(table);
                document.Close();

                // Devolver el archivo PDF
                byte[] pdfBytes = memoryStream.ToArray();
                return File(pdfBytes, "application/pdf", "Factura_Pagos.pdf");
            }
        }

        // Clase para manejar la marca de agua
        public class WatermarkEvent : IPdfPageEvent
        {
            private readonly string watermarkText;

            public WatermarkEvent(string watermarkText)
            {
                this.watermarkText = watermarkText;
            }

            public void OnStartPage(PdfWriter writer, Document document)
            {
                // No se necesita acción aquí
            }

            public void OnEndPage(PdfWriter writer, Document document)
            {
                PdfContentByte canvas = writer.DirectContentUnder;
                Font watermarkFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 50, BaseColor.LIGHT_GRAY);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER,
                    new Phrase(watermarkText, watermarkFont),
                    document.PageSize.Width / 2,
                    document.PageSize.Height / 2,
                    45); // Ángulo de 45 grados
            }

            public void OnOpenDocument(PdfWriter writer, Document document)
            {
                // No se necesita acción aquí
            }

            public void OnCloseDocument(PdfWriter writer, Document document)
            {
                // No se necesita acción aquí
            }

            public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition)
            {
                // No se necesita acción aquí
            }

            public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition)
            {
                // No se necesita acción aquí
            }

            public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
            {
                // No se necesita acción aquí
            }

            public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition)
            {
                // No se necesita acción aquí
            }

            public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title)
            {
                // No se necesita acción aquí
            }

            public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition)
            {
                // No se necesita acción aquí
            }

            public void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, string text)
            {
                // No se necesita acción aquí
            }
        }





    }
}
