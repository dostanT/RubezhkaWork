using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class TeachersController : Controller
    {
        private readonly IUniversityDbService _service;
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(IUniversityDbService service, ILogger<TeachersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var teachers = await _service.GetAllTeachersAsync();
            return View(teachers);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _service.GetTeacherByIdAsync(id.Value);
            if (teacher == null)
            {
                return NotFound();
            }

            var disciplines = await _service.GetAllDisciplinesWithTeachersAsync();
            var teacherDisciplines = disciplines.Where(d => d.Teacher.Id == id).ToList();
            ViewBag.Disciplines = teacherDisciplines;
            
            var students = await _service.GetStudentsByTeacherAsync(teacher.LastName);
            ViewBag.Students = students;
            ViewBag.StudentsCount = students.Count;

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,AcademicDegree,Position,Phone,Email")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateTeacherAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _service.GetTeacherByIdAsync(id.Value);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,AcademicDegree,Position,Phone,Email")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateTeacherAsync(teacher);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _service.GetTeacherByIdAsync(id.Value);
            if (teacher == null)
            {
                return NotFound();
            }

            var disciplines = await _service.GetAllDisciplinesWithTeachersAsync();
            var teacherDisciplines = disciplines.Where(d => d.Teacher.Id == id).ToList();
            
            if (teacherDisciplines.Any())
            {
                ViewBag.HasDisciplines = true;
                ViewBag.DisciplinesCount = teacherDisciplines.Count;
                ViewBag.Disciplines = teacherDisciplines;
            }

            var students = await _service.GetStudentsByTeacherAsync(teacher.LastName);
            if (students.Any())
            {
                ViewBag.HasStudents = true;
                ViewBag.StudentsCount = students.Count;
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _service.GetTeacherByIdAsync(id);
            if (teacher != null)
            {
                var disciplines = await _service.GetAllDisciplinesWithTeachersAsync();
                var teacherDisciplines = disciplines.Where(d => d.Teacher.Id == id).ToList();
                
                if (teacherDisciplines.Any())
                {
                    TempData["Error"] = $"Нельзя удалить преподавателя '{teacher.LastName}', так как он ведет {teacherDisciplines.Count} дисциплин(у)";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            
            await _service.DeleteTeacherAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TeacherExists(int id)
        {
            return await _service.ExistsAsync<Teacher>(t => t.Id == id);
        }
    }
}