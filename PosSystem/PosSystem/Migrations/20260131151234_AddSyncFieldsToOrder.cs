using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 12, 34, 323, DateTimeKind.Utc).AddTicks(4079));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 12, 34, 323, DateTimeKind.Utc).AddTicks(4073));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 12, 34, 323, DateTimeKind.Utc).AddTicks(4064));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 12, 34, 323, DateTimeKind.Utc).AddTicks(4268));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 12, 34, 323, DateTimeKind.Utc).AddTicks(4254));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 12, 34, 323, DateTimeKind.Utc).AddTicks(4262));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 20, 57, 12, 879, DateTimeKind.Utc).AddTicks(2966));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 20, 57, 12, 879, DateTimeKind.Utc).AddTicks(2950));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 20, 57, 12, 879, DateTimeKind.Utc).AddTicks(2935));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 20, 57, 12, 879, DateTimeKind.Utc).AddTicks(3129));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 20, 57, 12, 879, DateTimeKind.Utc).AddTicks(3111));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 30, 20, 57, 12, 879, DateTimeKind.Utc).AddTicks(3121));
        }
    }
}
