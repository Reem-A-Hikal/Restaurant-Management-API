using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryStatusTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Addresses_Latitude",
                table: "Addresses",
                sql: "[Latitude] >= -90 AND [Latitude] <= 90 OR [Latitude] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Addresses_Longitude",
                table: "Addresses",
                sql: "[Longitude] >= -180 AND [Longitude] <= 180 OR [Longitude] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Addresses_Latitude",
                table: "Addresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Addresses_Longitude",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Deliveries");
        }
    }
}
