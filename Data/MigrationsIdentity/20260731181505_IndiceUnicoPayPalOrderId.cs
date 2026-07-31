using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P3Examen_AirportApp.Data.MigrationsIdentity
{
    /// <inheritdoc />
    public partial class IndiceUnicoPayPalOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PayPalOrderId",
                schema: "airportdb",
                table: "PaymentTransactions",
                column: "PayPalOrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_PayPalOrderId",
                schema: "airportdb",
                table: "PaymentTransactions");
        }
    }
}
