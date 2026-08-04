using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEE.SSO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_ClientApplications_ClientApplicationId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_ClientApplicationId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ClientApplicationId",
                table: "Permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClientApplicationId",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ClientApplicationId",
                table: "Permissions",
                column: "ClientApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_ClientApplications_ClientApplicationId",
                table: "Permissions",
                column: "ClientApplicationId",
                principalTable: "ClientApplications",
                principalColumn: "Id");
        }
    }
}
