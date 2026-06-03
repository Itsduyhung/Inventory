using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryDashboard.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthSalesVehicleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionStatus",
                table: "vehicles");

            migrationBuilder.RenameColumn(
                name: "ActionUpdatedAt",
                table: "vehicles",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "vehicles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "vehicles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "vehicles",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExteriorColor",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InteriorColor",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Mileage",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "vehicles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE vehicles SET "Note" = "ProposedAction" WHERE "ProposedAction" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "ProposedAction",
                table: "vehicles");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Transmission",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sale_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantitySold = table.Column<int>(type: "integer", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingQuantityAfterSale = table.Column<int>(type: "integer", nullable: false),
                    SoldAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sale_records_users_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_records_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sale_records_StaffUserId",
                table: "sale_records",
                column: "StaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_records_VehicleId",
                table: "sale_records",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_records");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Engine",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "ExteriorColor",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "InteriorColor",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "vehicles");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "vehicles",
                newName: "ActionUpdatedAt");

            migrationBuilder.AddColumn<string>(
                name: "ProposedAction",
                table: "vehicles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionStatus",
                table: "vehicles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
