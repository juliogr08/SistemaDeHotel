using Microsoft.EntityFrameworkCore;
using SistemaDeHotel.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el contexto de la base de datos
builder.Services.AddDbContext<ReservasHotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conn")));

// Agregar soporte para sesiones
builder.Services.AddDistributedMemoryCache(); // Usar en memoria para sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de espera para la sesión
    options.Cookie.HttpOnly = true; // Asegura que solo el servidor pueda acceder a la cookie de la sesión
    options.Cookie.IsEssential = true; // Necesario para que funcione en algunos entornos como producción
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// Habilitar el uso de sesiones
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
