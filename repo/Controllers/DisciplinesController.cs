using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class DisciplinesController : Controller
    {
        private readonly IUniversityDbService _service;
        private readonly ILogger<DisciplinesController> _logger;

        public DisciplinesController(IUniversityDbService service, ILogger<DisciplinesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: Disciplines
        public async Task<IActionResult> Index()
        {
            var disciplines = await _service.GetAllDisciplinesWithTeachersAsync();
            return View(disciplines);
        }

        // GET: Disciplines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _service.GetDisciplineByIdAsync(id.Value);
            if (discipline == null)
            {
                return NotFound();
            }

            var students = await _service.GetStudentsByDisciplineAsync(discipline.Name);
            ViewBag.Students = students;
            ViewBag.StudentsCount = students.Count;

            return View(discipline);
        }

        // GET: Disciplines/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Teachers = await _service.GetAllTeachersAsync();
            return View();
        }

        // POST: Disciplines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Credits,TeacherId")] Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateDisciplineAsync(discipline);
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Teachers = await _service.GetAllTeachersAsync();
            return View(discipline);
        }

        // GET: Disciplines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _service.GetDisciplineByIdAsync(id.Value);
            if (discipline == null)
            {
                return NotFound();
            }
            
            ViewBag.Teachers = await _service.GetAllTeachersAsync();
            return View(discipline);
        }

        // POST: Disciplines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Credits,TeacherId")] Discipline discipline)
        {
            if (id != discipline.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateDisciplineAsync(discipline);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DisciplineExists(discipline.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Teachers = await _service.GetAllTeachersAsync();
            return View(discipline);
        }

        // GET: Disciplines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _service.GetDisciplineByIdAsync(id.Value);
            if (discipline == null)
            {
                return NotFound();
            }

            var students = await _service.GetStudentsByDisciplineAsync(discipline.Name);
            if (students.Any())
            {
                ViewBag.HasStudents = true;
                ViewBag.StudentsCount = students.Count;
                ViewBag.Students = students.Take(5).ToList();
            }

            return View(discipline);
        }

        // POST: Disciplines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discipline = await _service.GetDisciplineByIdAsync(id);
            if (discipline != null)
            {
                var students = await _service.GetStudentsByDisciplineAsync(discipline.Name);
                if (students.Any())
                {
                    TempData["Error"] = $"Нельзя удалить дисциплину '{discipline.Name}', так как она привязана к {students.Count} студентам";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            
            await _service.DeleteDisciplineAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Disciplines/AssignTeacher
        public async Task<IActionResult> AssignTeacher(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _service.GetDisciplineByIdAsync(id.Value);
            if (discipline == null)
            {
                return NotFound();
            }

            ViewBag.Teachers = await _service.GetAllTeachersAsync();
            return View(discipline);
        }

        // POST: Disciplines/AssignTeacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacher(int disciplineId, int teacherId)
        {
            if (disciplineId == 0 || teacherId == 0)
            {
                return BadRequest();
            }

            try
            {
                await _service.AssignTeacherToDisciplineAsync(disciplineId, teacherId);
                TempData["Success"] = "Преподаватель успешно назначен";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = disciplineId });
        }

        private async Task<bool> DisciplineExists(int id)
        {
            return await _service.ExistsAsync<Discipline>(d => d.Id == id);
        }
    }
}