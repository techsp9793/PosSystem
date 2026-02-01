using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncFlagsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 27, 53, 692, DateTimeKind.Utc).AddTicks(9865));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 27, 53, 692, DateTimeKind.Utc).AddTicks(9859));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 27, 53, 692, DateTimeKind.Utc).AddTicks(9847));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 27, 53, 693, DateTimeKind.Utc).AddTicks(1));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 27, 53, 692, DateTimeKind.Utc).AddTicks(9987));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 15, 27, 53, 692, DateTimeKind.Utc).AddTicks(9995));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
