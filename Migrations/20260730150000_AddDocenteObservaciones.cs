using Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260730150000_AddDocenteObservaciones")]
    public partial class AddDocenteObservaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE docente_observaciones (
                    id SERIAL PRIMARY KEY,
                    student_id UUID NOT NULL REFERENCES student(id) ON DELETE CASCADE,
                    docente_id UUID NOT NULL REFERENCES "user"(id) ON DELETE RESTRICT,
                    school_year_id INT NULL REFERENCES school_year(id) ON DELETE SET NULL,
                    texto TEXT NOT NULL,
                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
                );

                CREATE INDEX ix_docente_observaciones_student_id_created_at
                    ON docente_observaciones (student_id, created_at DESC);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS docente_observaciones;");
        }
    }
}
