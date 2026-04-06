using Microsoft.EntityFrameworkCore;
using repo.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Discipline> Disciplines { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Настройка TPH (Table Per Hierarchy) для Student
        modelBuilder.Entity<Student>()
            .HasDiscriminator<string>("StudentType")
            .HasValue<FullTimeStudent>("FullTime")
            .HasValue<PartTimeStudent>("PartTime")
            .HasValue<TargetStudent>("Target");
        
        // Настройка отношений
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId);
            
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId);
            
        modelBuilder.Entity<Discipline>()
            .HasOne(d => d.Teacher)
            .WithMany(t => t.Disciplines)
            .HasForeignKey(d => d.TeacherId);
    }
}