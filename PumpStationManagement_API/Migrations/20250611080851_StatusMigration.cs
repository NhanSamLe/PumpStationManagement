using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PumpStationManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class StatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PumpStatusBackups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PumpId = table.Column<int>(type: "int", nullable: false),
                    OriginalStatus = table.Column<int>(type: "int", nullable: false),
                    BackupTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpStatusBackups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PumpStatusBackups_Pumps_PumpId",
                        column: x => x.PumpId,
                        principalTable: "Pumps",
                        principalColumn: "PumpId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpStatusBackups_PumpId",
                table: "PumpStatusBackups",
                column: "PumpId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumpStatusBackups");
        }
    }
}
