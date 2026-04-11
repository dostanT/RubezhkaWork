using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace repo.Models
{
    // ==================== БАЗОВЫЕ СУЩНОСТИ ====================
    
    public class Group
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        
        [Range(1, 6)]
        public int YearOfStudy { get; set; }
        
        // Связь с кафедрой
        public int DepartmentId { get; set; }
        
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        
        public List<Student> Students { get; set; } = new();
        
        public Group() { }
        
        public Group(int id, string name, int yearOfStudy, int departmentId)
        {
            Id = id;
            Name = name;
            YearOfStudy = yearOfStudy;
            DepartmentId = departmentId;
        }
    }
    
    public class Department
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(100)]
        public string? HeadOfDepartment { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        public List<Student> Students { get; set; } = new();
        public List<Group> Groups { get; set; } = new();
        
        public Department() { }
        
        public Department(int id, string name, string? headOfDepartment = null, string? phone = null, string? email = null)
        {
            Id = id;
            Name = name;
            HeadOfDepartment = headOfDepartment;
            Phone = phone;
            Email = email;
        }
    }
    
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        public string? Patronymic { get; set; }
        
        [StringLength(100)]
        public string? AcademicDegree { get; set; }
        
        [StringLength(50)]
        public string? Position { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        // НОВЫЕ ПОЛЯ ДЛЯ АВТОРИЗАЦИИ
        [Required]
        [StringLength(50)]
        public string Login { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = null!;
        
        public List<Discipline> Disciplines { get; set; } = new();
        
        public Teacher() { }
        
        public Teacher(int id, string firstName, string lastName, string login, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Login = login;
            Password = password;
        }
        
        public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
    }
    
    public class Discipline
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Range(1, 12)]
        public int Credits { get; set; }
        
        public int? TeacherId { get; set; }
        
        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
        
        // Many-to-Many через StudentDiscipline
        public List<StudentDiscipline> StudentDisciplines { get; set; } = new();
        
        public Discipline() { }
        
        public Discipline(int id, string name, int credits, int? teacherId = null)
        {
            Id = id;
            Name = name;
            Credits = credits;
            TeacherId = teacherId;
        }
    }
    
    // ==================== ПРОМЕЖУТОЧНАЯ ТАБЛИЦА Many-to-Many ====================
    
    public class StudentDiscipline
    {
        [Key]
        [Column(Order = 0)]
        public int StudentId { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int DisciplineId { get; set; }
        
        [Required]
        [StringLength(2)]
        public string Grade { get; set; } = null!; // Оценка студента по дисциплине
        
        [DataType(DataType.Date)]
        public DateTime? DateReceived { get; set; }
        
        // Навигационные свойства
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;
        
        [ForeignKey("DisciplineId")]
        public Discipline Discipline { get; set; } = null!;
    }
    
    // ==================== АБСТРАКТНЫЙ СТУДЕНТ ====================
    
    public abstract class Student
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string RecordBookNumber { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        public string? Patronymic { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        public int? GroupId { get; set; }
        
        [ForeignKey("GroupId")]
        public Group? Group { get; set; }
        
        public int? DepartmentId { get; set; }
        
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        
        // НОВЫЕ ПОЛЯ ДЛЯ АВТОРИЗАЦИИ
        [Required]
        [StringLength(50)]
        public string Login { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = null!;
        
        // Many-to-Many через StudentDiscipline
        public List<StudentDiscipline> StudentDisciplines { get; set; } = new();
        
        protected Student() { }
        
        protected Student(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth, string login, string password)
        {
            RecordBookNumber = recordBookNumber;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Login = login;
            Password = password;
        }
        
        public abstract void PrintInfo();
        
        public virtual void PrintFullInfo()
        {
            Console.WriteLine($"Студент: {LastName} {FirstName}");
            Console.WriteLine($"№ зачетки: {RecordBookNumber}");
            Console.WriteLine($"Дата рождения: {DateOfBirth:dd.MM.yyyy}");
            Console.WriteLine($"Email: {Email ?? "Не указан"}");
            Console.WriteLine($"Телефон: {Phone ?? "Не указан"}");
            Console.WriteLine($"Адрес: {Address ?? "Не указан"}");
            Console.WriteLine($"Группа: {Group?.Name ?? "Не указана"}");
            Console.WriteLine($"Кафедра: {Department?.Name ?? "Не указана"}");
            Console.WriteLine($"Логин: {Login}");
            Console.WriteLine("Дисциплины и оценки:");
            foreach (var sd in StudentDisciplines)
            {
                string teacherName = sd.Discipline.Teacher != null 
                    ? sd.Discipline.Teacher.FullName 
                    : "Не назначен";
                Console.WriteLine($"  - {sd.Discipline.Name} (Кредиты: {sd.Discipline.Credits}, " +
                                $"Оценка: {sd.Grade}, Преподаватель: {teacherName})");
            }
        }
        
        // Вспомогательные методы для работы с дисциплинами
        public void AddDiscipline(Discipline discipline, string grade)
        {
            StudentDisciplines.Add(new StudentDiscipline
            {
                StudentId = this.Id,
                DisciplineId = discipline.Id,
                Grade = grade,
                DateReceived = DateTime.Now
            });
        }
        
        public double GetAverageGrade()
        {
            if (StudentDisciplines.Count == 0) return 0;
            
            var gradeMap = new Dictionary<string, double>
            {
                { "5", 5 }, { "5+", 5 },
                { "4", 4 }, { "4+", 4 },
                { "3", 3 }, { "3+", 3 },
                { "2", 2 }, { "2+", 2 },
                { "зачет", 5 }, { "зачтено", 5 }
            };
            
            double sum = 0;
            foreach (var sd in StudentDisciplines)
            {
                if (gradeMap.ContainsKey(sd.Grade))
                    sum += gradeMap[sd.Grade];
            }
            
            return sum / StudentDisciplines.Count;
        }
    }
    
    // ==================== ОЧНОЕ ОТДЕЛЕНИЕ ====================
    
    public class FullTimeStudent : Student
    {
        [Range(0, 100)]
        public int EgeScore { get; set; }
        
        [Range(0, 5)]
        public double AverageScore { get; set; }
        
        public FullTimeStudent() : base() { }
        
        public FullTimeStudent(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth,
                              int egeScore, double averageScore, string login, string password)
            : base(recordBookNumber, firstName, lastName, dateOfBirth, login, password)
        {
            EgeScore = egeScore;
            AverageScore = averageScore;
        }
        
        public override void PrintInfo()
        {
            Console.WriteLine($"[ОЧНИК] {LastName} {FirstName}, Группа: {Group?.Name ?? "Нет группы"}, " +
                            $"ЕГЭ: {EgeScore}, Средний балл: {AverageScore:F2}");
        }
        
        public override void PrintFullInfo()
        {
            base.PrintFullInfo();
            Console.WriteLine($"ЕГЭ: {EgeScore}");
            Console.WriteLine($"Средний балл: {AverageScore:F2}");
        }
    }
    
    // ==================== ЗАОЧНОЕ ОТДЕЛЕНИЕ ====================
    
    public class PartTimeStudent : Student
    {
        [StringLength(100)]
        public string? WorkPlace { get; set; }
        
        [StringLength(50)]
        public string? Position { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TuitionFee { get; set; }
        
        public PartTimeStudent() : base() { }
        
        public PartTimeStudent(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth,
                              string? workPlace = null, string? position = null, decimal tuitionFee = 0, 
                              string login = "", string password = "")
            : base(recordBookNumber, firstName, lastName, dateOfBirth, login, password)
        {
            WorkPlace = workPlace;
            Position = position;
            TuitionFee = tuitionFee;
        }
        
        public override void PrintInfo()
        {
            Console.WriteLine($"[ЗАОЧНИК] {LastName} {FirstName}, Группа: {Group?.Name ?? "Нет группы"}, " +
                            $"Место работы: {WorkPlace ?? "Не указано"}, Должность: {Position ?? "Не указана"}");
        }
        
        public override void PrintFullInfo()
        {
            base.PrintFullInfo();
            Console.WriteLine($"Место работы: {WorkPlace ?? "Не указано"}");
            Console.WriteLine($"Должность: {Position ?? "Не указана"}");
            Console.WriteLine($"Стоимость обучения: {TuitionFee:C}");
        }
    }
    
    // ==================== ЦЕЛЕВОЕ ОТДЕЛЕНИЕ ====================
    
    public class TargetStudent : Student
    {
        [StringLength(100)]
        public string? TargetCompany { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TuitionFee { get; set; }
        
        public TargetStudent() : base() { }
        
        public TargetStudent(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth,
                            string? targetCompany = null, decimal tuitionFee = 0,
                            string login = "", string password = "")
            : base(recordBookNumber, firstName, lastName, dateOfBirth, login, password)
        {
            TargetCompany = targetCompany;
            TuitionFee = tuitionFee;
        }
        
        public override void PrintInfo()
        {
            Console.WriteLine($"[ЦЕЛЕВИК] {LastName} {FirstName}, Группа: {Group?.Name ?? "Нет группы"}, " +
                            $"Целевое предприятие: {TargetCompany ?? "Не указано"}");
        }
        
        public override void PrintFullInfo()
        {
            base.PrintFullInfo();
            Console.WriteLine($"Целевое предприятие: {TargetCompany ?? "Не указано"}");
            Console.WriteLine($"Стоимость обучения: {TuitionFee:C}");
        }
    }
}