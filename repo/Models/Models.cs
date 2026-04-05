// using System.Text.RegularExpressions;

// namespace repo.Models
// {
//     public class StudentModel
//     {
//         public int id { get; set; }
//         public required string secondName { get; set; }
//         public required string name { get; set; }
//         public int groupId { get; set; }
//         public int cafedraId { get; set; }
//         public required List<int> disciplineIds { get; set; }
//         public int gpa { get; set; }
//         public int teacherId { get; set; }
//     }
//     public class GroupModel
//     {
//         public int id { get; set; }
//         public required List<int> studentsIds { get; set; }

//     }

//     public class DisciplineModel
//     {
//         public int id { get; set; }
//         public int teacherId { get; set; }
//         public required List<DisciplineRecordModel> scores { get; set; }
//     }

//     public class DisciplineRecordModel
//     {
//         public int id { get; set; }
//         public int studentID { get; set; }
//         public int score { get; set; }
//     }
// }


// //(номер зачетной книжки, фамилия, имя, группа, кафедра, дисциплина, оценка, фамилия преподавателя) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace repo.Models
{
    // ==================== БАЗОВЫЕ СУЩНОСТИ ====================
    
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Student> Students { get; set; } = new();
        
        public Group(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Student> Students { get; set; } = new();
        
        public Department(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    
    public class Teacher
    {
        public int Id { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public List<Discipline> Disciplines { get; set; } = new();
        
        public Teacher(int id, string lastName, string firstName, string patronymic)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
        }
        
        public string FullName => $"{LastName} {FirstName} {Patronymic}";
    }
    
    public class Discipline
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public Teacher? Teacher { get; set; }
        public string Grade { get; set; } = null!;

        // Пустой конструктор для EF
        public Discipline() { }

        // Дополнительный конструктор для удобства создания в коде
        public Discipline(int id, string name, string grade = "")
        {
            Id = id;
            Name = name;
            Grade = grade;
        }
    }
    
    // ==================== АБСТРАКТНЫЙ СТУДЕНТ ====================
    
    

public abstract class Student
{
    [Key] // <- Это делает EF aware, что это PK
    public string RecordBookNumber { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string Patronymic { get; set; } = null!;

    public Group? Group { get; set; }
    public Department? Department { get; set; }
    public List<Discipline> Disciplines { get; set; } = new();

    public Student(){}
    
    protected Student(string recordBookNumber, string lastName, string firstName, string patronymic)
    {
        RecordBookNumber = recordBookNumber;
        LastName = lastName;
        FirstName = firstName;
        Patronymic = patronymic;
    }

    public abstract void PrintInfo();

        public void PrintFullInfo()
        {
            Console.WriteLine($"Студент: {LastName} {FirstName} {Patronymic}");
            Console.WriteLine($"№ зачетки: {RecordBookNumber}");
            Console.WriteLine($"Группа: {Group?.Name ?? "Не указана"}");
            Console.WriteLine($"Кафедра: {Department?.Name ?? "Не указана"}");
            Console.WriteLine("Дисциплины:");
            foreach (var discipline in Disciplines)
            {
                string teacherName = discipline.Teacher != null ? discipline.Teacher.FullName : "Не назначен";
                Console.WriteLine($"  - {discipline.Name} (Оценка: {discipline.Grade}, Преподаватель: {teacherName})");
            }
        }
    }
    
    // ==================== ОЧНОЕ ОТДЕЛЕНИЕ ====================
    
    public class FullTimeStudent : Student
    {
        public int EgeScore { get; set; }
        public double AverageScore { get; set; }

        public FullTimeStudent(string recordBookNumber, string lastName, string firstName, string patronymic)
            : base(recordBookNumber, lastName, firstName, patronymic)
        {
        }

        public override void PrintInfo()
        {
            Console.WriteLine($"[ОЧНИК] {LastName} {FirstName} {Patronymic}, Группа: {Group?.Name ?? "Нет группы"}, ЕГЭ: {EgeScore}, Средний балл: {AverageScore:F2}");
        }
    }
    
    // ==================== ЗАОЧНОЕ ОТДЕЛЕНИЕ ====================
    
    public class PartTimeStudent : Student
    {
        public string WorkPlace { get; set; } = null!;
        public string Position { get; set; } = null!;
        public decimal TuitionFee { get; set; }

        public PartTimeStudent(string recordBookNumber, string lastName, string firstName, string patronymic)
            : base(recordBookNumber, lastName, firstName, patronymic)
        {
        }

        public override void PrintInfo()
        {
            Console.WriteLine($"[ЗАОЧНИК] {LastName} {FirstName} {Patronymic}, Группа: {Group?.Name ?? "Нет группы"}, Место работы: {WorkPlace}, Должность: {Position}, Сумма обучения: {TuitionFee:C}");
        }
    }
    
    // ==================== ЦЕЛЕВОЕ ОТДЕЛЕНИЕ ====================
    
    public class TargetStudent : Student
    {
        public string TargetCompany { get; set; } = null!;
        public decimal TuitionFee { get; set; }

        public TargetStudent(string recordBookNumber, string lastName, string firstName, string patronymic)
            : base(recordBookNumber, lastName, firstName, patronymic)
        {
        }

        public override void PrintInfo()
        {
            Console.WriteLine($"[ЦЕЛЕВИК] {LastName} {FirstName} {Patronymic}, Группа: {Group?.Name ?? "Нет группы"}, Целевое предприятие: {TargetCompany}, Сумма обучения: {TuitionFee:C}");
        }
    }
    
    // ==================== УПРАВЛЕНИЕ СТУДЕНТАМИ ====================
    
    public class StudentList
    {
        private readonly List<Student> students = new();

        public void AddStudent(Student student)
        {
            students.Add(student);
        }
        
        public void RemoveStudent(string recordBookNumber)
        {
            var student = students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);
            if (student != null)
            {
                students.Remove(student);
                Console.WriteLine($"Студент {student.LastName} {student.FirstName} удален");
            }
            else
            {
                Console.WriteLine($"Студент с номером зачетки {recordBookNumber} не найден");
            }
        }
        
        public Student? FindStudent(string recordBookNumber)
        {
            return students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);
        }
        
        public List<Student> GetStudentsByGroup(string groupName)
        {
            return students.Where(s => s.Group != null && s.Group.Name == groupName).ToList();
        }
        
        public List<Student> GetStudentsByDepartment(string departmentName)
        {
            return students.Where(s => s.Department != null && s.Department.Name == departmentName).ToList();
        }
        
        public List<Student> GetStudentsByDiscipline(string disciplineName)
        {
            return students.Where(s => s.Disciplines.Any(d => d.Name == disciplineName)).ToList();
        }
        
        public List<Student> GetStudentsByTeacher(string teacherLastName)
        {
            return students.Where(s => s.Disciplines.Any(d => d.Teacher != null && d.Teacher.LastName == teacherLastName)).ToList();
        }
        
        public void PrintAllStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст");
                return;
            }
            
            Console.WriteLine($"\n=== ВСЕГО СТУДЕНТОВ: {students.Count} ===\n");
            foreach (var student in students)
            {
                student.PrintInfo();
            }
        }
        
        public void PrintAllStudentsFullInfo()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст");
                return;
            }
            
            Console.WriteLine($"\n=== ПОЛНАЯ ИНФОРМАЦИЯ О СТУДЕНТАХ ({students.Count}) ===\n");
            foreach (var student in students)
            {
                student.PrintFullInfo();
                Console.WriteLine();
            }
        }
    }
}

