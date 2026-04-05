using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using repo.Models;


namespace repo.Context
{
    public class UniversityDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) 
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Настройка TPH (Table Per Hierarchy) для наследования Student
            modelBuilder.Entity<Student>()
                .HasDiscriminator<string>("StudentType")
                .HasValue<FullTimeStudent>("FullTime")
                .HasValue<PartTimeStudent>("PartTime")
                .HasValue<TargetStudent>("Target");
            
            // Связь Student - Group
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey("GroupId")
                .OnDelete(DeleteBehavior.SetNull);
            
            // Связь Student - Department
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey("DepartmentId")
                .OnDelete(DeleteBehavior.SetNull);
            
            // Связь многие ко многим Student - Discipline
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Disciplines)
                .WithMany()
                .UsingEntity(j => j.ToTable("StudentDisciplines"));
            
            // Связь Discipline - Teacher
            modelBuilder.Entity<Discipline>()
                .HasOne(d => d.Teacher)
                .WithMany(t => t.Disciplines)
                .HasForeignKey("TeacherId")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}