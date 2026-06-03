using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryDashboard.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Make = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DateAddedToInventory = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProposedAction = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActionStatus = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ActionUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_Vin",
                table: "vehicles",
                column: "Vin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicles");
        }
    }
}
