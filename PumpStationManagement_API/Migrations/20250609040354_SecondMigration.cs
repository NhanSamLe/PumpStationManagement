using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PumpStationManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "OperatingDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "OperatingDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "MaintenanceHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "MaintenanceHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatingDatas_CreatedBy",
                table: "OperatingDatas",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingDatas_ModifiedBy",
                table: "OperatingDatas",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_CreatedBy",
                table: "MaintenanceHistories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_ModifiedBy",
                table: "MaintenanceHistories",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceHistories_Users_CreatedBy",
                table: "MaintenanceHistories",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceHistories_Users_ModifiedBy",
                table: "MaintenanceHistories",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperatingDatas_Users_CreatedBy",
                table: "OperatingDatas",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperatingDatas_Users_ModifiedBy",
                table: "OperatingDatas",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceHistories_Users_CreatedBy",
                table: "MaintenanceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceHistories_Users_ModifiedBy",
                table: "MaintenanceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OperatingDatas_Users_CreatedBy",
                table: "OperatingDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_OperatingDatas_Users_ModifiedBy",
                table: "OperatingDatas");

            migrationBuilder.DropIndex(
                name: "IX_OperatingDatas_CreatedBy",
                table: "OperatingDatas");

            migrationBuilder.DropIndex(
                name: "IX_OperatingDatas_ModifiedBy",
                table: "OperatingDatas");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceHistories_CreatedBy",
                table: "MaintenanceHistories");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceHistories_ModifiedBy",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OperatingDatas");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "OperatingDatas");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "MaintenanceHistories");
        }
    }
}
