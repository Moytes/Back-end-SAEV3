using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttentionAreasCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "cve", "name" },
                values: new object[] { "COMUNICACION", "Comunicación" });

            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "cve", "name" },
                values: new object[] { "ATENCION", "Atención" });

            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "cve", "name" },
                values: new object[] { "CONDUCTA", "Conducta" });

            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "cve", "name" },
                values: new object[] { "APRENDIZAJE", "Aprendizaje" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "cve", "name" },
                values: new object[] { "APRENDIZAJE", "Aprendizaje" });

            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "cve", "name" },
                values: new object[] { "PSICOLOGIA", "Psicología" });

            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "cve", "name" },
                values: new object[] { "COMUNICACION", "Comunicación" });

            migrationBuilder.UpdateData(
                table: "attention_area",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "cve", "name" },
                values: new object[] { "TRABAJO_SOCIAL", "Trabajo Social" });
        }
    }
}
