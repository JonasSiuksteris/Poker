using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.Data.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8c28aa46-cec2-4eb9-9282-ff65ecd7ccfb", "e08cd04f-7550-4c33-b9ec-aa732207e5fc", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3b86fa76-69ec-4432-855f-8b7dbc0c2e54", "64e4426a-4d12-47c3-9ffe-b16988db9a79", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b86fa76-69ec-4432-855f-8b7dbc0c2e54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c28aa46-cec2-4eb9-9282-ff65ecd7ccfb");
        }
    }
}
