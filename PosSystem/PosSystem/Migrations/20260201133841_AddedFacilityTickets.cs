using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedFacilityTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacilityTickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityTickets_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MembershipNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-food-01",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 1, 13, 38, 41, 528, DateTimeKind.Utc).AddTicks(8446));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-park-01",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 1, 13, 38, 41, 528, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "BusinessCategories",
                keyColumn: "Id",
                keyValue: "cat-retail-01",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 1, 13, 38, 41, 528, DateTimeKind.Utc).AddTicks(8430));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-kitchen-01",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 1, 13, 38, 41, 528, DateTimeKind.Utc).AddTicks(8552));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-sales-01",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 1, 13, 38, 41, 528, DateTimeKind.Utc).AddTicks(8535));

            migrationBuilder.UpdateData(
                table: "UnitTypes",
                keyColumn: "Id",
                keyValue: "type-warehouse-01",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 1, 13, 38, 41, 528, DateTimeKind.Utc).AddTicks(8546));

            migrationBuilder.CreateIndex(
                name: "IX_FacilityTickets_ProductId",
                table: "FacilityTickets",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UserId",
                table: "Memberships",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityTickets");

            migrationBuilder.DropTable(
                name: "Memberships");

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
    }
}
