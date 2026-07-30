using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id",
                keyValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "clave", "descripcion", "nombre", "permisos" },
                values: new object[,]
                {
                    { 2, "SUPERVISOR", null, "Supervisor de zona", null },
                    { 3, "DIRECTOR_USAER", null, "Director de USAER", null },
                    { 7, "TRABAJO_SOCIAL", null, "Trabajo Social", null },
                    { 10, "ALUMNO", null, "Alumno (autoservicio)", null }
                });
        }
    }
}
