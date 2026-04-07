using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIAEV2.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attention_area",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cve = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attention_area", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cie_dimension",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cve = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    color_hex = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cie_dimension", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "disabilitie",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cve = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    disability_category = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_disabilitie", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "material_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cve = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "school_year",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_year", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "school_zone",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "text", nullable: false),
                    cct = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_zone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    father_last_name = table.Column<string>(type: "text", nullable: false),
                    mother_last_name = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    curp = table.Column<string>(type: "text", nullable: true),
                    photo_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tea_indicator",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    domain = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    age_range_min = table.Column<short>(type: "smallint", nullable: true),
                    age_range_max = table.Column<short>(type: "smallint", nullable: true),
                    order = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tea_indicator", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cie_indicator",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<short>(type: "smallint", nullable: false),
                    dimension_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cie_indicator", x => x.id);
                    table.ForeignKey(
                        name: "fk_cie_indicator_cie_dimension_dimension_id",
                        column: x => x.dimension_id,
                        principalTable: "cie_dimension",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "school",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    cct = table.Column<string>(type: "text", nullable: true),
                    turn = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    school_zone_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school", x => x.id);
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    father_last_name = table.Column<string>(type: "text", nullable: false),
                    mother_last_name = table.Column<string>(type: "text", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    password_salt = table.Column<string>(type: "text", nullable: false),
                    school_zone_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_school_zone_school_zone_id",
                        column: x => x.school_zone_id,
                        principalTable: "school_zone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attention_mode",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phase = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "psychoeducational_assessment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    evaluation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    evaluation_reason = table.Column<string>(type: "text", nullable: true),
                    evaluation_behavior = table.Column<string>(type: "text", nullable: true),
                    pregnancy_history = table.Column<string>(type: "text", nullable: true),
                    hereditary_history = table.Column<string>(type: "text", nullable: true),
                    motor_development = table.Column<string>(type: "text", nullable: true),
                    language_development = table.Column<string>(type: "text", nullable: true),
                    medical_history = table.Column<string>(type: "text", nullable: true),
                    school_history = table.Column<string>(type: "text", nullable: true),
                    family_situation = table.Column<string>(type: "text", nullable: true),
                    student_description = table.Column<string>(type: "text", nullable: true),
                    family_context = table.Column<string>(type: "text", nullable: true),
                    school_context = table.Column<string>(type: "text", nullable: true),
                    social_context = table.Column<string>(type: "text", nullable: true),
                    physical_development = table.Column<string>(type: "text", nullable: true),
                    cognitive_development = table.Column<string>(type: "text", nullable: true),
                    socio_affective_development = table.Column<string>(type: "text", nullable: true),
                    learning_evaluation = table.Column<string>(type: "text", nullable: true),
                    creativity = table.Column<string>(type: "text", nullable: true),
                    results_interpretation = table.Column<string>(type: "text", nullable: true),
                    conclusions = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_psychoeducational_assessment", x => x.id);
                    table.ForeignKey(
                        name: "fk_psychoeducational_assessment_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_psychoeducational_assessment_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_attention_area",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_required = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attention_area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "student_disabilitie",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_diagnosis = table.Column<int>(type: "integer", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disability_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_disabilitie", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_disabilitie_disabilitie_disability_id",
                        column: x => x.disability_id,
                        principalTable: "disabilitie",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_disabilitie_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_disabilitie_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tutor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complete_name = table.Column<string>(type: "text", nullable: false),
                    parent = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "cie_sub_indicator",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    order = table.Column<short>(type: "smallint", nullable: false),
                    indicator_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cie_sub_indicator", x => x.id);
                    table.ForeignKey(
                        name: "fk_cie_sub_indicator_cie_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "cie_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grade = table.Column<int>(type: "integer", nullable: false),
                    section = table.Column<char>(type: "character(1)", nullable: false),
                    school_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group", x => x.id);
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
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    action = table.Column<string>(type: "text", nullable: false),
                    affected_table = table.Column<string>(type: "text", nullable: true),
                    record_id = table.Column<string>(type: "text", nullable: true),
                    request = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_log_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "canalization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    canalization_date = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    classroom_actions = table.Column<string>(type: "text", nullable: false),
                    received_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attention_area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requester_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_canalization", x => x.id);
                    table.ForeignKey(
                        name: "fk_canalization_attention_area_attention_area_id",
                        column: x => x.attention_area_id,
                        principalTable: "attention_area",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_canalization_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_canalization_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_canalization_user_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_canalization_user_requester_id",
                        column: x => x.requester_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cie_evaluation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    evaluation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    observations = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evaluator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dimension_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cie_evaluation", x => x.id);
                    table.ForeignKey(
                        name: "fk_cie_evaluation_cie_dimension_dimension_id",
                        column: x => x.dimension_id,
                        principalTable: "cie_dimension",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cie_evaluation_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cie_evaluation_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cie_evaluation_user_evaluator_id",
                        column: x => x.evaluator_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "material",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    grade_min = table.Column<short>(type: "smallint", nullable: true),
                    grade_max = table.Column<short>(type: "smallint", nullable: true),
                    content_json = table.Column<string>(type: "text", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    thumbnail_url = table.Column<string>(type: "text", nullable: true),
                    auto_evaluation = table.Column<int>(type: "integer", nullable: false),
                    criteria_json = table.Column<string>(type: "text", nullable: true),
                    published = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    material_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dimension_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material", x => x.id);
                    table.ForeignKey(
                        name: "fk_material_cie_dimension_dimension_id",
                        column: x => x.dimension_id,
                        principalTable: "cie_dimension",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_material_material_type_material_type_id",
                        column: x => x.material_type_id,
                        principalTable: "material_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_material_user_creator_id",
                        column: x => x.creator_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    read = table.Column<int>(type: "integer", nullable: false),
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
                name: "tea_screening",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    screening_date = table.Column<DateOnly>(type: "date", nullable: false),
                    observation_context = table.Column<string>(type: "text", nullable: true),
                    general_observations = table.Column<string>(type: "text", nullable: true),
                    total_score = table.Column<short>(type: "smallint", nullable: true),
                    alert_level = table.Column<int>(type: "integer", nullable: true),
                    requires_canalization = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evaluator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tea_screening", x => x.id);
                    table.ForeignKey(
                        name: "fk_tea_screening_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tea_screening_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tea_screening_user_evaluator_id",
                        column: x => x.evaluator_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_school",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "psycho_bap",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bap_type = table.Column<string>(type: "text", nullable: true),
                    context = table.Column<string>(type: "text", nullable: true),
                    inclusion_indicator = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    psychoeducational_assessment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_psycho_bap", x => x.id);
                    table.ForeignKey(
                        name: "fk_psycho_bap_psychoeducational_assessment_psychoeducational_a",
                        column: x => x.psychoeducational_assessment_id,
                        principalTable: "psychoeducational_assessment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "psycho_collaborator",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_name = table.Column<string>(type: "text", nullable: true),
                    collaborator_role = table.Column<string>(type: "text", nullable: true),
                    digital_signature = table.Column<int>(type: "integer", nullable: false),
                    signature_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    psychoeducational_assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_psycho_collaborator", x => x.id);
                    table.ForeignKey(
                        name: "fk_psycho_collaborator_psychoeducational_assessment_psychoeduc",
                        column: x => x.psychoeducational_assessment_id,
                        principalTable: "psychoeducational_assessment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_psycho_collaborator_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingress_date = table.Column<DateOnly>(type: "date", nullable: false),
                    its_new = table.Column<int>(type: "integer", nullable: false),
                    its_tracking = table.Column<int>(type: "integer", nullable: false),
                    final_situation = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    parameters_json = table.Column<string>(type: "text", nullable: true),
                    content_json = table.Column<string>(type: "text", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    format = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: true),
                    group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false),
                    generated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report", x => x.id);
                    table.ForeignKey(
                        name: "fk_report_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_report_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_report_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_report_user_generated_by_id",
                        column: x => x.generated_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    es_titular = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "cie_answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    achieved = table.Column<bool>(type: "boolean", nullable: true),
                    help_level = table.Column<short>(type: "smallint", nullable: true),
                    response_type = table.Column<int>(type: "integer", nullable: true),
                    observation = table.Column<string>(type: "text", nullable: true),
                    evidence_url = table.Column<string>(type: "text", nullable: true),
                    evaluation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sub_indicator_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cie_answer", x => x.id);
                    table.ForeignKey(
                        name: "fk_cie_answer_cie_evaluation_evaluation_id",
                        column: x => x.evaluation_id,
                        principalTable: "cie_evaluation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cie_answer_cie_sub_indicator_sub_indicator_id",
                        column: x => x.sub_indicator_id,
                        principalTable: "cie_sub_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cie_phonology_answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    functional = table.Column<bool>(type: "boolean", nullable: true),
                    observation_form = table.Column<string>(type: "text", nullable: true),
                    evaluation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sub_indicator_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cie_phonology_answer", x => x.id);
                    table.ForeignKey(
                        name: "fk_cie_phonology_answer_cie_evaluation_evaluation_id",
                        column: x => x.evaluation_id,
                        principalTable: "cie_evaluation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cie_phonology_answer_cie_sub_indicator_sub_indicator_id",
                        column: x => x.sub_indicator_id,
                        principalTable: "cie_sub_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assignment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    instructions = table.Column<string>(type: "text", nullable: true),
                    active = table.Column<int>(type: "integer", nullable: false),
                    material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_year_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assignment", x => x.id);
                    table.ForeignKey(
                        name: "fk_assignment_material_material_id",
                        column: x => x.material_id,
                        principalTable: "material",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assignment_school_year_school_year_id",
                        column: x => x.school_year_id,
                        principalTable: "school_year",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assignment_user_assigned_by_id",
                        column: x => x.assigned_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dialog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    character_json = table.Column<string>(type: "text", nullable: true),
                    scenes_json = table.Column<string>(type: "text", nullable: true),
                    estimated_duration_min = table.Column<short>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    material_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dialog", x => x.id);
                    table.ForeignKey(
                        name: "fk_dialog_material_material_id",
                        column: x => x.material_id,
                        principalTable: "material",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "material_tag",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: false),
                    material_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_tag", x => x.id);
                    table.ForeignKey(
                        name: "fk_material_tag_material_material_id",
                        column: x => x.material_id,
                        principalTable: "material",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tea_answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<short>(type: "smallint", nullable: true),
                    intensity = table.Column<short>(type: "smallint", nullable: true),
                    observation = table.Column<string>(type: "text", nullable: true),
                    screening_id = table.Column<Guid>(type: "uuid", nullable: false),
                    indicator_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tea_answer", x => x.id);
                    table.ForeignKey(
                        name: "fk_tea_answer_tea_indicator_indicator_id",
                        column: x => x.indicator_id,
                        principalTable: "tea_indicator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tea_answer_tea_screening_screening_id",
                        column: x => x.screening_id,
                        principalTable: "tea_screening",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assignment_student",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    response_json = table.Column<string>(type: "text", nullable: true),
                    evidence_urls = table.Column<string[]>(type: "text[]", nullable: true),
                    auto_evaluation_json = table.Column<string>(type: "text", nullable: true),
                    manual_grade_json = table.Column<string>(type: "text", nullable: true),
                    evaluation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    feedback = table.Column<string>(type: "text", nullable: true),
                    assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evaluated_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assignment_student", x => x.id);
                    table.ForeignKey(
                        name: "fk_assignment_student_assignment_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "assignment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_assignment_student_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_assignment_student_user_evaluated_by_id",
                        column: x => x.evaluated_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dialog_progress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_scene = table.Column<short>(type: "smallint", nullable: false),
                    responses_json = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<decimal>(type: "numeric", nullable: true),
                    completed = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dialog_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dialog_progress", x => x.id);
                    table.ForeignKey(
                        name: "fk_dialog_progress_dialog_dialog_id",
                        column: x => x.dialog_id,
                        principalTable: "dialog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dialog_progress_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "attention_area",
                columns: new[] { "id", "cve", "name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "APRENDIZAJE", "Aprendizaje" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "PSICOLOGIA", "Psicología" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "COMUNICACION", "Comunicación" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "TRABAJO_SOCIAL", "Trabajo Social" }
                });

            migrationBuilder.InsertData(
                table: "cie_dimension",
                columns: new[] { "id", "cve", "color_hex", "description", "name", "order" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "FONOLOGIA", "#FF6B6B", null, "Fonología", (short)1 },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "SEMANTICA", "#4ECDC4", null, "Semántica (Contenido)", (short)2 },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "PRAGMATICA", "#45B7D1", null, "Pragmática (Uso)", (short)3 },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "MORFOSINTAXIS", "#96CEB4", null, "Morfosintaxis (Forma)", (short)4 },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "DISCURSO_ORAL", "#FFEAA7", null, "Discursos Orales", (short)5 },
                    { new Guid("30000000-0000-0000-0000-000000000006"), "JUEGO", "#DDA0DD", null, "Juego", (short)6 }
                });

            migrationBuilder.InsertData(
                table: "disabilitie",
                columns: new[] { "id", "cve", "description", "disability_category", "name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "INTELECTUAL", null, 1, "Discapacidad Intelectual" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "MOTRIZ", null, 1, "Discapacidad Motriz" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "SORDERA", null, 1, "Sordera" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "HIPOACUSIA", null, 1, "Hipoacusia" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "CEGUERA", null, 1, "Ceguera" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "BAJA_VISION", null, 1, "Baja Visión" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "MULTIPLE", null, 1, "Discapacidad Múltiple" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "SORDOCEGUERA", null, 1, "Sordoceguera" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "TEA", null, 1, "Trastorno del Espectro Autista" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "TDAH", null, 2, "TDAH" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "APRENDIZAJE", null, 2, "Barreras de Aprendizaje" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "COMUNICACION", null, 2, "Barreras de Comunicación" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "CONDUCTA", null, 2, "Barreras de Conducta" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "AS", null, 3, "Aptitudes Sobresalientes" }
                });

            migrationBuilder.InsertData(
                table: "material_type",
                columns: new[] { "id", "cve", "description", "name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "DIALOGO_ANIMADO", null, "Diálogo animado interactivo" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "ACTIVIDAD", null, "Actividad didáctica" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "JUEGO_DIGITAL", null, "Juego digital educativo" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "IMAGEN", null, "Material visual / imagen" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "AUDIO", null, "Material de audio" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "VIDEO", null, "Video educativo" },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "DOCUMENTO", null, "Documento / ficha de trabajo" }
                });

            migrationBuilder.InsertData(
                table: "tea_indicator",
                columns: new[] { "id", "age_range_max", "age_range_min", "code", "description", "domain", "order" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), (short)144, (short)72, "TEA_CS_01", "Dificultad para iniciar o mantener conversaciones", 0, (short)1 },
                    { new Guid("40000000-0000-0000-0000-000000000002"), (short)144, (short)72, "TEA_CS_02", "Respuestas inusuales en interacciones sociales", 0, (short)2 },
                    { new Guid("40000000-0000-0000-0000-000000000003"), (short)144, (short)72, "TEA_CS_03", "Contacto visual limitado o atípico", 0, (short)3 },
                    { new Guid("40000000-0000-0000-0000-000000000004"), (short)144, (short)72, "TEA_CS_04", "Dificultad para comprender lenguaje no literal (ironía, chistes)", 0, (short)4 },
                    { new Guid("40000000-0000-0000-0000-000000000005"), (short)144, (short)72, "TEA_CS_05", "Dificultad para hacer amigos o mantener relaciones", 0, (short)5 },
                    { new Guid("40000000-0000-0000-0000-000000000006"), (short)144, (short)72, "TEA_CS_06", "Expresión emocional limitada o inadecuada al contexto", 0, (short)6 },
                    { new Guid("40000000-0000-0000-0000-000000000007"), (short)144, (short)72, "TEA_CS_07", "Dificultad para tomar turnos en la conversación", 0, (short)7 },
                    { new Guid("40000000-0000-0000-0000-000000000008"), (short)144, (short)72, "TEA_CS_08", "Prosodia inusual (tono monótono, volumen inadecuado)", 0, (short)8 },
                    { new Guid("40000000-0000-0000-0000-000000000009"), (short)144, (short)72, "TEA_CR_01", "Intereses intensos y restringidos", 1, (short)1 },
                    { new Guid("40000000-0000-0000-0000-00000000000a"), (short)144, (short)72, "TEA_CR_02", "Inflexibilidad ante cambios de rutina", 1, (short)2 },
                    { new Guid("40000000-0000-0000-0000-00000000000b"), (short)144, (short)72, "TEA_CR_03", "Movimientos repetitivos o estereotipados", 1, (short)3 },
                    { new Guid("40000000-0000-0000-0000-00000000000c"), (short)144, (short)72, "TEA_CR_04", "Hiper o hipo reactividad sensorial", 1, (short)4 },
                    { new Guid("40000000-0000-0000-0000-00000000000d"), (short)144, (short)72, "TEA_CR_05", "Adherencia excesiva a reglas o patrones", 1, (short)5 }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "avatar_url", "created_at", "email", "father_last_name", "mother_last_name", "name", "password_hash", "password_salt", "phone_number", "role", "school_zone_id", "status", "updated_at" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@system.com", "Admin", null, "Admin", "cTZfg3WXU8h6n6cVkemLpgFsbETdN1tsoL3dVM10HuM=", "322JhrUxDTVzC5KijDL+FlE+Zk22My5MRBC89R8noN4=", null, 1, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "cie_indicator",
                columns: new[] { "id", "code", "description", "dimension_id", "name", "order" },
                values: new object[,]
                {
                    { new Guid("31000000-0000-0000-0000-000000000001"), "FON_VOC", null, new Guid("30000000-0000-0000-0000-000000000001"), "Realiza vocalizaciones", (short)1 },
                    { new Guid("31000000-0000-0000-0000-000000000002"), "FON_PAL", null, new Guid("30000000-0000-0000-0000-000000000001"), "Produce palabras con:", (short)2 },
                    { new Guid("31000000-0000-0000-0000-000000000003"), "FON_PTO", null, new Guid("30000000-0000-0000-0000-000000000001"), "Punto de articulación", (short)3 },
                    { new Guid("31000000-0000-0000-0000-000000000004"), "FON_SIT", null, new Guid("30000000-0000-0000-0000-000000000001"), "Situación fonológica", (short)4 },
                    { new Guid("31000000-0000-0000-0000-000000000005"), "FON_APA", null, new Guid("30000000-0000-0000-0000-000000000001"), "Aparato fonoarticulador", (short)5 }
                });

            migrationBuilder.InsertData(
                table: "cie_sub_indicator",
                columns: new[] { "id", "code", "description", "indicator_id", "name", "order" },
                values: new object[,]
                {
                    { new Guid("32000000-0000-0000-0000-000000000001"), "FON_VOC_A", null, new Guid("31000000-0000-0000-0000-000000000001"), "Con intención comunicativa", (short)1 },
                    { new Guid("32000000-0000-0000-0000-000000000002"), "FON_PAL_A", null, new Guid("31000000-0000-0000-0000-000000000002"), "Una sílaba", (short)1 },
                    { new Guid("32000000-0000-0000-0000-000000000003"), "FON_PAL_B", null, new Guid("31000000-0000-0000-0000-000000000002"), "Dos sílabas", (short)2 },
                    { new Guid("32000000-0000-0000-0000-000000000004"), "FON_PAL_C", null, new Guid("31000000-0000-0000-0000-000000000002"), "Heterosilábicas", (short)3 },
                    { new Guid("32000000-0000-0000-0000-000000000005"), "FON_PAL_D", null, new Guid("31000000-0000-0000-0000-000000000002"), "Homosilábicas: /r/: tr, br, kr, gr; /l/: bl, pl, kl, fl", (short)4 },
                    { new Guid("32000000-0000-0000-0000-000000000006"), "FON_PAL_E", null, new Guid("31000000-0000-0000-0000-000000000002"), "Combinaciones: /mbr/, /str/", (short)5 },
                    { new Guid("32000000-0000-0000-0000-000000000007"), "FON_PAL_F", null, new Guid("31000000-0000-0000-0000-000000000002"), "Diptongos", (short)6 },
                    { new Guid("32000000-0000-0000-0000-000000000008"), "FON_PTO_A", null, new Guid("31000000-0000-0000-0000-000000000003"), "Vocales (SI al producir 3/5 o más)", (short)1 },
                    { new Guid("32000000-0000-0000-0000-000000000009"), "FON_PTO_B", null, new Guid("31000000-0000-0000-0000-000000000003"), "Velares /k/, /g/, /j/", (short)2 },
                    { new Guid("32000000-0000-0000-0000-00000000000a"), "FON_PTO_C", null, new Guid("31000000-0000-0000-0000-000000000003"), "Bilabiales /p/, /b/, /m/", (short)3 },
                    { new Guid("32000000-0000-0000-0000-00000000000b"), "FON_PTO_D", null, new Guid("31000000-0000-0000-0000-000000000003"), "Alveolares /s/, /l/, /r/, /n/", (short)4 },
                    { new Guid("32000000-0000-0000-0000-00000000000c"), "FON_PTO_E", null, new Guid("31000000-0000-0000-0000-000000000003"), "Palatales /ch/, /ll/, /ñ/", (short)5 },
                    { new Guid("32000000-0000-0000-0000-00000000000d"), "FON_PTO_F", null, new Guid("31000000-0000-0000-0000-000000000003"), "Dentales /d/, /t/", (short)6 },
                    { new Guid("32000000-0000-0000-0000-00000000000e"), "FON_PTO_G", null, new Guid("31000000-0000-0000-0000-000000000003"), "Labiodentales /f/", (short)7 },
                    { new Guid("32000000-0000-0000-0000-00000000000f"), "FON_PTO_H", null, new Guid("31000000-0000-0000-0000-000000000003"), "Laterales /l/", (short)8 },
                    { new Guid("32000000-0000-0000-0000-000000000010"), "FON_PTO_I", null, new Guid("31000000-0000-0000-0000-000000000003"), "Vibrantes /r/, /ṝ/", (short)9 },
                    { new Guid("32000000-0000-0000-0000-000000000011"), "FON_SIT_A", null, new Guid("31000000-0000-0000-0000-000000000004"), "Habla sin omisiones", (short)1 },
                    { new Guid("32000000-0000-0000-0000-000000000012"), "FON_SIT_B", null, new Guid("31000000-0000-0000-0000-000000000004"), "Habla sin adiciones", (short)2 },
                    { new Guid("32000000-0000-0000-0000-000000000013"), "FON_SIT_C", null, new Guid("31000000-0000-0000-0000-000000000004"), "Habla sin sustituciones", (short)3 },
                    { new Guid("32000000-0000-0000-0000-000000000014"), "FON_SIT_D", null, new Guid("31000000-0000-0000-0000-000000000004"), "Habla sin distorsiones", (short)4 },
                    { new Guid("32000000-0000-0000-0000-000000000015"), "FON_SIT_E", null, new Guid("31000000-0000-0000-0000-000000000004"), "Habla sin alteraciones globales", (short)5 },
                    { new Guid("32000000-0000-0000-0000-000000000016"), "FON_SIT_F", null, new Guid("31000000-0000-0000-0000-000000000004"), "Habla sin reducción silábica", (short)6 },
                    { new Guid("32000000-0000-0000-0000-000000000017"), "FON_APA_LENGUA", null, new Guid("31000000-0000-0000-0000-000000000005"), "Lengua", (short)1 },
                    { new Guid("32000000-0000-0000-0000-000000000018"), "FON_APA_FRENILLO", null, new Guid("31000000-0000-0000-0000-000000000005"), "Frenillo lingual", (short)2 },
                    { new Guid("32000000-0000-0000-0000-000000000019"), "FON_APA_LABIOS", null, new Guid("31000000-0000-0000-0000-000000000005"), "Labios", (short)3 },
                    { new Guid("32000000-0000-0000-0000-00000000001a"), "FON_APA_MANDIBULA", null, new Guid("31000000-0000-0000-0000-000000000005"), "Mandíbula", (short)4 },
                    { new Guid("32000000-0000-0000-0000-00000000001b"), "FON_APA_MEJILLAS", null, new Guid("31000000-0000-0000-0000-000000000005"), "Mejillas", (short)5 },
                    { new Guid("32000000-0000-0000-0000-00000000001c"), "FON_APA_DIENTES", null, new Guid("31000000-0000-0000-0000-000000000005"), "Dientes", (short)6 },
                    { new Guid("32000000-0000-0000-0000-00000000001d"), "FON_APA_PALADAR", null, new Guid("31000000-0000-0000-0000-000000000005"), "Paladar duro y velo", (short)7 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_assignment_assigned_by_id",
                table: "assignment",
                column: "assigned_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_material_id",
                table: "assignment",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_school_year_id",
                table: "assignment",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_student_assignment_id",
                table: "assignment_student",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_student_evaluated_by_id",
                table: "assignment_student",
                column: "evaluated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_student_student_id",
                table: "assignment_student",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_attention_mode_school_year_id",
                table: "attention_mode",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_attention_mode_student_id",
                table: "attention_mode",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_user_id",
                table: "audit_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_canalization_attention_area_id",
                table: "canalization",
                column: "attention_area_id");

            migrationBuilder.CreateIndex(
                name: "ix_canalization_receiver_id",
                table: "canalization",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "ix_canalization_requester_id",
                table: "canalization",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "ix_canalization_school_year_id",
                table: "canalization",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_canalization_student_id",
                table: "canalization",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_answer_evaluation_id",
                table: "cie_answer",
                column: "evaluation_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_answer_sub_indicator_id",
                table: "cie_answer",
                column: "sub_indicator_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_evaluation_dimension_id",
                table: "cie_evaluation",
                column: "dimension_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_evaluation_evaluator_id",
                table: "cie_evaluation",
                column: "evaluator_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_evaluation_school_year_id",
                table: "cie_evaluation",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_evaluation_student_id",
                table: "cie_evaluation",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_indicator_dimension_id",
                table: "cie_indicator",
                column: "dimension_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_phonology_answer_evaluation_id",
                table: "cie_phonology_answer",
                column: "evaluation_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_phonology_answer_sub_indicator_id",
                table: "cie_phonology_answer",
                column: "sub_indicator_id");

            migrationBuilder.CreateIndex(
                name: "ix_cie_sub_indicator_indicator_id",
                table: "cie_sub_indicator",
                column: "indicator_id");

            migrationBuilder.CreateIndex(
                name: "ix_dialog_material_id",
                table: "dialog",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "ix_dialog_progress_dialog_id",
                table: "dialog_progress",
                column: "dialog_id");

            migrationBuilder.CreateIndex(
                name: "ix_dialog_progress_student_id",
                table: "dialog_progress",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_school_id",
                table: "group",
                column: "school_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_school_year_id",
                table: "group",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_creator_id",
                table: "material",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_dimension_id",
                table: "material",
                column: "dimension_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_material_type_id",
                table: "material",
                column: "material_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_tag_material_id",
                table: "material_tag",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_user_id",
                table: "notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_psycho_bap_psychoeducational_assessment_id",
                table: "psycho_bap",
                column: "psychoeducational_assessment_id");

            migrationBuilder.CreateIndex(
                name: "ix_psycho_collaborator_psychoeducational_assessment_id",
                table: "psycho_collaborator",
                column: "psychoeducational_assessment_id");

            migrationBuilder.CreateIndex(
                name: "ix_psycho_collaborator_user_id",
                table: "psycho_collaborator",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_psychoeducational_assessment_school_year_id",
                table: "psychoeducational_assessment",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_psychoeducational_assessment_student_id",
                table: "psychoeducational_assessment",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_registration_group_id",
                table: "registration",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_registration_school_year_id",
                table: "registration",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_registration_student_id",
                table: "registration",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_generated_by_id",
                table: "report",
                column: "generated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_group_id",
                table: "report",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_school_year_id",
                table: "report",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_student_id",
                table: "report",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_school_zone_id",
                table: "school",
                column: "school_zone_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_attention_area_attention_area_id",
                table: "student_attention_area",
                column: "attention_area_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_attention_area_school_year_id",
                table: "student_attention_area",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_attention_area_student_id",
                table: "student_attention_area",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_disabilitie_disability_id",
                table: "student_disabilitie",
                column: "disability_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_disabilitie_school_year_id",
                table: "student_disabilitie",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_disabilitie_student_id",
                table: "student_disabilitie",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_tea_answer_indicator_id",
                table: "tea_answer",
                column: "indicator_id");

            migrationBuilder.CreateIndex(
                name: "ix_tea_answer_screening_id",
                table: "tea_answer",
                column: "screening_id");

            migrationBuilder.CreateIndex(
                name: "ix_tea_screening_evaluator_id",
                table: "tea_screening",
                column: "evaluator_id");

            migrationBuilder.CreateIndex(
                name: "ix_tea_screening_school_year_id",
                table: "tea_screening",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_tea_screening_student_id",
                table: "tea_screening",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_tutor_student_id",
                table: "tutor",
                column: "student_id");

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
                name: "ix_user_group_user_id",
                table: "user_group",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_school_school_id",
                table: "user_school",
                column: "school_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_school_school_year_id",
                table: "user_school",
                column: "school_year_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_school_user_id",
                table: "user_school",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assignment_student");

            migrationBuilder.DropTable(
                name: "attention_mode");

            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "canalization");

            migrationBuilder.DropTable(
                name: "cie_answer");

            migrationBuilder.DropTable(
                name: "cie_phonology_answer");

            migrationBuilder.DropTable(
                name: "dialog_progress");

            migrationBuilder.DropTable(
                name: "material_tag");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "psycho_bap");

            migrationBuilder.DropTable(
                name: "psycho_collaborator");

            migrationBuilder.DropTable(
                name: "registration");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "student_attention_area");

            migrationBuilder.DropTable(
                name: "student_disabilitie");

            migrationBuilder.DropTable(
                name: "tea_answer");

            migrationBuilder.DropTable(
                name: "tutor");

            migrationBuilder.DropTable(
                name: "user_group");

            migrationBuilder.DropTable(
                name: "user_school");

            migrationBuilder.DropTable(
                name: "assignment");

            migrationBuilder.DropTable(
                name: "cie_evaluation");

            migrationBuilder.DropTable(
                name: "cie_sub_indicator");

            migrationBuilder.DropTable(
                name: "dialog");

            migrationBuilder.DropTable(
                name: "psychoeducational_assessment");

            migrationBuilder.DropTable(
                name: "attention_area");

            migrationBuilder.DropTable(
                name: "disabilitie");

            migrationBuilder.DropTable(
                name: "tea_indicator");

            migrationBuilder.DropTable(
                name: "tea_screening");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "cie_indicator");

            migrationBuilder.DropTable(
                name: "material");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "school");

            migrationBuilder.DropTable(
                name: "school_year");

            migrationBuilder.DropTable(
                name: "cie_dimension");

            migrationBuilder.DropTable(
                name: "material_type");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "school_zone");
        }
    }
}
