using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attention_area",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cve = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attention_area", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "disability",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cve = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_disability", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "education_level",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clave = table.Column<string>(type: "text", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    orden = table.Column<short>(type: "smallint", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_education_level", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clave = table.Column<string>(type: "text", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    permisos = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "school_year",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_year", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    father_last_name = table.Column<string>(type: "text", nullable: false),
                    mother_last_name = table.Column<string>(type: "text", nullable: true),
                    sexo = table.Column<int>(type: "integer", nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    curp = table.Column<string>(type: "text", nullable: true),
                    photo_url = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clave = table.Column<string>(type: "text", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    precio_mensual = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    precio_anual = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    max_usuarios = table.Column<int>(type: "integer", nullable: true),
                    max_alumnos = table.Column<int>(type: "integer", nullable: true),
                    caracteristicas = table.Column<string>(type: "jsonb", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_plan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grade",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero = table.Column<short>(type: "smallint", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    education_level_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grade", x => x.id);
                    table.ForeignKey(
                        name: "fk_grade_education_level_education_level_id",
                        column: x => x.education_level_id,
                        principalTable: "education_level",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attention_mode",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    phase = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attention_mode", x => x.id);
                    table.ForeignKey(
                        name: "fk_attention_mode_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_attention_mode_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_attention_area",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attention_area_id = table.Column<int>(type: "integer", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_attention_area", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_attention_area_attention_area_attention_area_id",
                        column: x => x.attention_area_id,
                        principalTable: "attention_area",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_attention_area_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_attention_area_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_disability",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    external_diagnosis = table.Column<bool>(type: "boolean", nullable: false),
                    document_url = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disability_id = table.Column<int>(type: "integer", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_disability", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_disability_disability_disability_id",
                        column: x => x.disability_id,
                        principalTable: "disability",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_disability_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_disability_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "academy_subscription",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_cuenta = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    periodo = table.Column<int>(type: "integer", nullable: false),
                    referencia_pago = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    plan_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_academy_subscription", x => x.id);
                    table.ForeignKey(
                        name: "fk_academy_subscription_subscription_plan_plan_id",
                        column: x => x.plan_id,
                        principalTable: "subscription_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "individual_subscription",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    periodo = table.Column<int>(type: "integer", nullable: false),
                    referencia_pago = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    plan_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_individual_subscription", x => x.id);
                    table.ForeignKey(
                        name: "fk_individual_subscription_subscription_plan_plan_id",
                        column: x => x.plan_id,
                        principalTable: "subscription_plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school_zone",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<string>(type: "text", nullable: false),
                    cct = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    academy_subscription_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_zone", x => x.id);
                    table.ForeignKey(
                        name: "fk_school_zone_academy_subscription_academy_subscription_id",
                        column: x => x.academy_subscription_id,
                        principalTable: "academy_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_method",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    academy_subscription_id = table.Column<int>(type: "integer", nullable: true),
                    individual_subscription_id = table.Column<int>(type: "integer", nullable: true),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    proveedor = table.Column<int>(type: "integer", nullable: false),
                    referencia_ext = table.Column<string>(type: "text", nullable: true),
                    ultimos_digitos = table.Column<string>(type: "text", nullable: true),
                    marca = table.Column<string>(type: "text", nullable: true),
                    titular = table.Column<string>(type: "text", nullable: true),
                    es_predeterminado = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_method", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_method_academy_subscription_academy_subscription_id",
                        column: x => x.academy_subscription_id,
                        principalTable: "academy_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payment_method_individual_subscription_individual_subscript",
                        column: x => x.individual_subscription_id,
                        principalTable: "individual_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    cct = table.Column<string>(type: "text", nullable: true),
                    turn = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    activa = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    school_zone_id = table.Column<int>(type: "integer", nullable: true),
                    education_level_id = table.Column<int>(type: "integer", nullable: false),
                    academy_subscription_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school", x => x.id);
                    table.ForeignKey(
                        name: "fk_school_academy_subscription_academy_subscription_id",
                        column: x => x.academy_subscription_id,
                        principalTable: "academy_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_school_education_level_education_level_id",
                        column: x => x.education_level_id,
                        principalTable: "education_level",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_school_school_zone_school_zone_id",
                        column: x => x.school_zone_id,
                        principalTable: "school_zone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    password_salt = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    father_last_name = table.Column<string>(type: "text", nullable: false),
                    mother_last_name = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    school_zone_id = table.Column<int>(type: "integer", nullable: true),
                    academy_subscription_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_academy_subscription_academy_subscription_id",
                        column: x => x.academy_subscription_id,
                        principalTable: "academy_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_school_zone_school_zone_id",
                        column: x => x.school_zone_id,
                        principalTable: "school_zone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    academy_subscription_id = table.Column<int>(type: "integer", nullable: true),
                    individual_subscription_id = table.Column<int>(type: "integer", nullable: true),
                    payment_method_id = table.Column<int>(type: "integer", nullable: true),
                    monto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    referencia_ext = table.Column<string>(type: "text", nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    fecha_pago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_mensaje = table.Column<string>(type: "text", nullable: true),
                    metadata_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transaction", x => x.id);
                    table.ForeignKey(
                        name: "fk_transaction_academy_subscription_academy_subscription_id",
                        column: x => x.academy_subscription_id,
                        principalTable: "academy_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transaction_individual_subscription_individual_subscription",
                        column: x => x.individual_subscription_id,
                        principalTable: "individual_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transaction_payment_method_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_method",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    section = table.Column<string>(type: "text", nullable: false),
                    school_id = table.Column<int>(type: "integer", nullable: false),
                    grade_id = table.Column<int>(type: "integer", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_grade_grade_id",
                        column: x => x.grade_id,
                        principalTable: "grade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_group_school_school_id",
                        column: x => x.school_id,
                        principalTable: "school",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_group_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    read = table.Column<bool>(type: "boolean", nullable: false),
                    destination_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tutor",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    complete_name = table.Column<string>(type: "text", nullable: false),
                    parentesco = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tutor", x => x.id);
                    table.ForeignKey(
                        name: "fk_tutor_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tutor_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_school",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_id = table.Column<int>(type: "integer", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_school", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_school_school_school_id",
                        column: x => x.school_id,
                        principalTable: "school",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_school_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_school_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    transaction_id = table.Column<int>(type: "integer", nullable: false),
                    folio = table.Column<string>(type: "text", nullable: true),
                    rfc = table.Column<string>(type: "text", nullable: true),
                    razon_social = table.Column<string>(type: "text", nullable: true),
                    regimen_fiscal = table.Column<string>(type: "text", nullable: true),
                    uso_cfdi = table.Column<string>(type: "text", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    iva = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    archivo_r2key = table.Column<string>(type: "text", nullable: true),
                    archivo_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_transaction_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registration",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ingress_date = table.Column<DateOnly>(type: "date", nullable: false),
                    is_new = table.Column<bool>(type: "boolean", nullable: false),
                    is_tracking = table.Column<bool>(type: "boolean", nullable: false),
                    final_situation = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registration", x => x.id);
                    table.ForeignKey(
                        name: "fk_registration_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registration_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registration_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    es_titular = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    school_year_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_group_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_group_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_group_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "attention_area",
                columns: new[] { "id", "cve", "name" },
                values: new object[,]
                {
                    { 1, "APRENDIZAJE", "Aprendizaje" },
                    { 2, "PSICOLOGIA", "Psicología" },
                    { 3, "COMUNICACION", "Comunicación" },
                    { 4, "TRABAJO_SOCIAL", "Trabajo Social" }
                });

            migrationBuilder.InsertData(
                table: "disability",
                columns: new[] { "id", "cve", "category", "description", "name" },
                values: new object[,]
                {
                    { 1, "INTELECTUAL", 1, null, "Discapacidad Intelectual" },
                    { 2, "MOTRIZ", 1, null, "Discapacidad Motriz" },
                    { 3, "SORDERA", 1, null, "Sordera" },
                    { 4, "HIPOACUSIA", 1, null, "Hipoacusia" },
                    { 5, "CEGUERA", 1, null, "Ceguera" },
                    { 6, "BAJA_VISION", 1, null, "Baja Visión" },
                    { 7, "MULTIPLE", 1, null, "Discapacidad Múltiple" },
                    { 8, "SORDOCEGUERA", 1, null, "Sordoceguera" },
                    { 9, "TEA", 1, null, "Trastorno del Espectro Autista" },
                    { 10, "TDAH", 2, null, "TDAH" },
                    { 11, "APRENDIZAJE", 2, null, "Barreras de Aprendizaje" },
                    { 12, "COMUNICACION", 2, null, "Barreras de Comunicación" },
                    { 13, "CONDUCTA", 2, null, "Barreras de Conducta" },
                    { 14, "AS", 3, null, "Aptitudes Sobresalientes" }
                });

            migrationBuilder.InsertData(
                table: "education_level",
                columns: new[] { "id", "activo", "clave", "nombre", "orden" },
                values: new object[,]
                {
                    { 1, true, "PREESCOLAR", "Preescolar", (short)1 },
                    { 2, true, "PRIMARIA", "Primaria", (short)2 },
                    { 3, true, "SECUNDARIA", "Secundaria", (short)3 },
                    { 4, true, "PREPARATORIA", "Preparatoria", (short)4 }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "clave", "descripcion", "nombre", "permisos" },
                values: new object[,]
                {
                    { 1, "ADMIN", null, "Administrador del sistema", null },
                    { 2, "SUPERVISOR", null, "Supervisor de zona", null },
                    { 3, "DIRECTOR_USAER", null, "Director de USAER", null },
                    { 4, "ESPECIALISTA_COM", null, "Especialista en Comunicación", null },
                    { 5, "ESPECIALISTA_PSI", null, "Especialista en Psicología", null },
                    { 6, "ESPECIALISTA_APR", null, "Especialista en Aprendizaje", null },
                    { 7, "TRABAJO_SOCIAL", null, "Trabajo Social", null },
                    { 8, "DOCENTE", null, "Docente de grupo regular", null },
                    { 9, "TUTOR", null, "Padre / tutor", null },
                    { 10, "ALUMNO", null, "Alumno (autoservicio)", null }
                });

            migrationBuilder.InsertData(
                table: "grade",
                columns: new[] { "id", "education_level_id", "nombre", "numero" },
                values: new object[,]
                {
                    { 1, 1, "Primero", (short)1 },
                    { 2, 1, "Segundo", (short)2 },
                    { 3, 1, "Tercero", (short)3 },
                    { 4, 2, "Primero", (short)1 },
                    { 5, 2, "Segundo", (short)2 },
                    { 6, 2, "Tercero", (short)3 },
                    { 7, 2, "Cuarto", (short)4 },
                    { 8, 2, "Quinto", (short)5 },
                    { 9, 2, "Sexto", (short)6 },
                    { 10, 3, "Primero", (short)1 },
                    { 11, 3, "Segundo", (short)2 },
                    { 12, 3, "Tercero", (short)3 },
                    { 13, 4, "Primer semestre", (short)1 },
                    { 14, 4, "Segundo semestre", (short)2 },
                    { 15, 4, "Tercer semestre", (short)3 },
                    { 16, 4, "Cuarto semestre", (short)4 },
                    { 17, 4, "Quinto semestre", (short)5 },
                    { 18, 4, "Sexto semestre", (short)6 }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "academy_subscription_id", "activo", "avatar_url", "created_at", "email", "father_last_name", "mother_last_name", "name", "password_hash", "password_salt", "phone", "role_id", "school_zone_id", "updated_at" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), null, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@system.com", "Sistema", null, "Admin", "cTZfg3WXU8h6n6cVkemLpgFsbETdN1tsoL3dVM10HuM=", "322JhrUxDTVzC5KijDL+FlE+Zk22My5MRBC89R8noN4=", null, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "ix_academy_subscription_plan_id",
                table: "academy_subscription",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_attention_mode_school_year_id",
                table: "attention_mode",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_attention_mode_student_id_school_year_id_phase_type",
                table: "attention_mode",
                columns: new[] { "student_id", "school_year_id", "phase", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_education_level_clave",
                table: "education_level",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_education_level_orden",
                table: "education_level",
                column: "orden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_grade_education_level_id_numero",
                table: "grade",
                columns: new[] { "education_level_id", "numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_group_grade_id",
                table: "group",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_school_id_grade_id_section_school_year_id",
                table: "group",
                columns: new[] { "school_id", "grade_id", "section", "school_year_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_group_school_year_id",
                table: "group",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_individual_subscription_plan_id",
                table: "individual_subscription",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_folio",
                table: "invoice",
                column: "folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoice_transaction_id",
                table: "invoice",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_user_id",
                table: "notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_method_academy_subscription_id",
                table: "payment_method",
                column: "academy_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_method_individual_subscription_id",
                table: "payment_method",
                column: "individual_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_registration_group_id",
                table: "registration",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_registration_school_year_id",
                table: "registration",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_registration_student_id_school_year_id",
                table: "registration",
                columns: new[] { "student_id", "school_year_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_clave",
                table: "role",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_school_academy_subscription_id",
                table: "school",
                column: "academy_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_cct",
                table: "school",
                column: "cct",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_school_education_level_id",
                table: "school",
                column: "education_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_school_zone_id",
                table: "school",
                column: "school_zone_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_year_name",
                table: "school_year",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_school_zone_academy_subscription_id",
                table: "school_zone",
                column: "academy_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_zone_cct",
                table: "school_zone",
                column: "cct",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_curp",
                table: "student",
                column: "curp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_attention_area_attention_area_id",
                table: "student_attention_area",
                column: "attention_area_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_attention_area_school_year_id",
                table: "student_attention_area",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_attention_area_student_id_attention_area_id_school_",
                table: "student_attention_area",
                columns: new[] { "student_id", "attention_area_id", "school_year_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_disability_disability_id",
                table: "student_disability",
                column: "disability_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_disability_school_year_id",
                table: "student_disability",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_disability_student_id_disability_id_school_year_id",
                table: "student_disability",
                columns: new[] { "student_id", "disability_id", "school_year_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_clave",
                table: "subscription_plan",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transaction_academy_subscription_id",
                table: "transaction",
                column: "academy_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_transaction_individual_subscription_id",
                table: "transaction",
                column: "individual_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_transaction_payment_method_id",
                table: "transaction",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_tutor_student_id",
                table: "tutor",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_tutor_user_id",
                table: "tutor",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_academy_subscription_id",
                table: "user",
                column: "academy_subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_school_zone_id",
                table: "user",
                column: "school_zone_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_group_group_id",
                table: "user_group",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_group_school_year_id",
                table: "user_group",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_group_user_id_group_id_school_year_id",
                table: "user_group",
                columns: new[] { "user_id", "group_id", "school_year_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_school_school_id",
                table: "user_school",
                column: "school_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_school_school_year_id",
                table: "user_school",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_school_user_id_school_id_school_year_id",
                table: "user_school",
                columns: new[] { "user_id", "school_id", "school_year_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attention_mode");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "registration");

            migrationBuilder.DropTable(
                name: "student_attention_area");

            migrationBuilder.DropTable(
                name: "student_disability");

            migrationBuilder.DropTable(
                name: "tutor");

            migrationBuilder.DropTable(
                name: "user_group");

            migrationBuilder.DropTable(
                name: "user_school");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "attention_area");

            migrationBuilder.DropTable(
                name: "disability");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "payment_method");

            migrationBuilder.DropTable(
                name: "grade");

            migrationBuilder.DropTable(
                name: "school");

            migrationBuilder.DropTable(
                name: "school_year");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "individual_subscription");

            migrationBuilder.DropTable(
                name: "education_level");

            migrationBuilder.DropTable(
                name: "school_zone");

            migrationBuilder.DropTable(
                name: "academy_subscription");

            migrationBuilder.DropTable(
                name: "subscription_plan");
        }
    }
}
