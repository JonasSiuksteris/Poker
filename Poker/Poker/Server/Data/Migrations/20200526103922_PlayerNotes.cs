using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class PlayerNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a703c34-7d90-448d-a467-4ee41b609d20");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88abcde1-2225-424c-83b8-4da7ba1f58e9");

            migrationBuilder.CreateTable(
                name: "PlayerNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    NotedPlayerName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerNotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "afafa750-4531-46bc-b585-e63aa9bc7f4d", "24205ebe-afcf-407a-85c2-bfbfdd0dfb49", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bcb4c140-63dd-4be9-afe7-c8270bdf4e35", "a4e1be8a-92ad-4b26-98dc-63f468b3edf6", "Admin", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNotes_UserId",
                table: "PlayerNotes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerNotes");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "afafa750-4531-46bc-b585-e63aa9bc7f4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcb4c140-63dd-4be9-afe7-c8270bdf4e35");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4a703c34-7d90-448d-a467-4ee41b609d20", "9cf211ec-7612-4930-9437-3478c3de8a76", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "88abcde1-2225-424c-83b8-4da7ba1f58e9", "d3f4ac5f-7530-473a-8cc8-8328d2826ec4", "Admin", "ADMIN" });
        }
    }
}
