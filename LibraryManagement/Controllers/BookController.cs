using LibraryManagement.Data;
using LibraryManagement.IService;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LibraryManagement.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        IBookService _bookService = null;
        private readonly ApplicationDbContext _context;
        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Admin");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Details(int id, string message = "")
        {
            ViewBag.Message = message;
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                if (email == "admin@admin.com")
                {
                    return RedirectToAction("Details", "Admin", new { id = id });
                }
            }
            var bookData = _context.Books.Find(id);
            return View(bookData);
        }
        [HttpPost]
        public IActionResult Issue(Issue issue)
        {
            if (User.Identity.IsAuthenticated)
            {
                // check user
                var email = User.Identity.Name;
                var query = from x in _context.Students
                            where email == x.Email
                            select x.Id;
                issue.StudentId = query.FirstOrDefault();
                issue.IssueDate = DateTime.Now;
                //
                // check if the book is already issued
                var checkIssue = from x in _context.Issues
                                 where issue.StudentId == x.StudentId
                                 && issue.BookId == x.BookId
                                 select x;
                var check = checkIssue.ToList();
                if (check.Count() != 0)
                {
                    return RedirectToAction("Details", new { id = issue.BookId, message = "This Book is already Issued" });
                }
                //
                // check if max issues is reached
                var checkMax = from x in _context.Issues
                               where issue.StudentId == x.StudentId
                               select x;
                if(checkMax.ToList().Count() >=3)
                {
                    return RedirectToAction("Details", new { id = issue.BookId, message = "You have reached Maximum no of issues. Return books to issue more." });
                }
                //
                if (ModelState.IsValid)
                {
                    _context.Issues.Add(issue);
                    _context.SaveChanges();
                }
                return RedirectToAction("Profile", "Student", new { id = issue.StudentId });                              
                
            }
            
            return Redirect("../Identity/Account/Login");
        }
        [HttpPost]
        public IActionResult Return(int id)
        {
            var query = from x in _context.Issues
                        where id == x.BookId
                        select x;
            int Id = query.FirstOrDefault().IssueId;
            int stuID = query.FirstOrDefault().StudentId;
            var issue = _context.Issues.Find(Id);
            if(issue != null)
            {
                _context.Issues.Remove(issue);
                _context.SaveChanges();
            }               
            return RedirectToAction("Profile", "Student", new { id = stuID });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
