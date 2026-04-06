using Microsoft.EntityFrameworkCore;
using repo.Models;

namespace repo.Services
{
    public interface IUniversityDbService
    {
        // ==================== ОСНОВНЫЕ CRUD ДЛЯ СТУДЕНТОВ ====================
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(string recordBookNumber);
        Task<Student?> GetStudentWithDetailsAsync(string recordBookNumber);
        Task CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(string recordBookNumber);
        
        // ==================== ПОИСК И ФИЛЬТРАЦИЯ ====================
        Task<List<Student>> GetStudentsByGroupAsync(string groupName);
        Task<List<Student>> GetStudentsByDepartmentAsync(string departmentName);
        Task<List<Student>> GetStudentsByDisciplineAsync(string disciplineName);
        Task<List<Student>> GetStudentsByTeacherAsync(string teacherLastName);
        Task<List<FullTimeStudent>> GetFullTimeStudentsAsync();
        Task<List<PartTimeStudent>> GetPartTimeStudentsAsync();
        Task<List<TargetStudent>> GetTargetStudentsAsync();
        
        // ==================== РАБОТА С ДИСЦИПЛИНАМИ ====================
        Task AssignDisciplineAsync(string studentId, int disciplineId);
        Task RemoveDisciplineAsync(string studentId, int disciplineId);
        Task<List<Discipline>> GetStudentDisciplinesAsync(string studentId);
        
        // ==================== СТАТИСТИКА ====================
        Task<int> GetTotalStudentsCountAsync();
        Task<double> CalculateAverageScoreAsync(string studentId);
        Task<List<Student>> GetTopStudentsAsync(int count);
        Task<Dictionary<string, int>> GetStudentCountByGroupAsync();
        
        // ==================== РАБОТА С ГРУППАМИ ====================
        Task<List<Group>> GetAllGroupsAsync();
        Task<Group?> GetGroupByIdAsync(int id);
        Task<Group?> GetGroupWithStudentsAsync(string groupName);
        Task<List<Group>> GetAllGroupsWithStudentsAsync();
        Task CreateGroupAsync(Group group);
        Task UpdateGroupAsync(Group group);
        Task DeleteGroupAsync(int id);
        Task<List<Student>> GetStudentsInGroupAsync(int groupId);
        Task AddStudentToGroupAsync(string studentId, int groupId);
        Task RemoveStudentFromGroupAsync(string studentId);
        
        // ==================== РАБОТА С КАФЕДРАМИ ====================
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task CreateDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int id);
        
        // ==================== РАБОТА С ДИСЦИПЛИНАМИ (полный CRUD) ====================
        Task<List<Discipline>> GetAllDisciplinesAsync();
        Task<List<Discipline>> GetAllDisciplinesWithTeachersAsync();
        Task<Discipline?> GetDisciplineByIdAsync(int id);
        Task CreateDisciplineAsync(Discipline discipline);
        Task UpdateDisciplineAsync(Discipline discipline);
        Task DeleteDisciplineAsync(int id);
        Task AssignTeacherToDisciplineAsync(int disciplineId, int teacherId);
        
        // ==================== РАБОТА С ПРЕПОДАВАТЕЛЯМИ ====================
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task CreateTeacherAsync(Teacher teacher);
        Task UpdateTeacherAsync(Teacher teacher);
        Task DeleteTeacherAsync(int id);
        
