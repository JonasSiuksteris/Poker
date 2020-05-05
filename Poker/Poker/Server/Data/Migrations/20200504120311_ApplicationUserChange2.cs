using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class ApplicationUserChange2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0d6c4295-6b33-43fe-ac80-5966f400f064");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48b6a847-42be-4c98-bf19-193b09e1ba16");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5e0bb619-4948-492a-863d-d42ac19f20b1", "a39a63eb-416c-4f95-a9c5-9a0d6fc709e4", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e364982b-6a56-4d8f-b05d-9d08ef1bac50", "3565920f-a6b8-45ad-bc7a-72b237a8370a", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e0bb619-4948-492a-863d-d42ac19f20b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e364982b-6a56-4d8f-b05d-9d08ef1bac50");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0d6c4295-6b33-43fe-ac80-5966f400f064", "a9a8f045-9007-411a-ad5f-dcc7edf027fd", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "48b6a847-42be-4c98-bf19-193b09e1ba16", "239c7ce4-20c8-4bda-bd05-8113ae6a0611", "Admin", "ADMIN" });
        }
    }
}
