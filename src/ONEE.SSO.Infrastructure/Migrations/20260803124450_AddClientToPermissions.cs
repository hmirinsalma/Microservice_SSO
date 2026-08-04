using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEE.SSO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientToPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permissions_Code",
                table: "Permissions");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientApplicationId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ClientApplicationId",
                table: "Permissions",
                column: "ClientApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ClientId_Code",
                table: "Permissions",
                columns: new[] { "ClientId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_ClientApplications_ClientApplicationId",
                table: "Permissions",
                column: "ClientApplicationId",
                principalTable: "ClientApplications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_ClientApplications_ClientId",
                table: "Permissions",
                column: "ClientId",
                principalTable: "ClientApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_ClientApplications_ClientApplicationId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_ClientApplications_ClientId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_ClientApplicationId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_ClientId_Code",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ClientApplicationId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Permissions");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);
        }
    }
}
