using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholarFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMVPFieldsAndExplanationSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Explanations_QuestionId_Type",
                table: "Explanations");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Explanations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Explanations");

            migrationBuilder.AddColumn<decimal>(
                name: "Marks",
                table: "Questions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeLimit",
                table: "Papers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Papers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "Options",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Explanations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExplanationSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExplanationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplanationSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExplanationSections_Explanations_ExplanationId",
                        column: x => x.ExplanationId,
                        principalTable: "Explanations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExplanationSections_ExplanationId_OrderIndex",
                table: "ExplanationSections",
                columns: new[] { "ExplanationId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExplanationSections");

            migrationBuilder.DropColumn(
                name: "Marks",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Explanations");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Explanations",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Explanations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Explanations_QuestionId_Type",
                table: "Explanations",
                columns: new[] { "QuestionId", "Type" },
                filter: "[IsDeleted] = 0");
        }
    }
}
