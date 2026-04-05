using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _service.GetDepartmentByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            var students = await _service.GetStudentsByDepartmentAsync(department.Name);
            ViewBag.Students = students;
            ViewBag.StudentsCount = students.Count;

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
        public async Task<IActionResult> Create([Bind("Id,Name,HeadOfDepartment,Phone,Email")] Department department)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateDepartmentAsync(department);
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _service.GetDepartmentByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HeadOfDepartment,Phone,Email")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateDepartmentAsync(department);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _service.GetDepartmentByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            var students = await _service.GetStudentsByDepartmentAsync(department.Name);
            if (students.Any())
            {
                ViewBag.HasStudents = true;
                ViewBag.StudentsCount = students.Count;
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _service.GetDepartmentByIdAsync(id);
            if (department != null)
            {
                var students = await _service.GetStudentsByDepartmentAsync(department.Name);
                if (students.Any())
                {
                    TempData["Error"] = "Нельзя удалить кафедру, к которой привязаны студенты";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            
            await _service.DeleteDepartmentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DepartmentExists(int id)
        {
            return await _service.ExistsAsync<Department>(d => d.Id == id);
        }
    }
}