        // ==================== ПОИСК (GENERIC) ====================
        Task<List<T>> FindAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;
        Task<bool> ExistsAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;
    }
    
    public class UniversityDbService : IUniversityDbService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContext> _logger;
        
        public UniversityDbService(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        // ==================== ОСНОВНЫЕ CRUD ДЛЯ СТУДЕНТОВ ====================
        
        // В методах GetAllStudentsAsync, GetStudentByIdAsync и др.
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                        .ThenInclude(d => d.Teacher)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех студентов");
                throw;
            }
        }
        
        public async Task<Student?> GetStudentByIdAsync(string recordBookNumber)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                    .ThenInclude(d => d.Teacher)
                    .FirstOrDefaultAsync(s => s.RecordBookNumber == recordBookNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении студента с номером {RecordBookNumber}", recordBookNumber);
                throw;
            }
        }
        
        public async Task<Student?> GetStudentWithDetailsAsync(string recordBookNumber)
        {
            return await GetStudentByIdAsync(recordBookNumber);
        }
        
        public async Task CreateStudentAsync(Student student)
        {
            try
            {
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Студент {LastName} {FirstName} успешно создан", student.LastName, student.FirstName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании студента");
                throw;
            }
        }
        
        public async Task UpdateStudentAsync(Student student)
        {
            try
            {
                _context.Entry(student).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Студент {RecordBookNumber} успешно обновлен", student.RecordBookNumber);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Конфликт при обновлении студента {RecordBookNumber}", student.RecordBookNumber);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении студента");
                throw;
            }
        }
        
        public async Task DeleteStudentAsync(string recordBookNumber)
        {
            try
            {
                var student = await GetStudentByIdAsync(recordBookNumber);
                if (student != null)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Студент {RecordBookNumber} успешно удален", recordBookNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении студента {RecordBookNumber}", recordBookNumber);
                throw;
            }
        }
        
        // ==================== ПОИСК И ФИЛЬТРАЦИЯ ====================
        
        public async Task<List<Student>> GetStudentsByGroupAsync(string groupName)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                    .Where(s => s.Group != null && s.Group.Name == groupName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске студентов по группе {GroupName}", groupName);
                throw;
            }
        }
        
        public async Task<List<Student>> GetStudentsByDepartmentAsync(string departmentName)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                    .Where(s => s.Department != null && s.Department.Name == departmentName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске студентов по кафедре {DepartmentName}", departmentName);
                throw;
            }
        }
        
        public async Task<List<Student>> GetStudentsByDisciplineAsync(string disciplineName)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                    .Where(s => s.Disciplines.Any(d => d.Name == disciplineName))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске студентов по дисциплине {DisciplineName}", disciplineName);
                throw;
            }
        }
        
        public async Task<List<Student>> GetStudentsByTeacherAsync(string teacherLastName)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                    .ThenInclude(d => d.Teacher)
                    .Where(s => s.Disciplines.Any(d => d.Teacher != null && d.Teacher.LastName == teacherLastName))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске студентов по преподавателю {TeacherLastName}", teacherLastName);
                throw;
            }
        }
        
        public async Task<List<FullTimeStudent>> GetFullTimeStudentsAsync()
        {
            return await _context.Students
                .OfType<FullTimeStudent>()
                .Include(s => s.Group)
                .Include(s => s.Department)
                .Include(s => s.Disciplines)
                .ToListAsync();
        }
        
        public async Task<List<PartTimeStudent>> GetPartTimeStudentsAsync()
        {
            return await _context.Students
                .OfType<PartTimeStudent>()
                .Include(s => s.Group)
                .Include(s => s.Department)
                .Include(s => s.Disciplines)
                .ToListAsync();
        }
        
        public async Task<List<TargetStudent>> GetTargetStudentsAsync()
        {
            return await _context.Students
                .OfType<TargetStudent>()
                .Include(s => s.Group)
                .Include(s => s.Department)
                .Include(s => s.Disciplines)
                .ToListAsync();
        }
        
        // ==================== РАБОТА С ДИСЦИПЛИНАМИ ====================
        
        public async Task AssignDisciplineAsync(string studentId, int disciplineId)
        {
            try
            {
                // Загружаем студента с дисциплинами
                var student = await _context.Students
                    .Include(s => s.Disciplines)
                    .FirstOrDefaultAsync(s => s.RecordBookNumber == studentId);
                    
                var discipline = await _context.Disciplines.FindAsync(disciplineId);
                
                if (student == null || discipline == null)
                    throw new ArgumentException("Студент или дисциплина не найдены");
                
                if (!student.Disciplines.Any(d => d.Id == disciplineId))
                {
                    student.Disciplines.Add(discipline);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Дисциплина {DisciplineId} добавлена студенту {StudentId}", disciplineId, studentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении дисциплины студенту");
                throw;
            }
        }

        public async Task RemoveDisciplineAsync(string studentId, int disciplineId)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.Disciplines)
                    .FirstOrDefaultAsync(s => s.RecordBookNumber == studentId);
                    
                if (student != null)
                {
                    var discipline = student.Disciplines.FirstOrDefault(d => d.Id == disciplineId);
                    if (discipline != null)
                    {
                        student.Disciplines.Remove(discipline);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Дисциплина {DisciplineId} удалена у студента {StudentId}", disciplineId, studentId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении дисциплины у студента");
                throw;
            }
        }
        
        public async Task<List<Discipline>> GetStudentDisciplinesAsync(string studentId)
        {
            var student = await _context.Students
                .Include(s => s.Disciplines)
                .ThenInclude(d => d.Teacher)
                .FirstOrDefaultAsync(s => s.RecordBookNumber == studentId);
                
            return student?.Disciplines.ToList() ?? new List<Discipline>();
        }
        
        // ==================== СТАТИСТИКА ====================
        
        public async Task<int> GetTotalStudentsCountAsync()
        {
            return await _context.Students.CountAsync();
        }
        
        public async Task<double> CalculateAverageScoreAsync(string studentId)
        {
            var student = await GetStudentByIdAsync(studentId);
            if (student is FullTimeStudent fullTime)
            {
                return fullTime.AverageScore;
            }
            return 0;
        }
        
        public async Task<List<Student>> GetTopStudentsAsync(int count)
        {
            return await _context.Students
                .OfType<FullTimeStudent>()
                .OrderByDescending(s => ((FullTimeStudent)s).AverageScore)
                .Take(count)
                .Include(s => s.Group)
                .Include(s => s.Department)
                .Include(s => s.Disciplines)
                .ThenInclude(d => d.Teacher)
                .Select(s => (Student)s)  // Явное приведение
                .ToListAsync();
        }
        
        public async Task<Dictionary<string, int>> GetStudentCountByGroupAsync()
        {
            return await _context.Students
                .Where(s => s.Group != null)
                .GroupBy(s => s.Group!.Name)
                .Select(g => new { GroupName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.GroupName, x => x.Count);
        }
        
        // ==================== РАБОТА С ГРУППАМИ ====================
        
        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups.ToListAsync();
        }
        
        public async Task<Group?> GetGroupByIdAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }
        
        public async Task<Group?> GetGroupWithStudentsAsync(string groupName)
        {
            return await _context.Groups
                .Include(g => g.Students)
                .ThenInclude(s => s.Disciplines)
                .FirstOrDefaultAsync(g => g.Name == groupName);
        }
        
        public async Task<List<Group>> GetAllGroupsWithStudentsAsync()
        {
            return await _context.Groups
                .Include(g => g.Students)
                .ToListAsync();
        }
        
        public async Task CreateGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateGroupAsync(Group group)
        {
            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteGroupAsync(int id)
        {
            var group = await GetGroupByIdAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<List<Student>> GetStudentsInGroupAsync(int groupId)
        {
            return await _context.Students
                .Where(s => s.Group != null && s.Group.Id == groupId)
                .Include(s => s.Disciplines)
                .ToListAsync();
        }
        
        public async Task AddStudentToGroupAsync(string studentId, int groupId)
        {
            var student = await GetStudentByIdAsync(studentId);
            var group = await GetGroupByIdAsync(groupId);
            
            if (student != null && group != null)
            {
                student.Group = group;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task RemoveStudentFromGroupAsync(string studentId)
        {
            var student = await GetStudentByIdAsync(studentId);
            if (student != null)
            {
                student.Group = null;
                await _context.SaveChangesAsync();
            }
        }
        
        // ==================== РАБОТА С КАФЕДРАМИ ====================
        
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }
        
        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }
        
        public async Task CreateDepartmentAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateDepartmentAsync(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }
        
        // ==================== РАБОТА С ДИСЦИПЛИНАМИ (полный CRUD) ====================
        
        public async Task<List<Discipline>> GetAllDisciplinesAsync()
        {
            return await _context.Disciplines.ToListAsync();
        }
        
        public async Task<List<Discipline>> GetAllDisciplinesWithTeachersAsync()
        {
            return await _context.Disciplines
                .Include(d => d.Teacher)
                .ToListAsync();
        }
        
        public async Task<Discipline?> GetDisciplineByIdAsync(int id)
        {
            return await _context.Disciplines
                .Include(d => d.Teacher)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task CreateDisciplineAsync(Discipline discipline)
        {
            await _context.Disciplines.AddAsync(discipline);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateDisciplineAsync(Discipline discipline)
        {
            _context.Entry(discipline).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Student>> GetStudentsByTypeAsync(string studentType)
        {
            try
            {
                return await _context.Students
                    .Where(s => s.StudentType == studentType)
                    .Include(s => s.Group)
                    .Include(s => s.Department)
                    .Include(s => s.Disciplines)
                    .ThenInclude(d => d.Teacher)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске студентов по типу {StudentType}", studentType);
                throw;
            }
        }

        public async Task<List<GroupWithCountDto>> GetGroupsWithStudentCountAsync()
        {
            return await _context.Groups
                .Select(g => new GroupWithCountDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    YearOfStudy = g.YearOfStudy,
                    StudentCount = g.Students.Count
                })
                .ToListAsync();
        }

        // DTO класс
        public class GroupWithCountDto
        {
            public int Id { get; set; }
            public string? Name { get; set; } = null;
            public int YearOfStudy { get; set; }
            public int StudentCount { get; set; }
        }
        
        public async Task DeleteDisciplineAsync(int id)
        {
            var discipline = await GetDisciplineByIdAsync(id);
            if (discipline != null)
            {
                _context.Disciplines.Remove(discipline);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task AssignTeacherToDisciplineAsync(int disciplineId, int teacherId)
        {
            var discipline = await GetDisciplineByIdAsync(disciplineId);
            var teacher = await GetTeacherByIdAsync(teacherId);
            
            if (discipline != null && teacher != null)
            {
                discipline.Teacher = teacher;
                await _context.SaveChangesAsync();
            }
        }
        
        // ==================== РАБОТА С ПРЕПОДАВАТЕЛЯМИ ====================
        
        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers.ToListAsync();
        }
        
        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }
        
        public async Task CreateTeacherAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _context.Entry(teacher).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await GetTeacherByIdAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }
        }
        
        // ==================== ПОИСК (GENERIC) ====================
        
        public async Task<List<T>> FindAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
        
        public async Task<bool> ExistsAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
    }
}