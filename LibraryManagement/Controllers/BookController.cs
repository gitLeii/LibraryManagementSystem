using LibraryManagement.Data;
using LibraryManagement.IService;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LibraryManagement.Controllers
{
    public class BookController : Controller
    {
        IBookService _bookService = null;
        private readonly ApplicationDbContext _context;
        public BookController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetBooks()
        {
            var bookData = _context.Books.ToList<Book>();
            var jsonData = new { data = bookData };
            return Json(jsonData, new Newtonsoft.Json.JsonSerializerSettings());
        }
        [HttpGet]
        public Book Get(int id)
        {
            return _bookService.GetById(id);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
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
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            _bookService.Add(book);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Update(Book book)
        {
            _bookService.Update(book);
        }
        [HttpDelete]
        public string Delete(int id)
        {
            return _bookService.Delete(id);
        }
        [HttpPost]
        public IActionResult Issue(Issue issue)
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                var query = from x in _context.Students
                            where email == x.Email
                            select x.Id;
                issue.StudentId = query.FirstOrDefault();
                issue.IssueDate = DateTime.Now;
                if(ModelState.IsValid)
                {
                    _context.Issues.Add(issue);
                    _context.SaveChanges();
                }
                return RedirectToAction("Details", new { id = issue.BookId });
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
