using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TLSpammer.WEB.Migrations
{
    public partial class initializedadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "21645d62-277e-4b96-a36b-821b067f3d62", 0, "bf2f8745-e6e6-4ae7-96db-baf895f795d4", "admin@admin.com", true, false, null, "admin@admin.com", "admin", "AQAAAAEAACcQAAAAEApr7+5WeGyU29AzCclPSShLbO4AW+vN/AXXbQFX+bSX7MGfi25vVcoS+hRk+o19Lw==", null, false, "", false, "admin" });

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 24, 10, 28, 45, 273, DateTimeKind.Local).AddTicks(4241));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "21645d62-277e-4b96-a36b-821b067f3d62");

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 23, 11, 15, 46, 365, DateTimeKind.Local).AddTicks(4796));
        }
    }
}
