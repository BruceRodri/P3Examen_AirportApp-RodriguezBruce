using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace P3Examen_AirportApp.Data.MigrationsIdentity
{
    /// <inheritdoc />
    public partial class AgregarReservasServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AirportId",
                schema: "airportdb",
                table: "ShoppingCartItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AirportName",
                schema: "airportdb",
                table: "ShoppingCartItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ServiceDate",
                schema: "airportdb",
                table: "ShoppingCartItems",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                schema: "airportdb",
                table: "ShoppingCartItems",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateTable(
                name: "ServiceAvailabilities",
                schema: "airportdb",
                columns: table => new
                {
                    ServiceAvailabilityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AirportServiceId = table.Column<int>(type: "integer", nullable: false),
                    AirportId = table.Column<int>(type: "integer", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    ReservedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAvailabilities", x => x.ServiceAvailabilityId);
                    table.ForeignKey(
                        name: "FK_ServiceAvailabilities_AirportServices_AirportServiceId",
                        column: x => x.AirportServiceId,
                        principalSchema: "airportdb",
                        principalTable: "AirportServices",
                        principalColumn: "AirportServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceReservations",
                schema: "airportdb",
                columns: table => new
                {
                    ServiceReservationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AirportId = table.Column<int>(type: "integer", nullable: false),
                    AirportName = table.Column<string>(type: "text", nullable: false),
                    AirportServiceId = table.Column<int>(type: "integer", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReservations", x => x.ServiceReservationId);
                    table.ForeignKey(
                        name: "FK_ServiceReservations_AirportServices_AirportServiceId",
                        column: x => x.AirportServiceId,
                        principalSchema: "airportdb",
                        principalTable: "AirportServices",
                        principalColumn: "AirportServiceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceReservations_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "airportdb",
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAvailabilities_AirportServiceId_AirportId_ServiceDat~",
                schema: "airportdb",
                table: "ServiceAvailabilities",
                columns: new[] { "AirportServiceId", "AirportId", "ServiceDate", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReservations_AirportServiceId",
                schema: "airportdb",
                table: "ServiceReservations",
                column: "AirportServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReservations_PurchaseOrderId",
                schema: "airportdb",
                table: "ServiceReservations",
                column: "PurchaseOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceAvailabilities",
                schema: "airportdb");

            migrationBuilder.DropTable(
                name: "ServiceReservations",
                schema: "airportdb");

            migrationBuilder.DropColumn(
                name: "AirportId",
                schema: "airportdb",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "AirportName",
                schema: "airportdb",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                schema: "airportdb",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "airportdb",
                table: "ShoppingCartItems");
        }
    }
}
