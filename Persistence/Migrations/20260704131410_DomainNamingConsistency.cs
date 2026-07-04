using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DomainNamingConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "ProjectParticipants");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Tickets",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Tickets",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Projects",
                newName: "DeletedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Tickets",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Tickets",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Projects",
                newName: "DeletedDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "ProjectParticipants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
