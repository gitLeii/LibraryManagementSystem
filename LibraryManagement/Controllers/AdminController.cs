using LibraryManagement.Data;
using LibraryManagement.IService;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IBookService _bookService = null;
        private readonly ApplicationDbContext _context;
        public AdminController(IBookService bookService, ApplicationDbContext context)
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
            var bookData = _context.Books.Find(id);
            return View(bookData);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var email = HttpContext.User.Identity.Name;
            if (checkUser(email) == false)
            {
                return Redirect("/Identity/Account/Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            Validate(book);
            _bookService.Add(book);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if ( id == null )
            {
                return RedirectToAction("Index");
            }
            var email = HttpContext.User.Identity.Name;
            if(checkUser(email) == false)
            {
                return Redirect("/Identity/Account/Login");
            }
            Book book = _context.Books.Find(id);
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Book book)
        {
            Validate(book);
            _bookService.Update(book);
            return RedirectToAction("Details", new { id = book.BookId });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _bookService.Delete(id);
            return Redirect("/");
        }
        public bool checkValidate(int id)
        {
            var userId = HttpContext.Session.GetString("ID");
            if (id.ToString() != userId)
            {
                HttpContext.Session.SetString("ID", "");
                return false;
            }
            return true;
        }
        public void Validate(Book book)
        {
            var query = from x in _context.Issues
                        where book.BookId == x.BookId
                        select x;
            int id = query.First().StudentId;
            if (checkValidate(id) != true)
            {
                Redirect("/Identity/Account/Login");
            }
        }
        public bool checkUser( string email )
        {
            if ( email == "admin@admin.com")
            {
                return true;
            }
            return false;
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
