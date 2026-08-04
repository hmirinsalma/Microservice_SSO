using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEE.SSO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "UserRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ClientId_Name",
                table: "Roles",
                columns: new[] { "ClientId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_ClientApplications_ClientId",
                table: "Roles",
                column: "ClientId",
                principalTable: "ClientApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_ClientApplications_ClientId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_ClientId_Name",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Roles");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);
        }
    }
}
