using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinBineBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddIngestionDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "IngestionDate",
                table: "Logs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IngestionDate",
                table: "Logs",
                column: "IngestionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Logs_IngestionDate",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "IngestionDate",
                table: "Logs");
        }
    }
}
