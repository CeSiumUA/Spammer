using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TLSpammer.WEB.Migrations
{
    public partial class initializedadminfixed1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "527be361-aa09-4d54-a67b-033bc9f79b92");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "df32a386-f730-45ee-b178-531f66eaaf5d", 0, "68d486e7-df03-44b9-bff8-6de11d3a9783", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEC8dF1A4xds/YHTSU7yiCuLRPlgcsCm8U330bnZbfzvF/NfFIhX7J1nVP5qD8T0pcA==", null, false, "", false, "admin@admin.com" });

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 24, 11, 11, 31, 437, DateTimeKind.Local).AddTicks(8878));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df32a386-f730-45ee-b178-531f66eaaf5d");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "527be361-aa09-4d54-a67b-033bc9f79b92", 0, "a16e696f-1d28-47c0-bd24-8eb30ed561b8", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEPNrrSwGtH3lkhUMEV70Zn3V4w5IKHinZMLrAI0+cmkit8VGHVFdRnwWkW08kU+q/w==", null, false, "", false, "admin@admin.com" });

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 24, 10, 36, 8, 756, DateTimeKind.Local).AddTicks(966));
        }
    }
}
