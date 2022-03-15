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
        [AllowAnonymous]
        public IActionResult Create(string email)
        {            
            if(email == null)
            {
                return Redirect("/Identity/Account/Register");
            }
            Student student = new Student();
            student.Email = email;
            return View(student);
        }
        [AllowAnonymous]
        //Post:Library/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(include: "Name,Email,Faculty")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return Redirect("/");
            }
            return View(student);
        }
        public IActionResult Update(int id)
        {
            var email = User.Identity.Name;
            if (Validate(id) != true)
            {
                return Redirect("/Identity/Account/Login");
            }
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
        public List<string> CheckState(int id)
        {
            var query = from x in _context.Issues
                        where id == x.StudentId
                        select x;
            var checkIssue = query.ToList();
            List<string> message = new List<string>();
            foreach (var x in checkIssue)
            {
                DateTime d1 = DateTime.Now; 
                DateTime d2 = x.IssueDate;
                int checkDate = DateTime.Compare(d1, d2);

                if (checkDate >= 10)
                {
                    Book? book = _context.Books.Find(x.BookId);
                    if (checkDate >= 15)
                    {
                        Student? student = _context.Students.Find(x.StudentId);
                        student.Fine = 15;
                        _context.Students.Update(student);
                        _context.SaveChanges();
                        string text1 = "Issue for Book:" + book.Title + "has exceeded  15 days. Fine of Rs 15 has been added. Return the book in time.";
                        message.Add(text1);
                    }
                    string text = "Issue for Book:" + book.Title + "has exceeded  10 days. Return the book in time.";
                    message.Add(text);
                }
            }
            return message;
        }
        public async Task<IActionResult> Profile(int id)
        {
            var email = User.Identity.Name;
            if (email == "admin@admin.com")
            {
                return RedirectToAction("IssueRequests", "Admin");
            }
            if (Validate(id) != true)
            {
                return Redirect("/Identity/Account/Login");
            }
            
            List<string> notices = CheckState(id);
            ViewBag.Message = notices;
            var issued = from x in _context.Issues
                         where x.StudentId == id
                         && x.Status == Status.Issued
                         select x;  
            var query = from x in _context.Issues
                        where x.StudentId == id
                        && x.Status == Status.Reserved
                        select x;
            ViewBag.Reserved = query.ToList();
            ViewBag.Issued = issued.ToList();
            Student ? student = await _context.Students.FindAsync(id);
            return View(student);
        }
        public bool Validate( int  id )
        {
            var userId = HttpContext.Session.GetString("ID"); 
            if ( id.ToString() != userId)
            {
                return false;
            }
            return true;
        }
    }
}

