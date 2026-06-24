using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitOrderNotesIntoCustomerAndStaffNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Orders",
                newName: "StaffNotes");

            migrationBuilder.AddColumn<string>(
                name: "CustomerNotes",
                table: "Orders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNotes",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "StaffNotes",
                table: "Orders",
                newName: "Notes");
        }
    }
}
