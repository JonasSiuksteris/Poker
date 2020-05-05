using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class ApplicationUserChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b6d180f-b2a4-4b41-8e2f-9adb413da0c5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "924c2d35-8500-4d4f-871f-2f9a5a346a0b");

            migrationBuilder.AddColumn<int>(
                name: "CurrentTableId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0d6c4295-6b33-43fe-ac80-5966f400f064", "a9a8f045-9007-411a-ad5f-dcc7edf027fd", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "48b6a847-42be-4c98-bf19-193b09e1ba16", "239c7ce4-20c8-4bda-bd05-8113ae6a0611", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0d6c4295-6b33-43fe-ac80-5966f400f064");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48b6a847-42be-4c98-bf19-193b09e1ba16");

            migrationBuilder.DropColumn(
                name: "CurrentTableId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0b6d180f-b2a4-4b41-8e2f-9adb413da0c5", "d5d834bb-57ff-4d47-ab14-049b25520f31", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "924c2d35-8500-4d4f-871f-2f9a5a346a0b", "59cc2698-c8cb-4d90-a279-ca0e3cd4b4bb", "Admin", "ADMIN" });
        }
    }
}
