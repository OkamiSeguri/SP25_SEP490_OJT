using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<CohortCurriculum> CohortCurriculums { get; set; }
        public DbSet<StudentGrade> StudentGrades { get; set; }
        public virtual DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<OJTProgram> OJTPrograms { get; set; }
        public virtual DbSet<OJTRegistration> OJTRegistrations { get; set; }
        public virtual DbSet<OJTCondition> OJTConditions { get; set; }
        public virtual DbSet<OJTResult> OJTResults { get; set; }
        public virtual DbSet<OJTFeedback> OJTFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CohortCurriculum>()
                .HasIndex(cc => new { cc.Cohort, cc.CurriculumId })
                .IsUnique();

            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.CohortCurriculum)
                .WithMany(cc => cc.StudentProfiles)
                .HasForeignKey(sp => new { sp.Cohort, sp.CurriculumId })
                .HasPrincipalKey(cc => new { cc.Cohort, cc.CurriculumId });

            modelBuilder.Entity<StudentProfile>()
        .HasOne(s => s.User)
        .WithOne(u => u.StudentProfile)
        .HasForeignKey<StudentProfile>(s => s.UserId)
        .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<StudentGrade>()
                .HasKey(sg => new { sg.UserId, sg.CurriculumId });
            modelBuilder.Entity<StudentGrade>()
    .HasOne(sg => sg.User)
    .WithMany()
    .HasForeignKey(sg => sg.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentGrade>()
                .HasOne(sg => sg.Curriculum)
                .WithMany()
                .HasForeignKey(sg => sg.CurriculumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentGrade>()
                .Property(sg => sg.IsPassed)
                .HasComputedColumnSql("CASE WHEN Grade >= 5.0 THEN 1 ELSE 0 END");

            modelBuilder.Entity<OJTFeedback>()
                .HasOne(fb => fb.OJTRegistration)
                .WithMany(r => r.OJTFeedbacks)
                .HasForeignKey(fb => fb.OJTId);

            modelBuilder.Entity<OJTRegistration>()
                .HasOne(r => r.OJTProgram)
                .WithMany(p => p.OJTRegistrations)
                .HasForeignKey(r => r.ProgramId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<OJTRegistration>()
                .HasOne(r => r.Enterprise)
                .WithMany(p => p.OJTRegistrations)
                .HasForeignKey(r => r.EnterpriseId)
                .OnDelete(DeleteBehavior.NoAction); 
        }

        public ApplicationDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(9, 0, 0))); 
        }

    }
}
