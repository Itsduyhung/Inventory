using InventoryDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryDashboard.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260602120000_AddUserAuthTokens")]
public partial class AddUserAuthTokens : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "users",
            type: "character varying(120)",
            maxLength: 120,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PasswordResetToken",
            table: "users",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "PasswordResetExpiresAt",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RefreshTokenHash",
            table: "users",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RefreshTokenExpiresAt",
            table: "users",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_users_Email",
            table: "users",
            column: "Email",
            unique: true,
            filter: "\"Email\" IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_users_Email",
            table: "users");

        migrationBuilder.DropColumn(
            name: "Email",
            table: "users");

        migrationBuilder.DropColumn(
            name: "PasswordResetToken",
            table: "users");

        migrationBuilder.DropColumn(
            name: "PasswordResetExpiresAt",
            table: "users");

        migrationBuilder.DropColumn(
            name: "RefreshTokenHash",
            table: "users");

        migrationBuilder.DropColumn(
            name: "RefreshTokenExpiresAt",
            table: "users");
    }
}
