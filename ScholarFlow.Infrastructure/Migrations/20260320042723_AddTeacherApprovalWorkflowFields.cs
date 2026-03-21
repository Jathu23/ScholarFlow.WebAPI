using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholarFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherApprovalWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "TeacherProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "TeacherProfiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "TeacherProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TeacherProfiles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "TeacherProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherCode",
                table: "TeacherProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_Status",
                table: "TeacherProfiles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_SubjectId",
                table: "TeacherProfiles",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_TeacherCode",
                table: "TeacherProfiles",
                column: "TeacherCode",
                unique: true,
                filter: "[TeacherCode] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherProfiles_Subjects_SubjectId",
                table: "TeacherProfiles",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherProfiles_Subjects_SubjectId",
                table: "TeacherProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TeacherProfiles_Status",
                table: "TeacherProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TeacherProfiles_SubjectId",
                table: "TeacherProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TeacherProfiles_TeacherCode",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "TeacherCode",
                table: "TeacherProfiles");
        }
    }
}
