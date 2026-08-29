using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPersonnel.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCongesAndEnrichEmploye : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Employes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Employes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponsableId",
                table: "Employes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Employes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motif = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentaireChef = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CommentaireDirecteur = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateTraitementChef = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateTraitementDirecteur = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeId = table.Column<int>(type: "int", nullable: false),
                    ChefServiceId = table.Column<int>(type: "int", nullable: true),
                    DirecteurId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conges_Employes_ChefServiceId",
                        column: x => x.ChefServiceId,
                        principalTable: "Employes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conges_Employes_DirecteurId",
                        column: x => x.DirecteurId,
                        principalTable: "Employes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conges_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employes_ResponsableId",
                table: "Employes",
                column: "ResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_Employes_UserId",
                table: "Employes",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Conges_ChefServiceId",
                table: "Conges",
                column: "ChefServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Conges_DirecteurId",
                table: "Conges",
                column: "DirecteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Conges_EmployeId",
                table: "Conges",
                column: "EmployeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Employes_ResponsableId",
                table: "Employes",
                column: "ResponsableId",
                principalTable: "Employes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_Users_UserId",
                table: "Employes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Employes_ResponsableId",
                table: "Employes");

            migrationBuilder.DropForeignKey(
                name: "FK_Employes_Users_UserId",
                table: "Employes");

            migrationBuilder.DropTable(
                name: "Conges");

            migrationBuilder.DropIndex(
                name: "IX_Employes_ResponsableId",
                table: "Employes");

            migrationBuilder.DropIndex(
                name: "IX_Employes_UserId",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "ResponsableId",
                table: "Employes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employes");
        }
    }
}
