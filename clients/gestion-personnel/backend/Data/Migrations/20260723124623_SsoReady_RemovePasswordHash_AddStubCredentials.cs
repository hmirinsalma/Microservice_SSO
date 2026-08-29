using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPersonnel.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SsoReady_RemovePasswordHash_AddStubCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "SsoId",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StubCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StubCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StubCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SsoId",
                table: "Users",
                column: "SsoId");

            migrationBuilder.CreateIndex(
                name: "IX_StubCredentials_UserId",
                table: "StubCredentials",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StubCredentials");

            migrationBuilder.DropIndex(
                name: "IX_Users_SsoId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SsoId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
