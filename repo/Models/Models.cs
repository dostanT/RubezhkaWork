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
        
        public List<Student> Students { get; set; } = new();
        
        public Group() { }
        
        public Group(int id, string name, int yearOfStudy)
        {
            Id = id;
            Name = name;
            YearOfStudy = yearOfStudy;
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
        public string HeadOfDepartment { get; set; } = null!;
        
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = null!;
        
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;
        
        public List<Student> Students { get; set; } = new();
        
        public Department() { }
        
        public Department(int id, string name, string headOfDepartment, string phone, string email)
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
        public string? Patronymic { get; set; } = null!; 
        
        [StringLength(100)]
        public string AcademicDegree { get; set; } = null!;
        
        [StringLength(50)]
        public string Position { get; set; } = null!;
        
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = null!;
        
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;
        
        public List<Discipline> Disciplines { get; set; } = new();
        
        public Teacher() { }
        
        public Teacher(int id, string firstName, string lastName, string academicDegree, string position, string phone, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            AcademicDegree = academicDegree;
            Position = position;
            Phone = phone;
            Email = email;
        }
        
        public string FullName => $"{LastName} {FirstName}";
    }
    
    public class Discipline
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(500)]
        public string Description { get; set; } = null!;
        
        [Range(1, 12)]
        public int Credits { get; set; }
        
        public int? TeacherId { get; set; }
        
        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
        
        public string Grade { get; set; } = null!;
        
        public Discipline() { }
        
        public Discipline(int id, string name, string description, int credits, int? teacherId = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Credits = credits;
            TeacherId = teacherId;
        }
    }
    
    // ==================== АБСТРАКТНЫЙ СТУДЕНТ ====================
    
    public abstract class Student
    {
        [Key]
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
        public string Email { get; set; } = null!;
        
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = null!;
        
        [StringLength(200)]
        public string Address { get; set; } = null!;
        
        public int? GroupId { get; set; }
        
        [ForeignKey("GroupId")]
        public Group? Group { get; set; }
        
        public int? DepartmentId { get; set; }
        
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        
        [Required]
        [StringLength(20)]
        public string StudentType { get; set; } = null!;
        
        public List<Discipline> Disciplines { get; set; } = new();
        
        protected Student() { }
        
        protected Student(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth, 
                         string email, string phone, string address, string studentType)
        {
            RecordBookNumber = recordBookNumber;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            Phone = phone;
            Address = address;
            StudentType = studentType;
        }
        
        public abstract void PrintInfo();
        
        public virtual void PrintFullInfo()
        {
            Console.WriteLine($"Студент: {LastName} {FirstName}");
            Console.WriteLine($"№ зачетки: {RecordBookNumber}");
            Console.WriteLine($"Дата рождения: {DateOfBirth:dd.MM.yyyy}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Телефон: {Phone}");
            Console.WriteLine($"Адрес: {Address}");
            Console.WriteLine($"Группа: {Group?.Name ?? "Не указана"}");
            Console.WriteLine($"Кафедра: {Department?.Name ?? "Не указана"}");
            Console.WriteLine($"Тип студента: {StudentType}");
            Console.WriteLine("Дисциплины:");
            foreach (var discipline in Disciplines)
            {
                string teacherName = discipline.Teacher != null ? discipline.Teacher.FullName : "Не назначен";
                Console.WriteLine($"  - {discipline.Name} (Кредиты: {discipline.Credits}, Преподаватель: {teacherName})");
            }
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
                              string email, string phone, string address, int egeScore, double averageScore)
            : base(recordBookNumber, firstName, lastName, dateOfBirth, email, phone, address, "FullTime")
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
        public string WorkPlace { get; set; } = null!;
        
        [StringLength(50)]
        public string Position { get; set; } = null!;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TuitionFee { get; set; }
        
        public PartTimeStudent() : base() { }
        
        public PartTimeStudent(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth,
                              string email, string phone, string address, string workPlace, string position, decimal tuitionFee)
            : base(recordBookNumber, firstName, lastName, dateOfBirth, email, phone, address, "PartTime")
        {
            WorkPlace = workPlace;
            Position = position;
            TuitionFee = tuitionFee;
        }
        
        public override void PrintInfo()
        {
            Console.WriteLine($"[ЗАОЧНИК] {LastName} {FirstName}, Группа: {Group?.Name ?? "Нет группы"}, " +
                            $"Место работы: {WorkPlace}, Должность: {Position}");
        }
        
        public override void PrintFullInfo()
        {
            base.PrintFullInfo();
            Console.WriteLine($"Место работы: {WorkPlace}");
            Console.WriteLine($"Должность: {Position}");
            Console.WriteLine($"Стоимость обучения: {TuitionFee:C}");
        }
    }
    
    // ==================== ЦЕЛЕВОЕ ОТДЕЛЕНИЕ ====================
    
    public class TargetStudent : Student
    {
        [StringLength(100)]
        public string TargetCompany { get; set; } = null!;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TuitionFee { get; set; }
        
        public TargetStudent() : base() { }
        
        public TargetStudent(string recordBookNumber, string firstName, string lastName, DateTime dateOfBirth,
                            string email, string phone, string address, string targetCompany, decimal tuitionFee)
            : base(recordBookNumber, firstName, lastName, dateOfBirth, email, phone, address, "Target")
        {
            TargetCompany = targetCompany;
            TuitionFee = tuitionFee;
        }
        
        public override void PrintInfo()
        {
            Console.WriteLine($"[ЦЕЛЕВИК] {LastName} {FirstName}, Группа: {Group?.Name ?? "Нет группы"}, " +
                            $"Целевое предприятие: {TargetCompany}");
        }
        
        public override void PrintFullInfo()
        {
            base.PrintFullInfo();
            Console.WriteLine($"Целевое предприятие: {TargetCompany}");
            Console.WriteLine($"Стоимость обучения: {TuitionFee:C}");
        }
    }
}