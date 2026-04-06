using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();

            var discipline = await _service.GetDisciplineByIdAsync(id);
            if (discipline == null)
                return NotFound();

            return View(discipline);
        }

        // GET: Disciplines/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Teachers = new SelectList(await _service.GetAllTeachersAsync(), "Id", "FullName");
            return View();
        }

        // POST: Disciplines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateDisciplineAsync(discipline);
                    TempData["Success"] = $"Дисциплина '{discipline.Name}' успешно создана";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании дисциплины");
                    ModelState.AddModelError("", "Ошибка при создании дисциплины: " + ex.Message);
                }
            }
            
            ViewBag.Teachers = new SelectList(await _service.GetAllTeachersAsync(), "Id", "FullName", discipline.TeacherId);
            return View(discipline);
        }

        // GET: Disciplines/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound();

            var discipline = await _service.GetDisciplineByIdAsync(id);
            if (discipline == null)
                return NotFound();
            
            ViewBag.Teachers = new SelectList(await _service.GetAllTeachersAsync(), "Id", "FullName", discipline.TeacherId);
            return View(discipline);
        }

        // POST: Disciplines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discipline discipline)
        {
            if (id != discipline.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateDisciplineAsync(discipline);
                    TempData["Success"] = "Данные дисциплины обновлены";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении дисциплины");
                    ModelState.AddModelError("", "Ошибка при обновлении дисциплины");
                }
            }
            
            ViewBag.Teachers = new SelectList(await _service.GetAllTeachersAsync(), "Id", "FullName", discipline.TeacherId);
            return View(discipline);
        }

        // GET: Disciplines/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var discipline = await _service.GetDisciplineByIdAsync(id);
            if (discipline == null)
                return NotFound();

            return View(discipline);
        }

        // POST: Disciplines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteDisciplineAsync(id);
                TempData["Success"] = "Дисциплина успешно удалена";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении дисциплины");
                TempData["Error"] = "Ошибка при удалении дисциплины: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}