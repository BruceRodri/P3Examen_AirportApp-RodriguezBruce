using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P3Examen_AirportApp.Data.MigrationsIdentity
{
    /// <inheritdoc />
    public partial class AgregarUserIdCurrencyTransacciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "airportdb",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "airportdb",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "airportdb",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "airportdb",
                table: "PaymentTransactions");
        }
    }
}
