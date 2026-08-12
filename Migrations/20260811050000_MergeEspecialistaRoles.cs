using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class MergeEspecialistaRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La separación en 3 roles (ESPECIALISTA_COM/PSI/APR) nunca tradujo en una
            // diferencia real de permisos: cada endpoint clínico que se restringió a un solo
            // tipo de especialista terminó necesitando abrirse a los 3. Se unifica en un solo
            // rol; la especialidad (para la pantalla de inicio por defecto, no para
            // autorización) pasa a vivir en user.especialidad.
            migrationBuilder.AddColumn<string>(
                name: "especialidad",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                "ALTER TABLE \"user\" ADD CONSTRAINT \"ck_user_especialidad\" " +
                "CHECK (especialidad IN ('PSICOLOGIA', 'COMUNICACION', 'APRENDIZAJE'));");

            migrationBuilder.Sql("UPDATE \"user\" SET especialidad = 'COMUNICACION' WHERE role_id = 4;");
            migrationBuilder.Sql("UPDATE \"user\" SET especialidad = 'PSICOLOGIA' WHERE role_id = 5;");
            migrationBuilder.Sql("UPDATE \"user\" SET especialidad = 'APRENDIZAJE' WHERE role_id = 6;");
            migrationBuilder.Sql("UPDATE \"user\" SET role_id = 5 WHERE role_id IN (4, 6);");

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "clave", "nombre" },
                values: new object[] { "ESPECIALISTA", "Especialista" });

            migrationBuilder.DeleteData(table: "role", keyColumn: "id", keyValue: 4);
            migrationBuilder.DeleteData(table: "role", keyColumn: "id", keyValue: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "clave", "descripcion", "nombre", "permisos" },
                values: new object[,]
                {
                    { 4, "ESPECIALISTA_COM", null, "Especialista en Comunicación", null },
                    { 6, "ESPECIALISTA_APR", null, "Especialista en Aprendizaje", null }
                });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "clave", "nombre" },
                values: new object[] { "ESPECIALISTA_PSI", "Especialista en Psicología" });

            migrationBuilder.Sql("UPDATE \"user\" SET role_id = 4 WHERE especialidad = 'COMUNICACION';");
            migrationBuilder.Sql("UPDATE \"user\" SET role_id = 6 WHERE especialidad = 'APRENDIZAJE';");

            migrationBuilder.Sql("ALTER TABLE \"user\" DROP CONSTRAINT \"ck_user_especialidad\";");

            migrationBuilder.DropColumn(
                name: "especialidad",
                table: "user");
        }
    }
}
