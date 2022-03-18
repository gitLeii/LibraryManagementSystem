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
            var issue = _context.Issues;
                /*.Where(x => x.Status == Status.Reserved)
                .ToList();*/
            var query = _context.Settings.ToList();
            if (query.Count() == 0)
            {
                ReserveSettings settings = new ReserveSettings { MaximumAllowedBooks = 0};
                _context.Add(settings);
                _context.SaveChanges();
                ViewBag.Settings = settings.MaximumAllowedBooks.ToString();
            }
            else
            {
                ReserveSettings setttings1 = query.FirstOrDefault();
                ViewBag.Settings = setttings1.MaximumAllowedBooks.ToString();
            }
             
            return View(issue);
        }
        [HttpPost]
        public IActionResult MaxIssues(int maxReserve)
        {
            var query = _context.Settings.ToList();
            if(query.Count() !=0 )
            {
                var changeVal = query.FirstOrDefault();
                ReserveSettings settings = _context.Settings.Find(changeVal.ID);
                settings.MaximumAllowedBooks = maxReserve;
                _context.SaveChanges();                
            }
            return RedirectToAction("IssueRequests");
        }
        [HttpPost]
        public IActionResult Issue(int id)
        {
            ////
            
            ////
            var query = from x in _context.Issues
                        where x.IssueId == id
                        select x;
            Issue issue = query.FirstOrDefault();
            issue.Status = Status.Issued;

            //query for books partial
            var queryBooks = from x in _context.AllBooks
                        where issue.BooksPartialId == x.Id
                        select x;
            BooksPartial books = queryBooks.FirstOrDefault();

            // check if others have reserved the book before 
            var checkReserved = from x in _context.Issues
                                where issue.BooksPartialId == x.BooksPartialId
                                && x.Status == Status.Reserved
                                select x;
            //
            books.Status = Status.Issued;
            //
            _context.SaveChanges();
            return RedirectToAction("IssueRequests"); 
        }

        [HttpGet]
        public IActionResult AddBooks(int id)
        {
            var email = HttpContext.User.Identity.Name;
            if (checkUser(email) == false)
            {
                return Redirect("/Identity/Account/Login");
            }
            BooksPartial book = new BooksPartial();
            book.BookId = id;
            var query = from x in _context.Books
                        where id == x.BookId
                        select x;
            //
            var checkPartialBooks = from x in _context.AllBooks
                                    where x.BookId == id
                                    select x;
            if(checkPartialBooks.Any())
            {
                int quantity = query.FirstOrDefault().Quantity - checkPartialBooks.Count();
                ViewBag.Quantity = quantity; 
            }
            else
            {
                ViewBag.Quantity = query.First().Quantity;
            }            
            return View(book);
        }
        [HttpPost]        
        public IActionResult AddBooks(string[] BookNumber, int id)
        {
            List<BooksPartial> books = new List<BooksPartial>();
            for (int i = 0; i<BookNumber.Length; i++)
            {
                BooksPartial book = new BooksPartial
                {
                    BookId = id,
                    BookNumber = BookNumber[i],
                    Status = Status.Available
                };
            books.Add(book);
            }            
            _context.AddRange(books);
            _context.SaveChanges();
            return RedirectToAction("Details", new {id = id});
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            
            var query = from x in _context.AllBooks
                        where id == x.BookId
                        select x;

            ViewBag.bookList = query.ToList();
            var bookData = _context.Books.Find(id);

            // integrate books and partial books
            //
            Integrate(id,bookData);
            /*var integrateQuery = from x in _context.AllBooks
                                 where x.BookId == id
                                 select x;
            if(integrateQuery.Any())
            {
                if (bookData.Quantity != integrateQuery.Count())
                {
                    BooksPartial books = _context.AllBooks.Find(integrateQuery.LastOrDefault().Id);
                    _context.Remove(books);
                    _context.SaveChanges();
                }
            }         */   
            //
            //
            return View(bookData);
        }
        public void Integrate(int id, Book bookData)
        {
            // integrate books and partial books
            //
            var integrateQuery = from x in _context.AllBooks
                                 where x.BookId == id
                                 select x;
            if (integrateQuery.Any())
            {
                if (bookData.Quantity != integrateQuery.Count())
                {
                    BooksPartial books = _context.AllBooks.Find(integrateQuery.LastOrDefault().Id);
                    _context.Remove(books);
                    _context.SaveChanges();
                }
            }
            //
            //
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
            /*var query = from x in _context.Books
                        where x.Title == book.Title
                        && x.Publication == book.Publication
                        select x;
            if (query != null)
            {
                return View(book);
            }*/
            var query = from x in _context.Books
                        where x.Title == book.Title
                       // && x.Publication == book.Publication
                        select x;
            if(query.Count() == 0)
            {
                _bookService.Add(book);
                return RedirectToAction("AddBooks", new { id = book.BookId });
            }
            else
            {
                int id = query.FirstOrDefault().BookId;
                Book books = _context.Books.Find(id);
                books.Quantity = books.Quantity + book.Quantity;
                _context.SaveChanges();
                return RedirectToAction("AddBooks", new { id = id });
            }

            //return RedirectToAction("AddBooks", new { id = book.BookId });
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
        /*protected void ExecuteToSQL(string sqlQuery, int id)
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

        }*/
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
