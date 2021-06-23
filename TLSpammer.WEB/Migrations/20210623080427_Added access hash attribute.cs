using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TLSpammer.WEB.Migrations
{
    public partial class Addedaccesshashattribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccessHash",
                table: "SelectedChats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 23, 11, 4, 27, 306, DateTimeKind.Local).AddTicks(1386));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessHash",
                table: "SelectedChats");

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 23, 8, 48, 39, 522, DateTimeKind.Local).AddTicks(2610));
        }
    }
}
