using Microsoft.EntityFrameworkCore;
using Models.DB;

namespace Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Suscripciones
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
    public DbSet<AcademySubscription> AcademySubscription { get; set; }
    public DbSet<IndividualSubscription> IndividualSubscription { get; set; }
    public DbSet<PaymentMethod> PaymentMethod { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<Invoice> Invoice { get; set; }

    // Estructura Académica
    public DbSet<SchoolYear> SchoolYear { get; set; }
    public DbSet<EducationLevel> EducationLevel { get; set; }
    public DbSet<Grade> Grade { get; set; }
    public DbSet<SchoolZone> SchoolZone { get; set; }
    public DbSet<School> School { get; set; }
    public DbSet<Group> Group { get; set; }

    // Usuarios y Roles
    public DbSet<Role> Role { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserGroups> UserGroup { get; set; }
    public DbSet<UserSchools> UserSchool { get; set; }

    // Alumnos
    public DbSet<Student> Student { get; set; }
    public DbSet<Tutor> Tutor { get; set; }
    public DbSet<Registration> Registration { get; set; }

    // Catálogos
    public DbSet<Disability> Disability { get; set; }
    public DbSet<StudentDisability> StudentDisability { get; set; }
    public DbSet<AttentionArea> AttentionArea { get; set; }
    public DbSet<StudentAttentionAreas> StudentAttentionArea { get; set; }
    public DbSet<AttentionMode> AttentionMode { get; set; }

    // Notificaciones
    public DbSet<Notification> Notification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ====================================================================
        // UNIQUE CONSTRAINTS
        // ====================================================================

        modelBuilder.Entity<SubscriptionPlan>()
            .HasIndex(sp => sp.Clave).IsUnique();

        modelBuilder.Entity<EducationLevel>()
            .HasIndex(el => el.Clave).IsUnique();

        modelBuilder.Entity<EducationLevel>()
            .HasIndex(el => el.Orden).IsUnique();

        modelBuilder.Entity<Grade>()
            .HasIndex(g => new { g.EducationLevelId, g.Numero }).IsUnique();

        modelBuilder.Entity<SchoolZone>()
            .HasIndex(sz => sz.CCT).IsUnique();

        modelBuilder.Entity<School>()
            .HasIndex(s => s.CCT).IsUnique();

        modelBuilder.Entity<SchoolYear>()
            .HasIndex(sy => sy.Name).IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Clave).IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.CURP).IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.UserId).IsUnique();

        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.Folio).IsUnique();

        modelBuilder.Entity<Group>()
            .HasIndex(g => new { g.SchoolId, g.GradeId, g.Section, g.SchoolYearId }).IsUnique();

        modelBuilder.Entity<UserGroups>()
            .HasIndex(ug => new { ug.UserId, ug.GroupId, ug.SchoolYearId }).IsUnique();

        modelBuilder.Entity<UserSchools>()
            .HasIndex(us => new { us.UserId, us.SchoolId, us.SchoolYearId }).IsUnique();

        modelBuilder.Entity<Registration>()
            .HasIndex(r => new { r.StudentId, r.SchoolYearId }).IsUnique();

        modelBuilder.Entity<StudentDisability>()
            .HasIndex(sd => new { sd.StudentId, sd.DisabilityId, sd.SchoolYearId }).IsUnique();

        modelBuilder.Entity<StudentAttentionAreas>()
            .HasIndex(saa => new { saa.StudentId, saa.AttentionAreaId, saa.SchoolYearId }).IsUnique();

        modelBuilder.Entity<AttentionMode>()
            .HasIndex(am => new { am.StudentId, am.SchoolYearId, am.Phase, am.Type }).IsUnique();

        // ====================================================================
        // COLUMN CONFIGURATIONS
        // ====================================================================

        modelBuilder.Entity<SubscriptionPlan>(e =>
        {
            e.Property(sp => sp.PrecioMensual).HasPrecision(10, 2);
            e.Property(sp => sp.PrecioAnual).HasPrecision(10, 2);
            e.Property(sp => sp.Caracteristicas).HasColumnType("jsonb");
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Monto).HasPrecision(10, 2);
            e.Property(t => t.MetadataJson).HasColumnType("jsonb");
            e.Property(t => t.Moneda).HasMaxLength(3);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.Property(i => i.Subtotal).HasPrecision(10, 2);
            e.Property(i => i.IVA).HasPrecision(10, 2);
            e.Property(i => i.Total).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Role>()
            .Property(r => r.Permisos).HasColumnType("jsonb");

        modelBuilder.Entity<User>()
            .Property(u => u.Id).HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.Entity<Student>()
            .Property(s => s.Id).HasDefaultValueSql("uuid_generate_v4()");

        // ====================================================================
        // RELATIONSHIPS
        // ====================================================================

        // AcademySubscription → SubscriptionPlan
        modelBuilder.Entity<AcademySubscription>()
            .HasOne(a => a.Plan)
            .WithMany()
            .HasForeignKey(a => a.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // IndividualSubscription → SubscriptionPlan
        modelBuilder.Entity<IndividualSubscription>()
            .HasOne(i => i.Plan)
            .WithMany()
            .HasForeignKey(i => i.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // PaymentMethod → AcademySubscription / IndividualSubscription
        modelBuilder.Entity<PaymentMethod>()
            .HasOne(pm => pm.AcademySubscription)
            .WithMany()
            .HasForeignKey(pm => pm.AcademySubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentMethod>()
            .HasOne(pm => pm.IndividualSubscription)
            .WithMany()
            .HasForeignKey(pm => pm.IndividualSubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transaction → AcademySubscription / IndividualSubscription / PaymentMethod
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.AcademySubscription)
            .WithMany()
            .HasForeignKey(t => t.AcademySubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.IndividualSubscription)
            .WithMany()
            .HasForeignKey(t => t.IndividualSubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.PaymentMethod)
            .WithMany()
            .HasForeignKey(t => t.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        // Invoice → Transaction
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Transaction)
            .WithMany()
            .HasForeignKey(i => i.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Grade → EducationLevel
        modelBuilder.Entity<Grade>()
            .HasOne(g => g.EducationLevel)
            .WithMany()
            .HasForeignKey(g => g.EducationLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        // SchoolZone → AcademySubscription
        modelBuilder.Entity<SchoolZone>()
            .HasOne(sz => sz.AcademySubscription)
            .WithMany()
            .HasForeignKey(sz => sz.AcademySubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // School → SchoolZone, EducationLevel, AcademySubscription
        modelBuilder.Entity<School>()
            .HasOne(s => s.SchoolZone)
            .WithMany(z => z.Schools)
            .HasForeignKey(s => s.SchoolZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<School>()
            .HasOne(s => s.EducationLevel)
            .WithMany()
            .HasForeignKey(s => s.EducationLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<School>()
            .HasOne(s => s.AcademySubscription)
            .WithMany()
            .HasForeignKey(s => s.AcademySubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Group → School, Grade, SchoolYear
        modelBuilder.Entity<Group>()
            .HasOne(g => g.School)
            .WithMany()
            .HasForeignKey(g => g.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Grade)
            .WithMany()
            .HasForeignKey(g => g.GradeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.SchoolYear)
            .WithMany()
            .HasForeignKey(g => g.SchoolYearId)
            .OnDelete(DeleteBehavior.Restrict);

        // User → Role, SchoolZone, AcademySubscription
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.SchoolZone)
            .WithMany()
            .HasForeignKey(u => u.SchoolZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.AcademySubscription)
            .WithMany()
            .HasForeignKey(u => u.AcademySubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserGroups → User, Group, SchoolYear
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

        // UserSchools → User, School, SchoolYear
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

        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.School)
            .WithMany()
            .HasForeignKey(s => s.SchoolId)
            .OnDelete(DeleteBehavior.SetNull);

        // Tutor → Student, User
        modelBuilder.Entity<Tutor>()
            .HasOne(t => t.Student)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tutor>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Registration → Student, Group, SchoolYear
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

        // StudentDisability → Student, Disability, SchoolYear
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

        // StudentAttentionAreas → Student, AttentionArea, SchoolYear
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

        // AttentionMode → Student, SchoolYear
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

        // Notification → User
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ====================================================================
        // SEED DATA
        // ====================================================================

        SeedEducationLevels(modelBuilder);
        SeedGrades(modelBuilder);
        SeedRoles(modelBuilder);
        SeedDisabilities(modelBuilder);
        SeedAttentionAreas(modelBuilder);
        SeedAdminUser(modelBuilder);
    }

    private void SeedEducationLevels(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EducationLevel>().HasData(
            new EducationLevel { Id = 1, Clave = "PREESCOLAR", Nombre = "Preescolar", Orden = 1 },
            new EducationLevel { Id = 2, Clave = "PRIMARIA", Nombre = "Primaria", Orden = 2 },
            new EducationLevel { Id = 3, Clave = "SECUNDARIA", Nombre = "Secundaria", Orden = 3 },
            new EducationLevel { Id = 4, Clave = "PREPARATORIA", Nombre = "Preparatoria", Orden = 4 }
        );
    }

    private void SeedGrades(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>().HasData(
            // Preescolar
            new Grade { Id = 1, EducationLevelId = 1, Numero = 1, Nombre = "Primero" },
            new Grade { Id = 2, EducationLevelId = 1, Numero = 2, Nombre = "Segundo" },
            new Grade { Id = 3, EducationLevelId = 1, Numero = 3, Nombre = "Tercero" },
            // Primaria
            new Grade { Id = 4, EducationLevelId = 2, Numero = 1, Nombre = "Primero" },
            new Grade { Id = 5, EducationLevelId = 2, Numero = 2, Nombre = "Segundo" },
            new Grade { Id = 6, EducationLevelId = 2, Numero = 3, Nombre = "Tercero" },
            new Grade { Id = 7, EducationLevelId = 2, Numero = 4, Nombre = "Cuarto" },
            new Grade { Id = 8, EducationLevelId = 2, Numero = 5, Nombre = "Quinto" },
            new Grade { Id = 9, EducationLevelId = 2, Numero = 6, Nombre = "Sexto" },
            // Secundaria
            new Grade { Id = 10, EducationLevelId = 3, Numero = 1, Nombre = "Primero" },
            new Grade { Id = 11, EducationLevelId = 3, Numero = 2, Nombre = "Segundo" },
            new Grade { Id = 12, EducationLevelId = 3, Numero = 3, Nombre = "Tercero" },
            // Preparatoria
            new Grade { Id = 13, EducationLevelId = 4, Numero = 1, Nombre = "Primer semestre" },
            new Grade { Id = 14, EducationLevelId = 4, Numero = 2, Nombre = "Segundo semestre" },
            new Grade { Id = 15, EducationLevelId = 4, Numero = 3, Nombre = "Tercer semestre" },
            new Grade { Id = 16, EducationLevelId = 4, Numero = 4, Nombre = "Cuarto semestre" },
            new Grade { Id = 17, EducationLevelId = 4, Numero = 5, Nombre = "Quinto semestre" },
            new Grade { Id = 18, EducationLevelId = 4, Numero = 6, Nombre = "Sexto semestre" }
        );
    }

    private void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Clave = "ADMIN", Nombre = "Administrador del sistema" },
            new Role { Id = 2, Clave = "SUPERVISOR", Nombre = "Supervisor de zona" },
            new Role { Id = 3, Clave = "DIRECTOR_USAER", Nombre = "Director de USAER" },
            new Role { Id = 4, Clave = "ESPECIALISTA_COM", Nombre = "Especialista en Comunicación" },
            new Role { Id = 5, Clave = "ESPECIALISTA_PSI", Nombre = "Especialista en Psicología" },
            new Role { Id = 6, Clave = "ESPECIALISTA_APR", Nombre = "Especialista en Aprendizaje" },
            new Role { Id = 7, Clave = "TRABAJO_SOCIAL", Nombre = "Trabajo Social" },
            new Role { Id = 8, Clave = "DOCENTE", Nombre = "Docente de grupo regular" },
            new Role { Id = 9, Clave = "TUTOR", Nombre = "Padre / tutor" },
            new Role { Id = 10, Clave = "ALUMNO", Nombre = "Alumno (autoservicio)" }
        );
    }

    private void SeedDisabilities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disability>().HasData(
            new Disability { Id = 1, CVE = "INTELECTUAL", Name = "Discapacidad Intelectual", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 2, CVE = "MOTRIZ", Name = "Discapacidad Motriz", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 3, CVE = "SORDERA", Name = "Sordera", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 4, CVE = "HIPOACUSIA", Name = "Hipoacusia", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 5, CVE = "CEGUERA", Name = "Ceguera", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 6, CVE = "BAJA_VISION", Name = "Baja Visión", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 7, CVE = "MULTIPLE", Name = "Discapacidad Múltiple", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 8, CVE = "SORDOCEGUERA", Name = "Sordoceguera", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 9, CVE = "TEA", Name = "Trastorno del Espectro Autista", Category = DisabilityCategory.DISCAPACIDAD },
            new Disability { Id = 10, CVE = "TDAH", Name = "TDAH", Category = DisabilityCategory.BAP },
            new Disability { Id = 11, CVE = "APRENDIZAJE", Name = "Barreras de Aprendizaje", Category = DisabilityCategory.BAP },
            new Disability { Id = 12, CVE = "COMUNICACION", Name = "Barreras de Comunicación", Category = DisabilityCategory.BAP },
            new Disability { Id = 13, CVE = "CONDUCTA", Name = "Barreras de Conducta", Category = DisabilityCategory.BAP },
            new Disability { Id = 14, CVE = "AS", Name = "Aptitudes Sobresalientes", Category = DisabilityCategory.AS }
        );
    }

    private void SeedAttentionAreas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttentionArea>().HasData(
            new AttentionArea { Id = 1, CVE = "APRENDIZAJE", Name = "Aprendizaje" },
            new AttentionArea { Id = 2, CVE = "PSICOLOGIA", Name = "Psicología" },
            new AttentionArea { Id = 3, CVE = "COMUNICACION", Name = "Comunicación" },
            new AttentionArea { Id = 4, CVE = "TRABAJO_SOCIAL", Name = "Trabajo Social" }
        );
    }

    private void SeedAdminUser(ModelBuilder modelBuilder)
    {
        DateTime seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                Name = "Admin",
                FatherLastName = "Sistema",
                Email = "admin@system.com",
                PasswordHash = "cTZfg3WXU8h6n6cVkemLpgFsbETdN1tsoL3dVM10HuM=",
                PasswordSalt = "322JhrUxDTVzC5KijDL+FlE+Zk22My5MRBC89R8noN4=",
                RoleId = 1,
                Activo = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}
