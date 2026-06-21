using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReworkOrderDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryPersonId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_DeliveryPersonId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryPersonId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryEndTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPersonId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryStartTime",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0.00m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Deliveries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Amount",
                table: "Payments",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderDetails_Quantity",
                table: "OrderDetails",
                sql: "[Quantity] >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderDetails_UnitPrice",
                table: "OrderDetails",
                sql: "[UnitPrice] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryPersonId",
                table: "Deliveries",
                column: "DeliveryPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryPersonId",
                table: "Deliveries");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Amount",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderDetails_Quantity",
                table: "OrderDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderDetails_UnitPrice",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "Source",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0.00m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryEndTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPersonId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryStartTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Deliveries",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryPersonId",
                table: "Orders",
                column: "DeliveryPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryPersonId",
                table: "Deliveries",
                column: "DeliveryPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_DeliveryPersonId",
                table: "Orders",
                column: "DeliveryPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
