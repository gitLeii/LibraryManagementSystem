using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Get Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }
        // Get: Library/Create
        public IActionResult Create(string email)
        {
            Student student = new Student();
            student.Email = email;
            return View(student);
        }
        //Post:Library/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Name,Email,Faculty")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(student);
        }
        public async Task<IActionResult> Details(int id)
        {
            Student? student = await _context.Students.FindAsync(id);
            return View(student);
        }
    }
}

