using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinBineBackend.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class AddGroupStatusToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupStatus",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "None");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupStatus",
                table: "Users");
        }
    }
}
