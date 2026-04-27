using Microsoft.EntityFrameworkCore;
using Models.DB;

namespace Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Catálogos Base
    public DbSet<SchoolYear> SchoolYear { get; set; }
    public DbSet<SchoolZone> SchoolZone { get; set; }
    public DbSet<School> School { get; set; }
    public DbSet<Group> Group { get; set; }

    // Usuarios
    public DbSet<User> User { get; set; }
    public DbSet<UserGroups> UserGroup { get; set; }
    public DbSet<UserSchools> UserSchool { get; set; }

    // Alumnos
    public DbSet<Student> Student { get; set; }
    public DbSet<Tutor> Tutor { get; set; }
    public DbSet<Registration> Registration { get; set; }

    // Catálogo de Discapacidades y BAP
    public DbSet<Disability> Disabilitie { get; set; }
    public DbSet<StudentDisability> StudentDisabilitie { get; set; }

    // Áreas de Atención
    public DbSet<AttentionArea> AttentionArea { get; set; }
    public DbSet<StudentAttentionAreas> StudentAttentionArea { get; set; }

    // Modalidad de Atención
    public DbSet<AttentionMode> AttentionMode { get; set; }

    // Canalizaciones
    public DbSet<Canalization> Canalization { get; set; }

    // Evaluaciones Psicopedagógicas
    public DbSet<PsychoeducationalAssessment> PsychoeducationalAssessment { get; set; }
    public DbSet<PsychoBAP> PsychoBAP { get; set; }
    public DbSet<PsychoCollaborator> PsychoCollaborator { get; set; }

    // Instrumento CIE - Comunicación
    public DbSet<CIEDimension> CIEDimension { get; set; }
    public DbSet<CIEIndicator> CIEIndicator { get; set; }
    public DbSet<CIESubIndicator> CIESubIndicator { get; set; }
    public DbSet<CIEEvaluation> CIEEvaluation { get; set; }
    public DbSet<CIEAnswer> CIEAnswer { get; set; }
    public DbSet<CIEPhonologyAnswer> CIEPhonologyAnswer { get; set; }

    // Detección TEA
    public DbSet<TEAIndicator> TEAIndicator { get; set; }
    public DbSet<TEAScreening> TEAScreening { get; set; }
    public DbSet<TEAAnswer> TEAAnswer { get; set; }

    // Material Didáctico
    public DbSet<MaterialType> MaterialType { get; set; }
    public DbSet<Material> Material { get; set; }
    public DbSet<MaterialTag> MaterialTag { get; set; }

    // Asignaciones
    public DbSet<Assignment> Assignment { get; set; }
    public DbSet<AssignmentStudent> AssignmentStudent { get; set; }
    // Diálogos
    public DbSet<Dialog> Dialog { get; set; }
    public DbSet<DialogProgress> DialogProgress { get; set; }

    // Reportes y Sistema
    public DbSet<Report> Report { get; set; }
    public DbSet<AuditLog> AuditLog { get; set; }
    public DbSet<Notification> Notification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ====================================================================
        // CONFIGURACIÓN DE RELACIONES
        // ====================================================================

        // User relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.SchoolZone)
            .WithMany()
            .HasForeignKey(u => u.SchoolZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Student)
            .WithMany()
            .HasForeignKey(u => u.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        // School relationships
        modelBuilder.Entity<School>()
            .HasOne(s => s.SchoolZone)
            .WithMany(z => z.Schools)
            .HasForeignKey(s => s.SchoolZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Group relationships
        modelBuilder.Entity<Group>()
            .HasOne(g => g.School)
            .WithMany()
            .HasForeignKey(g => g.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.SchoolYear)
            .WithMany()
            .HasForeignKey(g => g.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserGroups relationships
        modelBuilder.Entity<UserGroups>()
            .HasOne(ug => ug.User)
            .WithMany()
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserGroups>()
            .HasOne(ug => ug.Group)
            .WithMany()
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserGroups>()
            .HasOne(ug => ug.SchoolYear)
            .WithMany()
            .HasForeignKey(ug => ug.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserSchools relationships
        modelBuilder.Entity<UserSchools>()
            .HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSchools>()
            .HasOne(us => us.School)
            .WithMany()
            .HasForeignKey(us => us.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSchools>()
            .HasOne(us => us.SchoolYear)
            .WithMany()
            .HasForeignKey(us => us.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tutor relationships
        modelBuilder.Entity<Tutor>()
            .HasOne(t => t.Student)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Registration relationships
        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Group)
            .WithMany()
            .HasForeignKey(r => r.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.SchoolYear)
            .WithMany()
            .HasForeignKey(r => r.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentDisability relationships
        modelBuilder.Entity<StudentDisability>()
            .HasOne(sd => sd.Student)
            .WithMany()
            .HasForeignKey(sd => sd.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentDisability>()
            .HasOne(sd => sd.Disability)
            .WithMany()
            .HasForeignKey(sd => sd.DisabilityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentDisability>()
            .HasOne(sd => sd.SchoolYear)
            .WithMany()
            .HasForeignKey(sd => sd.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentAttentionAreas relationships
        modelBuilder.Entity<StudentAttentionAreas>()
            .HasOne(saa => saa.Student)
            .WithMany()
            .HasForeignKey(saa => saa.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentAttentionAreas>()
            .HasOne(saa => saa.AttentionArea)
            .WithMany()
            .HasForeignKey(saa => saa.AttentionAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentAttentionAreas>()
            .HasOne(saa => saa.SchoolYear)
            .WithMany()
            .HasForeignKey(saa => saa.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // AttentionMode relationships
        modelBuilder.Entity<AttentionMode>()
            .HasOne(am => am.Student)
            .WithMany()
            .HasForeignKey(am => am.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AttentionMode>()
            .HasOne(am => am.SchoolYear)
            .WithMany()
            .HasForeignKey(am => am.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // Canalization relationships
        modelBuilder.Entity<Canalization>()
            .HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Canalization>()
            .HasOne(c => c.SchoolYear)
            .WithMany()
            .HasForeignKey(c => c.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Canalization>()
            .HasOne(c => c.AttentionArea)
            .WithMany()
            .HasForeignKey(c => c.AttentionAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Canalization>()
            .HasOne(c => c.Requester)
            .WithMany()
            .HasForeignKey(c => c.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Canalization>()
            .HasOne(c => c.Receiver)
            .WithMany()
            .HasForeignKey(c => c.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // PsychoeducationalAssessment relationships
        modelBuilder.Entity<PsychoeducationalAssessment>()
            .HasOne(pa => pa.Student)
            .WithMany()
            .HasForeignKey(pa => pa.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PsychoeducationalAssessment>()
            .HasOne(pa => pa.SchoolYear)
            .WithMany()
            .HasForeignKey(pa => pa.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // PsychoBAP relationships
        modelBuilder.Entity<PsychoBAP>()
            .HasOne(pb => pb.PsychoeducationalAssessment)
            .WithMany()
            .HasForeignKey(pb => pb.PsychoeducationalAssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // PsychoCollaborator relationships
        modelBuilder.Entity<PsychoCollaborator>()
            .HasOne(pc => pc.PsychoeducationalAssessment)
            .WithMany()
            .HasForeignKey(pc => pc.PsychoeducationalAssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PsychoCollaborator>()
            .HasOne(pc => pc.User)
            .WithMany()
            .HasForeignKey(pc => pc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // CIEIndicator relationships
        modelBuilder.Entity<CIEIndicator>()
            .HasOne(ci => ci.Dimension)
            .WithMany()
            .HasForeignKey(ci => ci.DimensionId)
            .OnDelete(DeleteBehavior.Cascade);

        // CIESubIndicator relationships
        modelBuilder.Entity<CIESubIndicator>()
            .HasOne(csi => csi.Indicator)
            .WithMany()
            .HasForeignKey(csi => csi.IndicatorId)
            .OnDelete(DeleteBehavior.Cascade);

        // CIEEvaluation relationships
        modelBuilder.Entity<CIEEvaluation>()
            .HasOne(ce => ce.Student)
            .WithMany()
            .HasForeignKey(ce => ce.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CIEEvaluation>()
            .HasOne(ce => ce.Evaluator)
            .WithMany()
            .HasForeignKey(ce => ce.EvaluatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CIEEvaluation>()
            .HasOne(ce => ce.SchoolYear)
            .WithMany()
            .HasForeignKey(ce => ce.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CIEEvaluation>()
            .HasOne(ce => ce.Dimension)
            .WithMany()
            .HasForeignKey(ce => ce.DimensionId)
            .OnDelete(DeleteBehavior.Restrict);

        // CIEAnswer relationships
        modelBuilder.Entity<CIEAnswer>()
            .HasOne(ca => ca.Evaluation)
            .WithMany()
            .HasForeignKey(ca => ca.EvaluationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CIEAnswer>()
            .HasOne(ca => ca.SubIndicator)
            .WithMany()
            .HasForeignKey(ca => ca.SubIndicatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // CIEPhonologyAnswer relationships
        modelBuilder.Entity<CIEPhonologyAnswer>()
            .HasOne(cpa => cpa.Evaluation)
            .WithMany()
            .HasForeignKey(cpa => cpa.EvaluationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CIEPhonologyAnswer>()
            .HasOne(cpa => cpa.SubIndicator)
            .WithMany()
            .HasForeignKey(cpa => cpa.SubIndicatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // TEAScreening relationships
        modelBuilder.Entity<TEAScreening>()
            .HasOne(ts => ts.Student)
            .WithMany()
            .HasForeignKey(ts => ts.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TEAScreening>()
            .HasOne(ts => ts.Evaluator)
            .WithMany()
            .HasForeignKey(ts => ts.EvaluatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TEAScreening>()
            .HasOne(ts => ts.SchoolYear)
            .WithMany()
            .HasForeignKey(ts => ts.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // TEAAnswer relationships
        modelBuilder.Entity<TEAAnswer>()
            .HasOne(ta => ta.Screening)
            .WithMany()
            .HasForeignKey(ta => ta.ScreeningId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TEAAnswer>()
            .HasOne(ta => ta.Indicator)
            .WithMany()
            .HasForeignKey(ta => ta.IndicatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Material relationships
        modelBuilder.Entity<Material>()
            .HasOne(m => m.Creator)
            .WithMany()
            .HasForeignKey(m => m.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Material>()
            .HasOne(m => m.MaterialType)
            .WithMany()
            .HasForeignKey(m => m.MaterialTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Material>()
            .HasOne(m => m.Dimension)
            .WithMany()
            .HasForeignKey(m => m.DimensionId)
            .OnDelete(DeleteBehavior.Restrict);

        // MaterialTag relationships
        modelBuilder.Entity<MaterialTag>()
            .HasOne(mt => mt.Material)
            .WithMany()
            .HasForeignKey(mt => mt.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        // Assignment relationships
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Material)
            .WithMany()
            .HasForeignKey(a => a.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.AssignedBy)
            .WithMany()
            .HasForeignKey(a => a.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.SchoolYear)
            .WithMany()
            .HasForeignKey(a => a.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // AssignmentStudent relationships
        modelBuilder.Entity<AssignmentStudent>()
            .HasOne(ast => ast.Assignment)
            .WithMany()
            .HasForeignKey(ast => ast.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignmentStudent>()
            .HasOne(ast => ast.Student)
            .WithMany()
            .HasForeignKey(ast => ast.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignmentStudent>()
            .HasOne(ast => ast.EvaluatedBy)
            .WithMany()
            .HasForeignKey(ast => ast.EvaluatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Dialog relationships
        modelBuilder.Entity<Dialog>()
            .HasOne(d => d.Material)
            .WithMany()
            .HasForeignKey(d => d.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        // DialogProgress relationships
        modelBuilder.Entity<DialogProgress>()
            .HasOne(dp => dp.Dialog)
            .WithMany()
            .HasForeignKey(dp => dp.DialogId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DialogProgress>()
            .HasOne(dp => dp.Student)
            .WithMany()
            .HasForeignKey(dp => dp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Report relationships
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Group)
            .WithMany()
            .HasForeignKey(r => r.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.SchoolYear)
            .WithMany()
            .HasForeignKey(r => r.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.GeneratedBy)
            .WithMany()
            .HasForeignKey(r => r.GeneratedById)
            .OnDelete(DeleteBehavior.Restrict);

        // AuditLog relationships
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notification relationships
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ====================================================================
        // SEEDING DE DATOS
        // ====================================================================

        // Seed: Disabilities
        SeedDisabilities(modelBuilder);

        // Seed: Attention Areas
        SeedAttentionAreas(modelBuilder);

        // Seed: Material Types
        SeedMaterialTypes(modelBuilder);

        // Seed: CIE Dimensions
        SeedCIEDimensions(modelBuilder);

        // Seed: CIE Indicators (Fonología)
        SeedCIEIndicators(modelBuilder);

        // Seed: CIE SubIndicators (Fonología)
        SeedCIESubIndicators(modelBuilder);

        // Seed: TEA Indicators
        SeedTEAIndicators(modelBuilder);

        // Seed: Users
        SeedUsers(modelBuilder);
    }

    // ====================================================================
    // MÉTODOS DE SEEDING
    // ====================================================================

    private void SeedDisabilities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disability>().HasData(
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), CVE = "INTELECTUAL", Name = "Discapacidad Intelectual", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), CVE = "MOTRIZ", Name = "Discapacidad Motriz", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), CVE = "SORDERA", Name = "Sordera", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), CVE = "HIPOACUSIA", Name = "Hipoacusia", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), CVE = "CEGUERA", Name = "Ceguera", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), CVE = "BAJA_VISION", Name = "Baja Visión", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), CVE = "MULTIPLE", Name = "Discapacidad Múltiple", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), CVE = "SORDOCEGUERA", Name = "Sordoceguera", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), CVE = "TEA", Name = "Trastorno del Espectro Autista", DisabilityCategory = disabilitiesCategory.DISCAPACIDAD },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-00000000000A"), CVE = "TDAH", Name = "TDAH", DisabilityCategory = disabilitiesCategory.BAP },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-00000000000B"), CVE = "APRENDIZAJE", Name = "Barreras de Aprendizaje", DisabilityCategory = disabilitiesCategory.BAP },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-00000000000C"), CVE = "COMUNICACION", Name = "Barreras de Comunicación", DisabilityCategory = disabilitiesCategory.BAP },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-00000000000D"), CVE = "CONDUCTA", Name = "Barreras de Conducta", DisabilityCategory = disabilitiesCategory.BAP },
            new Disability { Id = Guid.Parse("00000000-0000-0000-0000-00000000000E"), CVE = "AS", Name = "Aptitudes Sobresalientes", DisabilityCategory = disabilitiesCategory.AS }
        );
    }

    private void SeedAttentionAreas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttentionArea>().HasData(
            new AttentionArea { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), CVE = "APRENDIZAJE", Name = "Aprendizaje" },
            new AttentionArea { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), CVE = "PSICOLOGIA", Name = "Psicología" },
            new AttentionArea { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), CVE = "COMUNICACION", Name = "Comunicación" },
            new AttentionArea { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), CVE = "TRABAJO_SOCIAL", Name = "Trabajo Social" }
        );
    }

    private void SeedMaterialTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialType>().HasData(
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), CVE = "DIALOGO_ANIMADO", Name = "Diálogo animado interactivo" },
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), CVE = "ACTIVIDAD", Name = "Actividad didáctica" },
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), CVE = "JUEGO_DIGITAL", Name = "Juego digital educativo" },
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), CVE = "IMAGEN", Name = "Material visual / imagen" },
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), CVE = "AUDIO", Name = "Material de audio" },
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), CVE = "VIDEO", Name = "Video educativo" },
            new MaterialType { Id = Guid.Parse("20000000-0000-0000-0000-000000000007"), CVE = "DOCUMENTO", Name = "Documento / ficha de trabajo" }
        );
    }

    private void SeedCIEDimensions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CIEDimension>().HasData(
            new CIEDimension { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), CVE = "FONOLOGIA", Name = "Fonología", ColorHex = "#FF6B6B", Order = 1 },
            new CIEDimension { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), CVE = "SEMANTICA", Name = "Semántica (Contenido)", ColorHex = "#4ECDC4", Order = 2 },
            new CIEDimension { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), CVE = "PRAGMATICA", Name = "Pragmática (Uso)", ColorHex = "#45B7D1", Order = 3 },
            new CIEDimension { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), CVE = "MORFOSINTAXIS", Name = "Morfosintaxis (Forma)", ColorHex = "#96CEB4", Order = 4 },
            new CIEDimension { Id = Guid.Parse("30000000-0000-0000-0000-000000000005"), CVE = "DISCURSO_ORAL", Name = "Discursos Orales", ColorHex = "#FFEAA7", Order = 5 },
            new CIEDimension { Id = Guid.Parse("30000000-0000-0000-0000-000000000006"), CVE = "JUEGO", Name = "Juego", ColorHex = "#DDA0DD", Order = 6 }
        );
    }

    private void SeedCIEIndicators(ModelBuilder modelBuilder)
    {
        var fonologiaId = Guid.Parse("30000000-0000-0000-0000-000000000001");

        modelBuilder.Entity<CIEIndicator>().HasData(
            new CIEIndicator { Id = Guid.Parse("31000000-0000-0000-0000-000000000001"), DimensionId = fonologiaId, Code = "FON_VOC", Name = "Realiza vocalizaciones", Order = 1 },
            new CIEIndicator { Id = Guid.Parse("31000000-0000-0000-0000-000000000002"), DimensionId = fonologiaId, Code = "FON_PAL", Name = "Produce palabras con:", Order = 2 },
            new CIEIndicator { Id = Guid.Parse("31000000-0000-0000-0000-000000000003"), DimensionId = fonologiaId, Code = "FON_PTO", Name = "Punto de articulación", Order = 3 },
            new CIEIndicator { Id = Guid.Parse("31000000-0000-0000-0000-000000000004"), DimensionId = fonologiaId, Code = "FON_SIT", Name = "Situación fonológica", Order = 4 },
            new CIEIndicator { Id = Guid.Parse("31000000-0000-0000-0000-000000000005"), DimensionId = fonologiaId, Code = "FON_APA", Name = "Aparato fonoarticulador", Order = 5 }
        );
    }

    private void SeedCIESubIndicators(ModelBuilder modelBuilder)
    {
        // Sub-indicadores para FON_VOC
        modelBuilder.Entity<CIESubIndicator>().HasData(
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000001"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000001"), Code = "FON_VOC_A", Name = "Con intención comunicativa", Order = 1 }
        );

        // Sub-indicadores para FON_PAL
        modelBuilder.Entity<CIESubIndicator>().HasData(
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000002"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000002"), Code = "FON_PAL_A", Name = "Una sílaba", Order = 1 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000003"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000002"), Code = "FON_PAL_B", Name = "Dos sílabas", Order = 2 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000004"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000002"), Code = "FON_PAL_C", Name = "Heterosilábicas", Order = 3 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000005"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000002"), Code = "FON_PAL_D", Name = "Homosilábicas: /r/: tr, br, kr, gr; /l/: bl, pl, kl, fl", Order = 4 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000006"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000002"), Code = "FON_PAL_E", Name = "Combinaciones: /mbr/, /str/", Order = 5 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000007"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000002"), Code = "FON_PAL_F", Name = "Diptongos", Order = 6 }
        );

        // Sub-indicadores para FON_PTO (Punto de articulación)
        modelBuilder.Entity<CIESubIndicator>().HasData(
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000008"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_A", Name = "Vocales (SI al producir 3/5 o más)", Order = 1 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000009"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_B", Name = "Velares /k/, /g/, /j/", Order = 2 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000000A"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_C", Name = "Bilabiales /p/, /b/, /m/", Order = 3 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000000B"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_D", Name = "Alveolares /s/, /l/, /r/, /n/", Order = 4 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000000C"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_E", Name = "Palatales /ch/, /ll/, /ñ/", Order = 5 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000000D"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_F", Name = "Dentales /d/, /t/", Order = 6 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000000E"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_G", Name = "Labiodentales /f/", Order = 7 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000000F"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_H", Name = "Laterales /l/", Order = 8 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000010"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000003"), Code = "FON_PTO_I", Name = "Vibrantes /r/, /ṝ/", Order = 9 }
        );

        // Sub-indicadores para FON_SIT (Situación fonológica)
        modelBuilder.Entity<CIESubIndicator>().HasData(
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000011"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000004"), Code = "FON_SIT_A", Name = "Habla sin omisiones", Order = 1 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000012"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000004"), Code = "FON_SIT_B", Name = "Habla sin adiciones", Order = 2 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000013"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000004"), Code = "FON_SIT_C", Name = "Habla sin sustituciones", Order = 3 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000014"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000004"), Code = "FON_SIT_D", Name = "Habla sin distorsiones", Order = 4 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000015"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000004"), Code = "FON_SIT_E", Name = "Habla sin alteraciones globales", Order = 5 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000016"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000004"), Code = "FON_SIT_F", Name = "Habla sin reducción silábica", Order = 6 }
        );

        // Sub-indicadores para FON_APA (Aparato fonoarticulador)
        modelBuilder.Entity<CIESubIndicator>().HasData(
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000017"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_LENGUA", Name = "Lengua", Order = 1 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000018"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_FRENILLO", Name = "Frenillo lingual", Order = 2 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-000000000019"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_LABIOS", Name = "Labios", Order = 3 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000001A"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_MANDIBULA", Name = "Mandíbula", Order = 4 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000001B"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_MEJILLAS", Name = "Mejillas", Order = 5 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000001C"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_DIENTES", Name = "Dientes", Order = 6 },
            new CIESubIndicator { Id = Guid.Parse("32000000-0000-0000-0000-00000000001D"), IndicatorId = Guid.Parse("31000000-0000-0000-0000-000000000005"), Code = "FON_APA_PALADAR", Name = "Paladar duro y velo", Order = 7 }
        );
    }

    private void SeedTEAIndicators(ModelBuilder modelBuilder)
    {
        // Indicadores de Comunicación Social
        modelBuilder.Entity<TEAIndicator>().HasData(
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_01", Description = "Dificultad para iniciar o mantener conversaciones", AgeRangeMin = 72, AgeRangeMax = 144, Order = 1 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_02", Description = "Respuestas inusuales en interacciones sociales", AgeRangeMin = 72, AgeRangeMax = 144, Order = 2 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_03", Description = "Contacto visual limitado o atípico", AgeRangeMin = 72, AgeRangeMax = 144, Order = 3 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_04", Description = "Dificultad para comprender lenguaje no literal (ironía, chistes)", AgeRangeMin = 72, AgeRangeMax = 144, Order = 4 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_05", Description = "Dificultad para hacer amigos o mantener relaciones", AgeRangeMin = 72, AgeRangeMax = 144, Order = 5 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000006"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_06", Description = "Expresión emocional limitada o inadecuada al contexto", AgeRangeMin = 72, AgeRangeMax = 144, Order = 6 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000007"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_07", Description = "Dificultad para tomar turnos en la conversación", AgeRangeMin = 72, AgeRangeMax = 144, Order = 7 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000008"), Domain = teaDomain.COMUNICACION_SOCIAL, Code = "TEA_CS_08", Description = "Prosodia inusual (tono monótono, volumen inadecuado)", AgeRangeMin = 72, AgeRangeMax = 144, Order = 8 }
        );

        // Indicadores de Conducta Repetitiva
        modelBuilder.Entity<TEAIndicator>().HasData(
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-000000000009"), Domain = teaDomain.CONDUCTA_REPETITIVA, Code = "TEA_CR_01", Description = "Intereses intensos y restringidos", AgeRangeMin = 72, AgeRangeMax = 144, Order = 1 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-00000000000A"), Domain = teaDomain.CONDUCTA_REPETITIVA, Code = "TEA_CR_02", Description = "Inflexibilidad ante cambios de rutina", AgeRangeMin = 72, AgeRangeMax = 144, Order = 2 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-00000000000B"), Domain = teaDomain.CONDUCTA_REPETITIVA, Code = "TEA_CR_03", Description = "Movimientos repetitivos o estereotipados", AgeRangeMin = 72, AgeRangeMax = 144, Order = 3 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-00000000000C"), Domain = teaDomain.CONDUCTA_REPETITIVA, Code = "TEA_CR_04", Description = "Hiper o hipo reactividad sensorial", AgeRangeMin = 72, AgeRangeMax = 144, Order = 4 },
            new TEAIndicator { Id = Guid.Parse("40000000-0000-0000-0000-00000000000D"), Domain = teaDomain.CONDUCTA_REPETITIVA, Code = "TEA_CR_05", Description = "Adherencia excesiva a reglas o patrones", AgeRangeMin = 72, AgeRangeMax = 144, Order = 5 }
        );
    }

    private void SeedUsers(ModelBuilder modelBuilder)
    {
        // Definimos la fecha una sola vez para que coincida exactamente
        DateTime seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                Name = "Admin",
                FatherLastName = "Admin",
                Email = "admin@system.com",
                // contraseña: Admin123!
                PasswordHash = "cTZfg3WXU8h6n6cVkemLpgFsbETdN1tsoL3dVM10HuM=",
                PasswordSalt = "322JhrUxDTVzC5KijDL+FlE+Zk22My5MRBC89R8noN4=",
                Role = UserRole.ADMIN,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}
