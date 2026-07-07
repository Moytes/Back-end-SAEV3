using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class AddCreadorContenidoRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "clave", "descripcion", "nombre", "permisos" },
                values: new object[] { 11, "CREADOR_CONTENIDO", "Creador de material universal de la plataforma", "Creador de Contenido", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id",
                keyValue: 11);
        }
    }
}
