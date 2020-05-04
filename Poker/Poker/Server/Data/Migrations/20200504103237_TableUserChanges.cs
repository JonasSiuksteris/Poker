using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class TableUserChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "54986024-0ed3-4782-abd3-b960b8ac611a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6183efa0-fad2-409e-8643-e0f7832bf6be");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0b6d180f-b2a4-4b41-8e2f-9adb413da0c5", "d5d834bb-57ff-4d47-ab14-049b25520f31", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "924c2d35-8500-4d4f-871f-2f9a5a346a0b", "59cc2698-c8cb-4d90-a279-ca0e3cd4b4bb", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b6d180f-b2a4-4b41-8e2f-9adb413da0c5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "924c2d35-8500-4d4f-871f-2f9a5a346a0b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6183efa0-fad2-409e-8643-e0f7832bf6be", "eac93876-f383-4276-8d56-0958af5819ec", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "54986024-0ed3-4782-abd3-b960b8ac611a", "0d41e1d1-38ee-4564-8357-e836bcb6f469", "Admin", "ADMIN" });
        }
    }
}
