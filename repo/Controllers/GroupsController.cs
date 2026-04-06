using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IUniversityDbService _service;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IUniversityDbService service, ILogger<GroupsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var groups = await _service.GetAllGroupsWithStudentsAsync();
            return View(groups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();

            var group = await _service.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound();

            var students = await _service.GetStudentsInGroupAsync(id);
            ViewBag.Students = students;
            ViewBag.StudentsCount = students.Count;
            
            return View(group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group group)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateGroupAsync(group);
                    TempData["Success"] = $"Группа '{group.Name}' успешно создана";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании группы");
                    ModelState.AddModelError("", "Ошибка при создании группы: " + ex.Message);
                }
            }
            return View(group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound();

            var group = await _service.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group group)
        {
            if (id != group.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateGroupAsync(group);
                    TempData["Success"] = "Данные группы обновлены";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении группы");
                    ModelState.AddModelError("", "Ошибка при обновлении группы");
                }
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var group = await _service.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound();

            var students = await _service.GetStudentsInGroupAsync(id);
            if (students.Any())
            {
                ViewBag.HasStudents = true;
                ViewBag.StudentsCount = students.Count;
                ViewBag.Students = students;
            }

            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var students = await _service.GetStudentsInGroupAsync(id);
            if (students.Any())
            {
                TempData["Error"] = $"Нельзя удалить группу, в которой есть {students.Count} студентов";
                return RedirectToAction(nameof(Delete), new { id });
            }
            
            await _service.DeleteGroupAsync(id);
            TempData["Success"] = "Группа успешно удалена";
            return RedirectToAction(nameof(Index));
        }
    }
}