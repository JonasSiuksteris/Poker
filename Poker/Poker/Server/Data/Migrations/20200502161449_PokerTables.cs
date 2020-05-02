using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.Data.Migrations
{
    public partial class PokerTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokerTables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaxPlayers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokerTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTables",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    TableId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTables", x => new { x.UserId, x.TableId });
                    table.ForeignKey(
                        name: "FK_PlayerTables_PokerTables_TableId",
                        column: x => x.TableId,
                        principalTable: "PokerTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerTables_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTables_TableId",
                table: "PlayerTables",
                column: "TableId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerTables");

            migrationBuilder.DropTable(
                name: "PokerTables");
        }
    }
}
