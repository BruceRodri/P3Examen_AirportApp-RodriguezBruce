using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P3Examen_AirportApp.Data.MigrationsIdentity
{
    /// <inheritdoc />
    public partial class AgregarPaypalATransacciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayResponse",
                schema: "airportdb",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalApprovalUrl",
                schema: "airportdb",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalCaptureId",
                schema: "airportdb",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalOrderId",
                schema: "airportdb",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
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
                name: "GatewayResponse",
                schema: "airportdb",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PayPalApprovalUrl",
                schema: "airportdb",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PayPalCaptureId",
                schema: "airportdb",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PayPalOrderId",
                schema: "airportdb",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Provider",
                schema: "airportdb",
                table: "PaymentTransactions");
        }
    }
}
