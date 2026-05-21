using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ComprehensiveSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ConfirmedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Products",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmedById",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Categories",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "Addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[SubTotal] + [DeliveryFee] + [Tax] - [Discount]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Subtotal",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Quantity] * [UnitPrice]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComputedColumnSql: "Quantity * UnitPrice",
                oldStored: null);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_CategoryId",
                table: "Products",
                columns: new[] { "Name", "CategoryId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Discount",
                table: "Products",
                sql: "[DiscountPercent] <= [AllowedDiscountPercent]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Price",
                table: "Products",
                sql: "[Price] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ConfirmedById",
                table: "Orders",
                column: "ConfirmedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ConfirmedById",
                table: "Orders",
                column: "ConfirmedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ConfirmedById",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Products_Name_CategoryId",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Discount",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Price",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ConfirmedById",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ConfirmedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "Image");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComputedColumnSql: "[SubTotal] + [DeliveryFee] + [Tax] - [Discount]");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmedBy",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Categories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Subtotal",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "Quantity * UnitPrice",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComputedColumnSql: "[Quantity] * [UnitPrice]");
        }
    }
}
