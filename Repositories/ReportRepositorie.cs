using System.Data;
using Dapper;
using Data;
using Models.DB;
using Models.Dto;
using Repositories.IRepositories;

namespace Repositories;

public class ReportRepositorie : IReportRepositorie
{
    private readonly IDbConnection _dbConnection;

    public ReportRepositorie(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<StudentDataSheetItemDto>> GetStudentDataSheet(Guid? schoolId, Guid? schoolYearId)
    {
        var sql = """
            SELECT
                s.id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                s.Gender,
                s.birth_date AS BirthDate,
                s.curp,

                sc.id AS SchoolId,
                sc.name AS SchoolName,
                g.id AS GroupId,
                CASE
                    WHEN g.id IS NULL THEN NULL
                    ELSE CONCAT(CAST(g.grade AS integer), '° ', g.section)
                END AS GroupName,
                sy.id AS SchoolYearId,
                sy.name AS SchoolYearName,

                COALESCE(dis.disabilities, ARRAY[]::text[]) AS Disabilities,
                COALESCE(aa.attention_areas, ARRAY[]::text[]) AS AttentionAreas,
                am.attention_mode AS AttentionMode
            FROM "student" s
            LEFT JOIN "registration" r ON r.student_id = s.id
            LEFT JOIN "group" g ON g.id = r.group_id
            LEFT JOIN "school" sc ON sc.id = g.school_id
            LEFT JOIN "school_year" sy ON sy.id = r.school_year_id
            LEFT JOIN LATERAL (
                SELECT ARRAY_AGG(DISTINCT d.name) AS disabilities
                FROM "student_disabilitie" sd
                INNER JOIN "disabilitie" d ON d.id = sd.disability_id
                WHERE sd.student_id = s.id
                  AND (@SchoolYearId IS NULL OR sd.school_year_id = @SchoolYearId)
            ) dis ON TRUE
            LEFT JOIN LATERAL (
                SELECT ARRAY_AGG(DISTINCT a.name) AS attention_areas
                FROM "student_attention_area" saa
                INNER JOIN "attention_area" a ON a.id = saa.attention_area_id
                WHERE saa.student_id = s.id
                  AND (@SchoolYearId IS NULL OR saa.school_year_id = @SchoolYearId)
            ) aa ON TRUE
            LEFT JOIN LATERAL (
                SELECT CONCAT(CAST(am.phase AS integer), '-', CAST(am.type AS integer)) AS attention_mode
                FROM "attention_mode" am
                WHERE am.student_id = s.id
                  AND (@SchoolYearId IS NULL OR am.school_year_id = @SchoolYearId)
                ORDER BY am.id DESC
                LIMIT 1
            ) am ON TRUE
            WHERE (@SchoolId IS NULL OR sc.id = @SchoolId)
              AND (@SchoolYearId IS NULL OR sy.id = @SchoolYearId)
            ORDER BY StudentFullName;
            """;

        return await _dbConnection.QueryAsync<StudentDataSheetItemDto>(sql, new
        {
            SchoolId = schoolId,
            SchoolYearId = schoolYearId
        });
    }

    public async Task<IEnumerable<CIESummaryItemDto>> GetCIESummary(Guid? studentId, Guid? schoolYearId)
    {
        var sql = """
            SELECT
                e.id AS EvaluationId,
                e.student_id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                e.dimension_id AS DimensionId,
                d.name AS DimensionName,
                d.cve AS DimensionCVE,
                COUNT(a.id)::int AS TotalAnswers,
                COALESCE(SUM(CASE WHEN a.achieved = TRUE THEN 1 ELSE 0 END), 0)::int AS AchievedAnswers,
                CASE
                    WHEN COUNT(a.id) = 0 THEN 0
                    ELSE ROUND((COALESCE(SUM(CASE WHEN a.achieved = TRUE THEN 1 ELSE 0 END), 0)::numeric / COUNT(a.id)::numeric) * 100, 2)
                END AS AchievedPercentage
            FROM "cie_evaluation" e
            INNER JOIN "student" s ON s.id = e.student_id
            INNER JOIN "cie_dimension" d ON d.id = e.dimension_id
            LEFT JOIN "cie_answer" a ON a.evaluation_id = e.id
            WHERE (@StudentId IS NULL OR e.student_id = @StudentId)
              AND (@SchoolYearId IS NULL OR e.school_year_id = @SchoolYearId)
            GROUP BY e.id, s.id, d.id
            ORDER BY StudentFullName, d.name;
            """;

        return await _dbConnection.QueryAsync<CIESummaryItemDto>(sql, new
        {
            StudentId = studentId,
            SchoolYearId = schoolYearId
        });
    }

    public async Task<IEnumerable<TEAAlertItemDto>> GetTEAAlerts(Guid? schoolYearId, alertLevel? alertLevel)
    {
        var sql = """
            SELECT
                ts.id AS ScreeningId,
                ts.student_id AS StudentId,
                CONCAT(s.name, ' ', s.father_last_name, ' ', COALESCE(s.mother_last_name, '')) AS StudentFullName,
                ts.school_year_id AS SchoolYearId,
                sy.name AS SchoolYearName,
                ts.screening_date AS ScreeningDate,
                ts.total_score AS TotalScore,
                ts.alert_level AS AlertLevel,
                ts.requires_canalization AS RequiresCanalization
            FROM "tea_screening" ts
            INNER JOIN "student" s ON s.id = ts.student_id
            INNER JOIN "school_year" sy ON sy.id = ts.school_year_id
            WHERE (@SchoolYearId IS NULL OR ts.school_year_id = @SchoolYearId)
              AND (@AlertLevel IS NULL OR ts.alert_level = @AlertLevel)
            ORDER BY ts.total_score DESC NULLS LAST, ts.screening_date DESC;
            """;

        return await _dbConnection.QueryAsync<TEAAlertItemDto>(sql, new
        {
            SchoolYearId = schoolYearId,
            AlertLevel = alertLevel
        });
    }
}