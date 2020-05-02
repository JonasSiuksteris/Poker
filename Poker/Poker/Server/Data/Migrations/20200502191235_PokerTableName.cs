using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class PokerTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b86fa76-69ec-4432-855f-8b7dbc0c2e54");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c28aa46-cec2-4eb9-9282-ff65ecd7ccfb");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PokerTables",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6183efa0-fad2-409e-8643-e0f7832bf6be", "eac93876-f383-4276-8d56-0958af5819ec", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "54986024-0ed3-4782-abd3-b960b8ac611a", "0d41e1d1-38ee-4564-8357-e836bcb6f469", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "54986024-0ed3-4782-abd3-b960b8ac611a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6183efa0-fad2-409e-8643-e0f7832bf6be");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PokerTables");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8c28aa46-cec2-4eb9-9282-ff65ecd7ccfb", "e08cd04f-7550-4c33-b9ec-aa732207e5fc", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3b86fa76-69ec-4432-855f-8b7dbc0c2e54", "64e4426a-4d12-47c3-9ffe-b16988db9a79", "Admin", "ADMIN" });
        }
    }
}
