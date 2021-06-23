using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TLSpammer.WEB.Migrations
{
    public partial class Addedinitialdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Times",
                columns: new[] { "Id", "Time" },
                values: new object[] { 1, new DateTime(2021, 6, 23, 8, 13, 8, 776, DateTimeKind.Local).AddTicks(2789) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
