using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentLoginSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "student_id",
                table: "user",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "student_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_user_student_id",
                table: "user",
                column: "student_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_student_student_id",
                table: "user",
                column: "student_id",
                principalTable: "student",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_student_student_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_student_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "student_id",
                table: "user");
        }
    }
}
