using LibraryManagement.Data;
using LibraryManagement.IService;
using LibraryManagement.Models;
using LibraryManagement.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagement.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IBookService _bookService = null;
        private readonly ApplicationDbContext _context;
        private readonly BookTitleValidation _validation;
        public AdminController(IBookService bookService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _context = context;
            _validation = new BookTitleValidation();
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
        public IActionResult IssueRequests()
        {
            var issue = _context.Issues
                .Where(x => x.Status == 0)
                .ToList();
            return View(issue);
        }
        [HttpPost]
        public IActionResult Issue(int id)
        {
            string query = "Update Issues" +
                " Set [Status] = 1 " +
                "Where IssueId = @id";  
            ExecuteToSQL(query, id);
            return RedirectToAction("IssueRequests"); 
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
            if (!ModelState.IsValid)
            {
                return View(book);
            }
            /*if (!_validation.IsValidTitle(book.Title))
            {
                ModelState.AddModelError("Title", "Max Length of title must me 5");
                return View(book);
            }*/
            book.Branch.ToString();
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
            if (!ModelState.IsValid)
            {
                return View(book);
            }
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
            try
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
            catch (Exception ex)
            {

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
        protected void ExecuteToSQL(string sqlQuery, int id)
        {
            string constr = "Server=(localdb)\\mssqllocaldb;Database=LibraryManagement;Trusted_Connection=True;MultipleActiveResultSets=true";

            using (SqlConnection con = new(constr))
            {
                using (SqlCommand cmd = new(sqlQuery, con))
                {
                    cmd.Parameters.Add(new SqlParameter("id", id));
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }

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
