using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeHotel.Migrations
{
    /// <inheritdoc />
    public partial class nn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clientes__3214EC2739723F69", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    capacidad = table.Column<int>(type: "int", nullable: true),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    servicios_adicionales = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__3214EC278CC9BA11", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cliente_id = table.Column<int>(type: "int", nullable: true),
                    habitacion_id = table.Column<int>(type: "int", nullable: true),
                    fecha_entrada = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_salida = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    forma_pago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservas__3214EC2757AF986E", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Reservas__client__145C0A3F",
                        column: x => x.cliente_id,
                        principalTable: "Clientes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Reservas__habita__15502E78",
                        column: x => x.habitacion_id,
                        principalTable: "Habitaciones",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reserva_id = table.Column<int>(type: "int", nullable: true),
                    monto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    fecha = table.Column<DateOnly>(type: "date", nullable: true),
                    forma_pago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pagos__3214EC2785B8FC6C", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Pagos__reserva_i__182C9B23",
                        column: x => x.reserva_id,
                        principalTable: "Reservas",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_reserva_id",
                table: "Pagos",
                column: "reserva_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_cliente_id",
                table: "Reservas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_habitacion_id",
                table: "Reservas",
                column: "habitacion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Habitaciones");
        }
    }
}
