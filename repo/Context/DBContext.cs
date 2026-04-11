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
    public DbSet<StudentDiscipline> StudentDisciplines { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // ========== TPH НАСЛЕДОВАНИЕ ==========
        modelBuilder.Entity<Student>()
            .HasDiscriminator<string>("StudentType")
            .HasValue<FullTimeStudent>("FullTime")
            .HasValue<PartTimeStudent>("PartTime")
            .HasValue<TargetStudent>("Target");
        
        // ========== MANY-TO-MANY ==========
        modelBuilder.Entity<StudentDiscipline>()
            .HasKey(sd => new { sd.StudentId, sd.DisciplineId });
        
        modelBuilder.Entity<StudentDiscipline>()
            .HasOne(sd => sd.Student)
            .WithMany(s => s.StudentDisciplines)
            .HasForeignKey(sd => sd.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<StudentDiscipline>()
            .HasOne(sd => sd.Discipline)
            .WithMany(d => d.StudentDisciplines)
            .HasForeignKey(sd => sd.DisciplineId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // ========== СВЯЗИ ==========
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Discipline>()
            .HasOne(d => d.Teacher)
            .WithMany(t => t.Disciplines)
            .HasForeignKey(d => d.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Group>()
            .HasOne(g => g.Department)
            .WithMany(d => d.Groups)
            .HasForeignKey(g => g.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // ========== ИНДЕКСЫ ==========
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.RecordBookNumber)
            .IsUnique();
        
        modelBuilder.Entity<Student>()
            .HasIndex(s => new { s.LastName, s.FirstName });
        
        modelBuilder.Entity<Discipline>()
            .HasIndex(d => d.Name);
        
        modelBuilder.Entity<Group>()
            .HasIndex(g => g.Name);
        
        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Name);
        
        // ========== НАСТРОЙКА ТИПОВ ==========
        modelBuilder.Entity<StudentDiscipline>()
            .Property(sd => sd.Grade)
            .HasMaxLength(2)
            .IsRequired();
        
        // ========== SEED DATA (расширенные данные) ==========
        // SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // ========== 1. КАФЕДРЫ (10 штук) ==========
        var departments = new List<Department>();
        for (int i = 1; i <= 10; i++)
        {
            departments.Add(new Department 
            { 
                Id = i, 
                Name = $"Факультет {i}", 
                HeadOfDepartment = $"Проф. Иванов И.И.", 
                Phone = $"+7(777)111-{i:D2}{i:D2}", 
                Email = $"dept{i}@university.kz" 
            });
        }
        modelBuilder.Entity<Department>().HasData(departments);
        
        // ========== 2. ГРУППЫ (50 штук) ==========
        var groups = new List<Group>();
        int groupId = 1;
        for (int deptId = 1; deptId <= 10; deptId++)
        {
            for (int course = 1; course <= 5; course++)
            {
                groups.Add(new Group 
                { 
                    Id = groupId++, 
                    Name = $"ГР-{deptId}{course}{Random.Shared.Next(1, 5)}", 
                    YearOfStudy = course, 
                    DepartmentId = deptId 
                });
            }
        }
        modelBuilder.Entity<Group>().HasData(groups);
        
        // ========== 3. ПРЕПОДАВАТЕЛИ (50 штук) ==========
        var teachers = new List<Teacher>();
        var firstNames = new[] { "Иван", "Мария", "Алексей", "Елена", "Дмитрий", "Анна", "Сергей", "Ольга" };
        var lastNames = new[] { "Иванов", "Петрова", "Сидоров", "Смирнова", "Козлов", "Морозова" };
        
        for (int i = 1; i <= 50; i++)
        {
            teachers.Add(new Teacher 
            { 
                Id = i, 
                FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                Patronymic = "Иванович",
                AcademicDegree = "Кандидат наук", 
                Position = "Доцент", 
                Phone = $"+7(701){i:000}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                Email = $"teacher{i}@university.kz" 
            });
        }
        modelBuilder.Entity<Teacher>().HasData(teachers);
        
        // ========== 4. ДИСЦИПЛИНЫ (100 штук) ==========
        var disciplines = new List<Discipline>();
        string[] baseNames = 
        {
            "Программирование", "Базы данных", "Web-разработка", "Алгоритмы", "ООП",
            "Экономика", "Маркетинг", "Финансы", "Бухучет", "Менеджмент",
            "Правоведение", "Уголовное право", "Гражданское право", "Административное право", "Конституционное право",
            "Английский язык", "Немецкий язык", "Французский язык", "Китайский язык", "Латынь",
            "Математика", "Физика", "Химия", "Биология", "География"
        };
        
        for (int i = 1; i <= 100; i++)
        {
            int nameIndex = (i - 1) % baseNames.Length;
            string name = baseNames[nameIndex];
            if (i > baseNames.Length)
            {
                name = $"{baseNames[nameIndex]} (продвинутый)";
            }
            
            disciplines.Add(new Discipline 
            { 
                Id = i, 
                Name = name, 
                Description = $"Изучение {name.ToLower()}", 
                Credits = Random.Shared.Next(3, 7), 
                TeacherId = Random.Shared.Next(1, 51) 
            });
        }
        modelBuilder.Entity<Discipline>().HasData(disciplines);
        
        // ========== 5. СТУДЕНТЫ (800 штук) ==========
        var students = new List<Student>();
        int studentId = 1;
        
        // 500 очников
        for (int i = 1; i <= 500; i++)
        {
            int groupIdForStudent = Random.Shared.Next(1, 51);
            var group = groups.First(g => g.Id == groupIdForStudent);
            students.Add(new FullTimeStudent 
            { 
                Id = studentId, 
                RecordBookNumber = $"2024{studentId:0000}", 
                FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                Patronymic = "Иванович",
                DateOfBirth = new DateTime(Random.Shared.Next(2002, 2007), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), 
                Email = $"student{studentId}@student.kz", 
                Phone = $"+7(700){Random.Shared.Next(100,999):D3}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                Address = $"г. Алматы, ул. Абая, д.{Random.Shared.Next(1,100)}", 
                GroupId = groupIdForStudent, 
                DepartmentId = group.DepartmentId,
                EgeScore = Random.Shared.Next(60, 101), 
                AverageScore = Math.Round(Random.Shared.NextDouble() * 2.5 + 2.5, 2)
            });
            studentId++;
        }
        
        // 200 заочников
        for (int i = 1; i <= 200; i++)
        {
            int groupIdForStudent = Random.Shared.Next(1, 51);
            var group = groups.First(g => g.Id == groupIdForStudent);
            students.Add(new PartTimeStudent 
            { 
                Id = studentId, 
                RecordBookNumber = $"2024{studentId:0000}", 
                FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                Patronymic = "Иванович",
                DateOfBirth = new DateTime(Random.Shared.Next(2000, 2005), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), 
                Email = $"student{studentId}@student.kz", 
                Phone = $"+7(700){Random.Shared.Next(100,999):D3}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                Address = $"г. Алматы, ул. Абая, д.{Random.Shared.Next(1,100)}", 
                GroupId = groupIdForStudent, 
                DepartmentId = group.DepartmentId,
                WorkPlace = "Компания", 
                Position = "Сотрудник", 
                TuitionFee = Random.Shared.Next(300000, 600001)
            });
            studentId++;
        }
        
        // 100 целевиков
        for (int i = 1; i <= 100; i++)
        {
            int groupIdForStudent = Random.Shared.Next(1, 51);
            var group = groups.First(g => g.Id == groupIdForStudent);
            students.Add(new TargetStudent 
            { 
                Id = studentId, 
                RecordBookNumber = $"2024{studentId:0000}", 
                FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                Patronymic = "Иванович",
                DateOfBirth = new DateTime(Random.Shared.Next(2002, 2007), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), 
                Email = $"student{studentId}@student.kz", 
                Phone = $"+7(700){Random.Shared.Next(100,999):D3}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                Address = $"г. Алматы, ул. Абая, д.{Random.Shared.Next(1,100)}", 
                GroupId = groupIdForStudent, 
                DepartmentId = group.DepartmentId,
                TargetCompany = "Компания", 
                TuitionFee = 0
            });
            studentId++;
        }
        
        modelBuilder.Entity<Student>().HasData(students);
        
        // ========== 6. СВЯЗИ СТУДЕНТ-ДИСЦИПЛИНА (~3000 связей) ==========
        var studentDisciplines = new List<StudentDiscipline>();
        string[] grades = { "4", "5", "зачет" };
        
        for (int i = 1; i <= 800; i++)
        {
            int disciplineCount = Random.Shared.Next(4, 7);
            var usedDisciplines = new HashSet<int>();
            
            for (int j = 0; j < disciplineCount; j++)
            {
                int discId;
                do
                {
                    discId = Random.Shared.Next(1, 101);
                } while (usedDisciplines.Contains(discId));
                usedDisciplines.Add(discId);
                
                studentDisciplines.Add(new StudentDiscipline 
                { 
                    StudentId = i, 
                    DisciplineId = discId, 
                    Grade = grades[Random.Shared.Next(grades.Length)],
                    DateReceived = new DateTime(2024, Random.Shared.Next(6, 13), Random.Shared.Next(1, 29))
                });
            }
        }
        modelBuilder.Entity<StudentDiscipline>().HasData(studentDisciplines);
    }
    // Вспомогательные методы
    private string GetRandomLastName()
    {
        string[] lastNames = { "Иванов", "Петров", "Сидоров", "Смирнов", "Козлов", "Морозов", "Новиков", "Волков", "Соколов", "Лебедев" };
        return lastNames[Random.Shared.Next(lastNames.Length)];
    }
    
    private string GetRandomStreet()
    {
        string[] streets = { "Абая", "Жибек жолы", "Толе би", "Самал", "Жандосова", "Розыбакиева", "Сатпаева", "Тимирязева", "Фурманова", "Гоголя" };
        return streets[Random.Shared.Next(streets.Length)];
    }
    
    private int GetRandomNumber(int min, int max)
    {
        return Random.Shared.Next(min, max);
    }
}