using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IUniversityDbService _service;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IUniversityDbService service, ILogger<StudentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _service.GetAllStudentsAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _service.GetStudentWithDetailsAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.Groups = _service.GetAllGroupsAsync().Result;
            ViewBag.Departments = _service.GetAllDepartmentsAsync().Result;
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordBookNumber,FirstName,LastName,DateOfBirth,Email,Phone,Address,GroupId,DepartmentId,StudentType,AverageScore,WorkPlace,CompanyName")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateStudentAsync(student);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании студента");
                    ModelState.AddModelError("", "Ошибка при создании студента");
                }
            }
            
            ViewBag.Groups = await _service.GetAllGroupsAsync();
            ViewBag.Departments = await _service.GetAllDepartmentsAsync();
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _service.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            
            ViewBag.Groups = await _service.GetAllGroupsAsync();
            ViewBag.Departments = await _service.GetAllDepartmentsAsync();
            ViewBag.Disciplines = await _service.GetAllDisciplinesAsync();
            ViewBag.StudentDisciplines = await _service.GetStudentDisciplinesAsync(id);
            
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RecordBookNumber,FirstName,LastName,DateOfBirth,Email,Phone,Address,GroupId,DepartmentId,StudentType,AverageScore,WorkPlace,CompanyName")] Student student)
        {
            if (id != student.RecordBookNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateStudentAsync(student);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await StudentExists(student.RecordBookNumber))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            
            ViewBag.Groups = await _service.GetAllGroupsAsync();
            ViewBag.Departments = await _service.GetAllDepartmentsAsync();
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _service.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Filter
        public async Task<IActionResult> Filter(string groupName, string departmentName, string disciplineName, string teacherLastName)
        {
            List<Student> students = new List<Student>();
            
            if (!string.IsNullOrEmpty(groupName))
            {
                students = await _service.GetStudentsByGroupAsync(groupName);
                ViewBag.FilterType = "Группе";
                ViewBag.FilterValue = groupName;
            }
            else if (!string.IsNullOrEmpty(departmentName))
            {
                students = await _service.GetStudentsByDepartmentAsync(departmentName);
                ViewBag.FilterType = "Кафедре";
                ViewBag.FilterValue = departmentName;
            }
            else if (!string.IsNullOrEmpty(disciplineName))
            {
                students = await _service.GetStudentsByDisciplineAsync(disciplineName);
                ViewBag.FilterType = "Дисциплине";
                ViewBag.FilterValue = disciplineName;
            }
            else if (!string.IsNullOrEmpty(teacherLastName))
            {
                students = await _service.GetStudentsByTeacherAsync(teacherLastName);
                ViewBag.FilterType = "Преподавателю";
                ViewBag.FilterValue = teacherLastName;
            }
            else
            {
                students = await _service.GetAllStudentsAsync();
            }
            
            ViewBag.Groups = await _service.GetAllGroupsAsync();
            ViewBag.Disciplines = await _service.GetAllDisciplinesAsync();
            ViewBag.Teachers = await _service.GetAllTeachersAsync();
            
            return View(students);
        }

        // POST: Students/AssignDiscipline
        [HttpPost]
        public async Task<IActionResult> AssignDiscipline(string studentId, int disciplineId)
        {
            if (string.IsNullOrEmpty(studentId) || disciplineId == 0)
            {
                return BadRequest();
            }

            try
            {
                await _service.AssignDisciplineAsync(studentId, disciplineId);
                TempData["Success"] = "Дисциплина успешно добавлена";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = studentId });
        }

        // POST: Students/RemoveDiscipline
        [HttpPost]
        public async Task<IActionResult> RemoveDiscipline(string studentId, int disciplineId)
        {
            if (string.IsNullOrEmpty(studentId) || disciplineId == 0)
            {
                return BadRequest();
            }

            try
            {
                await _service.RemoveDisciplineAsync(studentId, disciplineId);
                TempData["Success"] = "Дисциплина успешно удалена";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = studentId });
        }

        private async Task<bool> StudentExists(string id)
        {
            return await _service.ExistsAsync<Student>(s => s.RecordBookNumber == id);
        }
    }
}