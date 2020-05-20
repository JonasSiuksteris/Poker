using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Server.data.Migrations
{
    public partial class PokerTable_smalls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bc7a836-f847-4f90-8551-6a825d581949");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9a1b2f2-fc29-496d-9339-e6a0cd68fd63");

            migrationBuilder.AddColumn<int>(
                name: "SmallBlind",
                table: "PokerTables",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4a703c34-7d90-448d-a467-4ee41b609d20", "9cf211ec-7612-4930-9437-3478c3de8a76", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "88abcde1-2225-424c-83b8-4da7ba1f58e9", "d3f4ac5f-7530-473a-8cc8-8328d2826ec4", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a703c34-7d90-448d-a467-4ee41b609d20");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88abcde1-2225-424c-83b8-4da7ba1f58e9");

            migrationBuilder.DropColumn(
                name: "SmallBlind",
                table: "PokerTables");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1bc7a836-f847-4f90-8551-6a825d581949", "d261548d-350c-4d61-a7bf-fe88570ec26b", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c9a1b2f2-fc29-496d-9339-e6a0cd68fd63", "56fe47e5-1467-40e0-8096-5b26fd5e6f2d", "Admin", "ADMIN" });
        }
    }
}
