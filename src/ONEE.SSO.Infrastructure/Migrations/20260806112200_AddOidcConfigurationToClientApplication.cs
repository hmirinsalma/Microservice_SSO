using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONEE.SSO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOidcConfigurationToClientApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessTokenLifetime",
                table: "ClientApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AllowedGrantTypes",
                table: "ClientApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AllowedScopes",
                table: "ClientApplications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostLogoutRedirectUri",
                table: "ClientApplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenLifetime",
                table: "ClientApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequireConsent",
                table: "ClientApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequirePkce",
                table: "ClientApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessTokenLifetime",
                table: "ClientApplications");

            migrationBuilder.DropColumn(
                name: "AllowedGrantTypes",
                table: "ClientApplications");

            migrationBuilder.DropColumn(
                name: "AllowedScopes",
                table: "ClientApplications");

            migrationBuilder.DropColumn(
                name: "PostLogoutRedirectUri",
                table: "ClientApplications");

            migrationBuilder.DropColumn(
                name: "RefreshTokenLifetime",
                table: "ClientApplications");

            migrationBuilder.DropColumn(
                name: "RequireConsent",
                table: "ClientApplications");

            migrationBuilder.DropColumn(
                name: "RequirePkce",
                table: "ClientApplications");
        }
    }
}
