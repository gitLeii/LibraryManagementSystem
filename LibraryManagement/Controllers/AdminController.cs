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
            Book book = _context.Books.Find(id);
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Book book)
        {
            _bookService.Update(book);
            return RedirectToAction("Details", new { id = book.BookId });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _bookService.Delete(id);
            return Redirect("/");
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
