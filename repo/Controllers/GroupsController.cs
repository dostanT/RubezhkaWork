using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _service.GetGroupByIdAsync(id.Value);
            if (group == null)
            {
                return NotFound();
            }

            var students = await _service.GetStudentsInGroupAsync(id.Value);
            ViewBag.Students = students;
            
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
        public async Task<IActionResult> Create([Bind("Id,Name,YearOfStudy")] Group group)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateGroupAsync(group);
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _service.GetGroupByIdAsync(id.Value);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,YearOfStudy")] Group group)
        {
            if (id != group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateGroupAsync(group);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GroupExists(group.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _service.GetGroupByIdAsync(id.Value);
            if (group == null)
            {
                return NotFound();
            }

            var students = await _service.GetStudentsInGroupAsync(id.Value);
            if (students.Any())
            {
                ViewBag.HasStudents = true;
                ViewBag.StudentsCount = students.Count;
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
                TempData["Error"] = "Нельзя удалить группу, в которой есть студенты";
                return RedirectToAction(nameof(Delete), new { id });
            }
            
            await _service.DeleteGroupAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GroupExists(int id)
        {
            return await _service.ExistsAsync<Group>(g => g.Id == id);
        }
    }
}