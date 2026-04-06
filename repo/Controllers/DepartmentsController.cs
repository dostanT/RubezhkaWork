using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using repo.Models;
using repo.Services;

namespace repo.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IUniversityDbService _service;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IUniversityDbService service, ILogger<DepartmentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _service.GetAllDepartmentsAsync();
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();

            var department = await _service.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreateDepartmentAsync(department);
                    TempData["Success"] = $"Кафедра '{department.Name}' успешно создана";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании кафедры");
                    ModelState.AddModelError("", "Ошибка при создании кафедры: " + ex.Message);
                }
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound();

            var department = await _service.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateDepartmentAsync(department);
                    TempData["Success"] = "Данные кафедры обновлены";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении кафедры");
                    ModelState.AddModelError("", "Ошибка при обновлении кафедры");
                }
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return NotFound();

            var department = await _service.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteDepartmentAsync(id);
                TempData["Success"] = "Кафедра успешно удалена";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении кафедры");
                TempData["Error"] = "Ошибка при удалении кафедры: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}