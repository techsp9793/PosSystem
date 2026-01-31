using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryFlagsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPosVisible",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TrackStock",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 17, 44, 4, 444, DateTimeKind.Utc).AddTicks(3260));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 17, 44, 4, 444, DateTimeKind.Utc).AddTicks(3254));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 17, 44, 4, 444, DateTimeKind.Utc).AddTicks(3244));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 17, 44, 4, 444, DateTimeKind.Utc).AddTicks(3420));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 17, 44, 4, 444, DateTimeKind.Utc).AddTicks(3406));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 17, 44, 4, 444, DateTimeKind.Utc).AddTicks(3414));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPosVisible",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TrackStock",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 15, 3, 5, 177, DateTimeKind.Utc).AddTicks(2797));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 15, 3, 5, 177, DateTimeKind.Utc).AddTicks(2758));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 15, 3, 5, 177, DateTimeKind.Utc).AddTicks(2746));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 15, 3, 5, 177, DateTimeKind.Utc).AddTicks(3010));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 15, 3, 5, 177, DateTimeKind.Utc).AddTicks(2996));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 15, 3, 5, 177, DateTimeKind.Utc).AddTicks(3004));
        }
    }
}
