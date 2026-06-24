using System;
using Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260624000100_AddStudentUserAccount")]
    public partial class AddStudentUserAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "student",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_user_id",
                table: "student",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_student_user_user_id",
                table: "student",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_student_user_user_id",
                table: "student");

            migrationBuilder.DropIndex(
                name: "ix_student_user_id",
                table: "student");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "student");
        }
    }
}
