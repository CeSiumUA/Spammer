using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TLSpammer.WEB.Migrations
{
    public partial class Addedchanneltype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Input",
                table: "SelectedChats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 23, 11, 15, 46, 365, DateTimeKind.Local).AddTicks(4796));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Input",
                table: "SelectedChats");

            migrationBuilder.UpdateData(
                table: "Times",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTime(2021, 6, 23, 11, 4, 27, 306, DateTimeKind.Local).AddTicks(1386));
        }
    }
}
