using repo.Models;
using Microsoft.EntityFrameworkCore;

namespace repo.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Создаем базу, если ее нет
            context.Database.EnsureCreated();
            
            // Проверяем, есть ли уже данные
            if (context.Students.Any())
            {
                return; // Данные уже есть
            }
            
            // ========== 1. КАФЕДРЫ ==========
            var departments = new List<Department>();
            for (int i = 1; i <= 10; i++)
            {
                departments.Add(new Department 
                { 
                    Name = $"Факультет {i}", 
                    HeadOfDepartment = $"Проф. Иванов И.И.", 
                    Phone = $"+7(777)111-{i:D2}{i:D2}", 
                    Email = $"dept{i}@university.kz" 
                });
            }
            context.Departments.AddRange(departments);
            context.SaveChanges();
            
            // ========== 2. ГРУППЫ ==========
            var groups = new List<Group>();
            for (int deptId = 1; deptId <= 10; deptId++)
            {
                for (int course = 1; course <= 5; course++)
                {
                    groups.Add(new Group 
                    { 
                        Name = $"ГР-{deptId}{course}{Random.Shared.Next(1, 5)}", 
                        YearOfStudy = course, 
                        DepartmentId = deptId 
                    });
                }
            }
            context.Groups.AddRange(groups);
            context.SaveChanges();
            
            // ========== 3. ПРЕПОДАВАТЕЛИ (с логинами и паролями) ==========
            var firstNames = new[] { "Иван", "Мария", "Алексей", "Елена", "Дмитрий" };
            var lastNames = new[] { "Иванов", "Петрова", "Сидоров", "Смирнова", "Козлов" };
            var teachers = new List<Teacher>();
            
            for (int i = 1; i <= 50; i++)
            {
                string login = $"teacher{i}";
                string password = $"Teacher{i}123!"; // В реальном проекте нужно хешировать!
                
                teachers.Add(new Teacher 
                { 
                    FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                    LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                    Patronymic = "Иванович",
                    AcademicDegree = "Кандидат наук", 
                    Position = "Доцент", 
                    Phone = $"+7(701){i:000}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                    Email = $"teacher{i}@university.kz",
                    Login = login,
                    Password = password
                });
            }
            context.Teachers.AddRange(teachers);
            context.SaveChanges();
            
            // ========== 4. ДИСЦИПЛИНЫ ==========
            var disciplines = new List<Discipline>();
            string[] baseNames = 
            {
                "Программирование", "Базы данных", "Web-разработка", "Алгоритмы", "ООП",
                "Экономика", "Маркетинг", "Финансы", "Бухучет", "Менеджмент"
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
                    Name = name, 
                    Description = $"Изучение {name.ToLower()}", 
                    Credits = Random.Shared.Next(3, 7), 
                    TeacherId = Random.Shared.Next(1, 51) 
                });
            }
            context.Disciplines.AddRange(disciplines);
            context.SaveChanges();
            
            // ========== 5. СТУДЕНТЫ (с логинами и паролями) ==========
            var students = new List<Student>();
            var groupsList = context.Groups.ToList();
            
            // 500 очников
            for (int i = 1; i <= 500; i++)
            {
                var group = groupsList[Random.Shared.Next(groupsList.Count)];
                string login = $"student{i}";
                string password = $"Student{i}123!"; // В реальном проекте нужно хешировать!
                
                students.Add(new FullTimeStudent 
                { 
                    RecordBookNumber = $"2024{i:0000}", 
                    FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                    LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                    Patronymic = "Иванович",
                    DateOfBirth = new DateTime(Random.Shared.Next(2002, 2007), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), 
                    Email = $"student{i}@student.kz", 
                    Phone = $"+7(700){Random.Shared.Next(100,999):D3}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                    Address = $"г. Алматы, ул. Абая, д.{Random.Shared.Next(1,100)}", 
                    GroupId = group.Id, 
                    DepartmentId = group.DepartmentId,
                    EgeScore = Random.Shared.Next(60, 101), 
                    AverageScore = Math.Round(Random.Shared.NextDouble() * 2.5 + 2.5, 2),
                    Login = login,
                    Password = password
                });
            }
            
            // 200 заочников
            for (int i = 501; i <= 700; i++)
            {
                var group = groupsList[Random.Shared.Next(groupsList.Count)];
                string login = $"student{i}";
                string password = $"Student{i}123!"; // В реальном проекте нужно хешировать!
                
                students.Add(new PartTimeStudent 
                { 
                    RecordBookNumber = $"2024{i:0000}", 
                    FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                    LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                    Patronymic = "Иванович",
                    DateOfBirth = new DateTime(Random.Shared.Next(2000, 2005), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), 
                    Email = $"student{i}@student.kz", 
                    Phone = $"+7(700){Random.Shared.Next(100,999):D3}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                    Address = $"г. Алматы, ул. Абая, д.{Random.Shared.Next(1,100)}", 
                    GroupId = group.Id, 
                    DepartmentId = group.DepartmentId,
                    WorkPlace = "Компания", 
                    Position = "Сотрудник", 
                    TuitionFee = Random.Shared.Next(300000, 600001),
                    Login = login,
                    Password = password
                });
            }
            
            // 100 целевиков
            for (int i = 701; i <= 800; i++)
            {
                var group = groupsList[Random.Shared.Next(groupsList.Count)];
                string login = $"student{i}";
                string password = $"Student{i}123!"; // В реальном проекте нужно хешировать!
                
                students.Add(new TargetStudent 
                { 
                    RecordBookNumber = $"2024{i:0000}", 
                    FirstName = firstNames[Random.Shared.Next(firstNames.Length)], 
                    LastName = lastNames[Random.Shared.Next(lastNames.Length)], 
                    Patronymic = "Иванович",
                    DateOfBirth = new DateTime(Random.Shared.Next(2002, 2007), Random.Shared.Next(1, 13), Random.Shared.Next(1, 29)), 
                    Email = $"student{i}@student.kz", 
                    Phone = $"+7(700){Random.Shared.Next(100,999):D3}-{Random.Shared.Next(10,99):D2}{Random.Shared.Next(10,99):D2}", 
                    Address = $"г. Алматы, ул. Абая, д.{Random.Shared.Next(1,100)}", 
                    GroupId = group.Id, 
                    DepartmentId = group.DepartmentId,
                    TargetCompany = "Компания", 
                    TuitionFee = 0,
                    Login = login,
                    Password = password
                });
            }
            
            context.Students.AddRange(students);
            context.SaveChanges();
            
            // ========== 6. СВЯЗИ СТУДЕНТ-ДИСЦИПЛИНА ==========
            var disciplinesList = context.Disciplines.ToList();
            var studentsList = context.Students.ToList();
            var studentDisciplines = new List<StudentDiscipline>();
            string[] grades = { "4", "5", "зачет" };
            
            foreach (var student in studentsList)
            {
                int disciplineCount = Random.Shared.Next(4, 7);
                var usedDisciplines = new HashSet<int>();
                
                for (int j = 0; j < disciplineCount; j++)
                {
                    int discId;
                    do
                    {
                        discId = disciplinesList[Random.Shared.Next(disciplinesList.Count)].Id;
                    } while (usedDisciplines.Contains(discId));
                    usedDisciplines.Add(discId);
                    
                    studentDisciplines.Add(new StudentDiscipline 
                    { 
                        StudentId = student.Id, 
                        DisciplineId = discId, 
                        Grade = grades[Random.Shared.Next(grades.Length)],
                        DateReceived = new DateTime(2024, Random.Shared.Next(6, 13), Random.Shared.Next(1, 29))
                    });
                }
            }
            
            context.StudentDisciplines.AddRange(studentDisciplines);
            context.SaveChanges();
        }
    }
}