using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Granjas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NumeroVacas = table.Column<int>(type: "INTEGER", nullable: false),
                    LitrosPorVaca = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Granjas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FechaProduccion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    FechaVencimiento = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    LitrosUsados = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    CantidadInicial = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    CantidadDisponible = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    CantidadDanada = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Cantidad = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GranjaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LitrosPorUnidad = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    VidaUtilDias = table.Column<int>(type: "INTEGER", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParametrosProducto_Granjas_GranjaId",
                        column: x => x.GranjaId,
                        principalTable: "Granjas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vacas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GranjaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Raza = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacas_Granjas_GranjaId",
                        column: x => x.GranjaId,
                        principalTable: "Granjas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VentaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VentaId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaProduccion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VacaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Litros = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producciones_Vacas_VacaId",
                        column: x => x.VacaId,
                        principalTable: "Vacas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_FechaVencimiento",
                table: "Lotes",
                column: "FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_Tipo_FechaProduccion",
                table: "Lotes",
                columns: new[] { "Tipo", "FechaProduccion" });

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosProducto_GranjaId_Tipo",
                table: "ParametrosProducto",
                columns: new[] { "GranjaId", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_Fecha",
                table: "Producciones",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_VacaId_Fecha",
                table: "Producciones",
                columns: new[] { "VacaId", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacas_GranjaId",
                table: "Vacas",
                column: "GranjaId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_LoteId",
                table: "VentaDetalles",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_VentaId",
                table: "VentaDetalles",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Fecha",
                table: "Ventas",
                column: "Fecha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParametrosProducto");

            migrationBuilder.DropTable(
                name: "Producciones");

            migrationBuilder.DropTable(
                name: "VentaDetalles");

            migrationBuilder.DropTable(
                name: "Vacas");

            migrationBuilder.DropTable(
                name: "Lotes");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Granjas");
        }
    }
}
