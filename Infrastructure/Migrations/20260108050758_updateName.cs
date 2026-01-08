using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameVersion",
                table: "GameVersion");

            migrationBuilder.RenameTable(
                name: "GameVersion",
                newName: "GameVersions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameVersions",
                table: "GameVersions",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameVersions",
                table: "GameVersions");

            migrationBuilder.RenameTable(
                name: "GameVersions",
                newName: "GameVersion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameVersion",
                table: "GameVersion",
                column: "Id");
        }
    }
}
