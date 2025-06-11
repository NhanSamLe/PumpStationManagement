using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PumpStationManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class AddOpHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalOperatingHours",
                table: "Pumps",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOperatingHours",
                table: "Pumps");
        }
    }
}
