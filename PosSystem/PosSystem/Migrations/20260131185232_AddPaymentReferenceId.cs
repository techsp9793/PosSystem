using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentReferenceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentReferenceId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 18, 52, 32, 588, DateTimeKind.Utc).AddTicks(5022));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 18, 52, 32, 588, DateTimeKind.Utc).AddTicks(5016));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 18, 52, 32, 588, DateTimeKind.Utc).AddTicks(5006));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 18, 52, 32, 588, DateTimeKind.Utc).AddTicks(5162));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 18, 52, 32, 588, DateTimeKind.Utc).AddTicks(5143));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 1, 31, 18, 52, 32, 588, DateTimeKind.Utc).AddTicks(5156));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentReferenceId",
                table: "Orders");

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
    }
}
