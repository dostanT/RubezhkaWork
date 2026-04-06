using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();

            var teacher = await _service.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound();

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
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateTeacherAsync(teacher);
                    TempData["Success"] = $"Преподаватель {teacher.LastName} {teacher.FirstName} успешно создан";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании преподавателя");
                    ModelState.AddModelError("", "Ошибка при создании преподавателя: " + ex.Message);
                }
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound();

            var teacher = await _service.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound();

            return View(teacher);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateTeacherAsync(teacher);
                    TempData["Success"] = "Данные преподавателя обновлены";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении преподавателя");
                    ModelState.AddModelError("", "Ошибка при обновлении преподавателя");
                }
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var teacher = await _service.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound();

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteTeacherAsync(id);
                TempData["Success"] = "Преподаватель успешно удален";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении преподавателя");
                TempData["Error"] = "Ошибка при удалении преподавателя: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}