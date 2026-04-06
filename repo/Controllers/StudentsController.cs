using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var student = await _service.GetStudentWithDetailsAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Groups = new SelectList(await _service.GetAllGroupsAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _service.GetAllDepartmentsAsync(), "Id", "Name");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Student student = null;
                    
                    switch (model.StudentType)
                    {
                        case "FullTime":
                            student = new FullTimeStudent
                            {
                                RecordBookNumber = model.RecordBookNumber,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Patronymic = model.Patronymic ?? "",  // Добавьте это
                                DateOfBirth = model.DateOfBirth,
                                Email = model.Email,
                                Phone = model.Phone,
                                Address = model.Address ?? "",  // Добавьте ?? ""
                                GroupId = model.GroupId,
                                DepartmentId = model.DepartmentId,
                                StudentType = "FullTime",
                                EgeScore = model.EgeScore ?? 0,
                                AverageScore = model.AverageScore ?? 0
                            };
                            break;
                            
                        case "PartTime":
                            student = new PartTimeStudent
                            {
                                RecordBookNumber = model.RecordBookNumber,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Patronymic = model.Patronymic ?? "",  // Добавьте это
                                DateOfBirth = model.DateOfBirth,
                                Email = model.Email,
                                Phone = model.Phone,
                                Address = model.Address ?? "",  // Добавьте ?? ""
                                GroupId = model.GroupId,
                                DepartmentId = model.DepartmentId,
                                StudentType = "PartTime",
                                WorkPlace = model.WorkPlace ?? "",
                                Position = model.Position ?? "",
                                TuitionFee = model.TuitionFee ?? 0
                            };
                            break;
                            
                        case "Target":
                            student = new TargetStudent
                            {
                                RecordBookNumber = model.RecordBookNumber,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Patronymic = model.Patronymic ?? "",  // Добавьте это
                                DateOfBirth = model.DateOfBirth,
                                Email = model.Email,
                                Phone = model.Phone,
                                Address = model.Address ?? "",  // Добавьте ?? ""
                                GroupId = model.GroupId,
                                DepartmentId = model.DepartmentId,
                                StudentType = "Target",
                                TargetCompany = model.TargetCompany ?? "",
                                TuitionFee = model.TuitionFee ?? 0
                            };
                            break;
                    }
                    
                    if (student != null)
                    {
                        await _service.CreateStudentAsync(student);
                        TempData["Success"] = "Студент успешно создан";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании студента");
                    ModelState.AddModelError("", "Ошибка при создании студента: " + ex.Message);
                }
            }
            
            ViewBag.Groups = new SelectList(await _service.GetAllGroupsAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _service.GetAllDepartmentsAsync(), "Id", "Name");
            return View(model);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var student = await _service.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();
            
            var model = new StudentEditViewModel
            {
                RecordBookNumber = student.RecordBookNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Patronymic = student.Patronymic,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Phone = student.Phone,
                Address = student.Address,
                GroupId = student.GroupId,
                DepartmentId = student.DepartmentId,
                StudentType = student.StudentType
            };
            
            // Заполняем специфические поля
            if (student is FullTimeStudent fullTime)
            {
                model.EgeScore = fullTime.EgeScore;
                model.AverageScore = fullTime.AverageScore;
            }
            else if (student is PartTimeStudent partTime)
            {
                model.WorkPlace = partTime.WorkPlace;
                model.Position = partTime.Position;
                model.TuitionFee = partTime.TuitionFee;
            }
            else if (student is TargetStudent target)
            {
                model.TargetCompany = target.TargetCompany;
                model.TuitionFee = target.TuitionFee;
            }
            
            ViewBag.Groups = new SelectList(await _service.GetAllGroupsAsync(), "Id", "Name", student.GroupId);
            ViewBag.Departments = new SelectList(await _service.GetAllDepartmentsAsync(), "Id", "Name", student.DepartmentId);
            ViewBag.Disciplines = await _service.GetAllDisciplinesAsync();
            ViewBag.StudentDisciplines = await _service.GetStudentDisciplinesAsync(id);
            
            return View(model);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StudentEditViewModel model)
        {
            if (id != model.RecordBookNumber)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _service.GetStudentByIdAsync(id);
                    if (student == null)
                        return NotFound();
                    
                    // Обновляем общие поля
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.Patronymic = model.Patronymic;
                    student.DateOfBirth = model.DateOfBirth;
                    student.Email = model.Email;
                    student.Phone = model.Phone;
                    student.Address = model.Address;
                    student.GroupId = model.GroupId;
                    student.DepartmentId = model.DepartmentId;
                    
                    // Обновляем специфические поля
                    if (student is FullTimeStudent fullTime)
                    {
                        fullTime.EgeScore = model.EgeScore ?? 0;
                        fullTime.AverageScore = model.AverageScore ?? 0;
                    }
                    else if (student is PartTimeStudent partTime)
                    {
                        partTime.WorkPlace = model.WorkPlace ?? "";
                        partTime.Position = model.Position ?? "";
                        partTime.TuitionFee = model.TuitionFee ?? 0;
                    }
                    else if (student is TargetStudent target)
                    {
                        target.TargetCompany = model.TargetCompany ?? "";
                        target.TuitionFee = model.TuitionFee ?? 0;
                    }
                    
                    await _service.UpdateStudentAsync(student);
                    TempData["Success"] = "Данные студента обновлены";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении студента");
                    ModelState.AddModelError("", "Ошибка при обновлении студента");
                }
            }
            
            ViewBag.Groups = new SelectList(await _service.GetAllGroupsAsync(), "Id", "Name", model.GroupId);
            ViewBag.Departments = new SelectList(await _service.GetAllDepartmentsAsync(), "Id", "Name", model.DepartmentId);
            return View(model);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var student = await _service.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteStudentAsync(id);
            TempData["Success"] = "Студент удален";
            return RedirectToAction(nameof(Index));
        }

        // POST: Students/AssignDiscipline
        [HttpPost]
        public async Task<IActionResult> AssignDiscipline(string studentId, int disciplineId)
        {
            if (string.IsNullOrEmpty(studentId) || disciplineId == 0)
                return BadRequest();

            try
            {
                await _service.AssignDisciplineAsync(studentId, disciplineId);
                TempData["Success"] = "Дисциплина добавлена";
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
                return BadRequest();

            try
            {
                await _service.RemoveDisciplineAsync(studentId, disciplineId);
                TempData["Success"] = "Дисциплина удалена";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = studentId });
        }
    }
}

// ViewModel для создания студента
public class StudentCreateViewModel
{
    public string RecordBookNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Address { get; set; }
    public int? GroupId { get; set; }
    public int? DepartmentId { get; set; }
    public string StudentType { get; set; } = null!;
    
    // FullTime поля
    public int? EgeScore { get; set; }
    public double? AverageScore { get; set; }
    
    // PartTime поля
    public string? WorkPlace { get; set; }
    public string? Position { get; set; }
    public decimal? TuitionFee { get; set; }
    
    // Target поля
    public string? TargetCompany { get; set; }
}

// ViewModel для редактирования студента
public class StudentEditViewModel
{
    public string RecordBookNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Address { get; set; }
    public int? GroupId { get; set; }
    public int? DepartmentId { get; set; }
    public string StudentType { get; set; } = null!;
    
    // FullTime поля
    public int? EgeScore { get; set; }
    public double? AverageScore { get; set; }
    
    // PartTime поля
    public string? WorkPlace { get; set; }
    public string? Position { get; set; }
    public decimal? TuitionFee { get; set; }
    
    // Target поля
    public string? TargetCompany { get; set; }
}