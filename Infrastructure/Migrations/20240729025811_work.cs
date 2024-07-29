using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class work : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "Servers");

            migrationBuilder.RenameColumn(
                name: "ServerStatus",
                table: "Servers",
                newName: "DeployStatus");

            migrationBuilder.CreateTable(
                name: "ConnectionData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    ServerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectionData_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionData_ServerId",
                table: "ConnectionData",
                column: "ServerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionData");

            migrationBuilder.RenameColumn(
                name: "DeployStatus",
                table: "Servers",
                newName: "ServerStatus");

            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "Servers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "Servers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
