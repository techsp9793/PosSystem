using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToUnitType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UnitTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UnitProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 18, 21, 3, 108, DateTimeKind.Utc).AddTicks(6602));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 18, 21, 3, 108, DateTimeKind.Utc).AddTicks(6596));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 29, 18, 21, 3, 108, DateTimeKind.Utc).AddTicks(6586));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 1, 29, 18, 21, 3, 108, DateTimeKind.Utc).AddTicks(6765), null });

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 1, 29, 18, 21, 3, 108, DateTimeKind.Utc).AddTicks(6752), null });

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 1, 29, 18, 21, 3, 108, DateTimeKind.Utc).AddTicks(6759), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UnitTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UnitProducts");

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
    }
}
