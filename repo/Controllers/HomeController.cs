using Microsoft.AspNetCore.Mvc;
using repo.Services;

namespace repo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUniversityDbService _service;

        public HomeController(ILogger<HomeController> logger, IUniversityDbService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _service.GetTotalStudentsCountAsync();
            ViewBag.TotalGroups = (await _service.GetAllGroupsAsync()).Count;
            ViewBag.TotalDepartments = (await _service.GetAllDepartmentsAsync()).Count;
            ViewBag.TotalDisciplines = (await _service.GetAllDisciplinesAsync()).Count;
            ViewBag.TotalTeachers = (await _service.GetAllTeachersAsync()).Count;
            
            var groupsStat = await _service.GetStudentCountByGroupAsync();
            ViewBag.GroupsStat = groupsStat;
            
            var topStudents = await _service.GetTopStudentsAsync(5);
            ViewBag.TopStudents = topStudents;
            
            var fullTimeCount = (await _service.GetFullTimeStudentsAsync()).Count;
            var partTimeCount = (await _service.GetPartTimeStudentsAsync()).Count;
            var targetCount = (await _service.GetTargetStudentsAsync()).Count;
            
            ViewBag.FullTimeCount = fullTimeCount;
            ViewBag.PartTimeCount = partTimeCount;
            ViewBag.TargetCount = targetCount;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

/*
Views/
│
├─ Home/
│   ├─ Index.cshtml          // Главная страница приложения
│   └─ Privacy.cshtml
│
├─ Students/
│   ├─ Index.cshtml          // Список всех студентов
│   ├─ Details.cshtml        // Подробности о студенте (Group, Department, Disciplines)
│   ├─ Create.cshtml         // Форма добавления студента
│   ├─ Edit.cshtml           // Форма редактирования студента
│   ├─ Delete.cshtml         // Подтверждение удаления
│   └─ Filter.cshtml         // Фильтры по группе/кафедре/дисциплине/преподавателю
│
├─ Groups/
│   ├─ Index.cshtml          // Список всех групп
│   ├─ Details.cshtml        // Группа с её студентами
│   ├─ Create.cshtml
│   ├─ Edit.cshtml
│   └─ Delete.cshtml
│
├─ Departments/
│   ├─ Index.cshtml
│   ├─ Details.cshtml
│   ├─ Create.cshtml
│   ├─ Edit.cshtml
│   └─ Delete.cshtml
│
├─ Disciplines/
│   ├─ Index.cshtml
│   ├─ Details.cshtml        // Дисциплина с привязанным Teacher
│   ├─ Create.cshtml
│   ├─ Edit.cshtml
│   └─ Delete.cshtml
│
└─ Teachers/
    ├─ Index.cshtml
    ├─ Details.cshtml
    ├─ Create.cshtml
    ├─ Edit.cshtml
    └─ Delete.cshtml
*/