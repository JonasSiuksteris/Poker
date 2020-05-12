using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class AddCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e0bb619-4948-492a-863d-d42ac19f20b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e364982b-6a56-4d8f-b05d-9d08ef1bac50");

            migrationBuilder.DropColumn(
                name: "CurrentTableId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1bc7a836-f847-4f90-8551-6a825d581949", "d261548d-350c-4d61-a7bf-fe88570ec26b", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c9a1b2f2-fc29-496d-9339-e6a0cd68fd63", "56fe47e5-1467-40e0-8096-5b26fd5e6f2d", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bc7a836-f847-4f90-8551-6a825d581949");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9a1b2f2-fc29-496d-9339-e6a0cd68fd63");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CurrentTableId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlayerTables",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5e0bb619-4948-492a-863d-d42ac19f20b1", "a39a63eb-416c-4f95-a9c5-9a0d6fc709e4", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e364982b-6a56-4d8f-b05d-9d08ef1bac50", "3565920f-a6b8-45ad-bc7a-72b237a8370a", "Admin", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTables_TableId",
                table: "PlayerTables",
                column: "TableId");
        }
    }
}
