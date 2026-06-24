using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndDeliveryIntegrityConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId_CompletedOnly",
                table: "Payments",
                column: "OrderId",
                unique: true,
                filter: "[Status] = 'Completed'");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId_ActiveOnly",
                table: "Deliveries",
                column: "OrderId",
                unique: true,
                filter: "[Status] IN ('Assigned', 'PickedUp')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId_CompletedOnly",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId_ActiveOnly",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId");
        }
    }
}
