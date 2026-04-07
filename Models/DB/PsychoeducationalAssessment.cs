namespace Models.DB;

public enum assessmentStatus
{
    BORRADOR = 0,
    EN_REVISION = 1,
    FIRMADA = 2,
    ENTREGADA = 3
}

public class PsychoeducationalAssessment
{
    public Guid Id { get; set; }
    public DateOnly EvaluationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    // II. Motivo
    public string? EvaluationReason { get; set; }
    
    // III. Conducta durante evaluación
    public string? EvaluationBehavior { get; set; }
    
    // IV. Antecedentes del desarrollo
    public string? PregnancyHistory { get; set; }
    public string? HereditaryHistory { get; set; }
    public string? MotorDevelopment { get; set; }
    public string? LanguageDevelopment { get; set; }
    public string? MedicalHistory { get; set; }
    public string? SchoolHistory { get; set; }
    public string? FamilySituation { get; set; }
    
    // V. Situación actual
    public string? StudentDescription { get; set; }
    public string? FamilyContext { get; set; }
    public string? SchoolContext { get; set; }
    public string? SocialContext { get; set; }
    public string? PhysicalDevelopment { get; set; }
    public string? CognitiveDevelopment { get; set; }
    public string? SocioAffectiveDevelopment { get; set; }
    public string? LearningEvaluation { get; set; }
    public string? Creativity { get; set; }
    
    // VI. Interpretación de resultados
    public string? ResultsInterpretation { get; set; }
    
    // VII. Conclusiones
    public string? Conclusions { get; set; }
    
    // Meta
    public assessmentStatus Status { get; set; } = assessmentStatus.BORRADOR;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
