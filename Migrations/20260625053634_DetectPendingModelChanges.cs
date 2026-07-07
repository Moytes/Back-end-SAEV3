using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class DetectPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "school_id",
                table: "student",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_school_id",
                table: "student",
                column: "school_id");

            migrationBuilder.AddForeignKey(
                name: "fk_student_school_school_id",
                table: "student",
                column: "school_id",
                principalTable: "school",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_student_school_school_id",
                table: "student");

            migrationBuilder.DropIndex(
                name: "ix_student_school_id",
                table: "student");

            migrationBuilder.DropColumn(
                name: "school_id",
                table: "student");
        }
    }
}
