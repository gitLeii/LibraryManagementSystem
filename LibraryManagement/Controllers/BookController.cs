using LibraryManagement.Data;
using LibraryManagement.IService;
using LibraryManagement.Models;
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
        public IActionResult GetBooks()
        {
            var bookData = _context.Books.ToList<Book>();
            var jsonData = new { data = bookData };
            return Json(jsonData, new Newtonsoft.Json.JsonSerializerSettings());
            /*var books = _context.Books.ToList();
            return new JsonResult(books);*/
        }
        [HttpGet]
        public Book Get(int id)
        {
            return _bookService.GetById(id);
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
    }
}
