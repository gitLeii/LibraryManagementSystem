using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    [Authorize]
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
        public IActionResult Update(int id)
        {
            Student student = _context.Students.Find(id);
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
            return RedirectToAction("Profile", new { id = student.Id });
        }
        public async Task<IActionResult> Profile(int id)
        {
            var issued = from x in _context.Issues
                         where x.StudentId == id
                         select x;       
            ViewBag.Issued = issued.ToList();
            Student ? student = await _context.Students.FindAsync(id);
            return View(student);
        }
    }
}

