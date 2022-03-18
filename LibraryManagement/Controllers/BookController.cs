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
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                if (email == "admin@admin.com")
                {
                    return RedirectToAction("Details", "Admin", new { id = id });
                }
            }
            var query = from x in _context.AllBooks
                        where id == x.BookId
                        select x;
            if (query.ToList().Count() == 0)
            {
                message = "No Books Available";
            }
            ViewBag.bookList = query.ToList();
            ViewBag.Message = message;
            var bookData = _context.Books.Find(id);
            return View(bookData);
        }
        /*[HttpPost]
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
                               && issue.Status == Status.Issued
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
        }*/
        [HttpPost]
        public IActionResult Reserve(Issue issue)
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
                var checkAllBooks = from x in _context.AllBooks
                                     where issue.BookId == x.BookId
                                     select x;
                if (checkAllBooks.ToList().Count() == 0)
                {
                    return RedirectToAction("Details", new { id = issue.BookId, message = "No Books Available" });
                }    
                // Check if book is available
                var checkAvailable = from x in _context.AllBooks
                                     where issue.BookId == x.BookId
                                     && x.Status == Status.Available                                     
                                     select x;                
                if (checkAvailable.ToList().Count() == 0)
                {
                    return RedirectToAction("Details", new { id = issue.BookId, message = "This Book is already Reserved" });

                }
                else
                {
                    //
                    // check if the book is already issued
                    var checkReserve = from x in _context.Issues
                                       where issue.StudentId == x.StudentId
                                       && issue.BookId == x.BookId
                                       select x;
                    var check = checkReserve.ToList();

                    if (check.Count() != 0)
                    {
                        return RedirectToAction("Details", new { id = issue.BookId, message = "This Book is already Reserved by you" });
                    }
                }
                issue.BooksPartialId = checkAvailable.ToList().FirstOrDefault().Id;
                //
                // check if max issues is reached
                var checkMax = from x in _context.Issues
                               where issue.StudentId == x.StudentId
                               select x;
                var checkMaxQuery = _context.Settings.FirstOrDefault();
                if (checkMax.ToList().Count() >= checkMaxQuery.MaximumAllowedBooks)
                {
                    return RedirectToAction("Details", new { id = issue.BookId, message = "You have reached Maximum number of reserved. Return books to reserve more." });
                }
                 issue.Status = Status.Reserved;
                //
                var queryBooks = from x in _context.AllBooks
                                 where issue.BooksPartialId == checkAvailable.ToList().FirstOrDefault().Id
                                 select x;
                BooksPartial books = queryBooks.FirstOrDefault();
                books.Status = Status.Reserved;
                //
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
                        where id == x.BooksPartialId
                        select x;
            int Id = query.FirstOrDefault().IssueId;
            int stuID = query.FirstOrDefault().StudentId; 
            var issue = _context.Issues.Find(Id);
            var queryBooks = from x in _context.AllBooks
                             where issue.BooksPartialId == id
                             select x;
            BooksPartial books = queryBooks.FirstOrDefault();
            books.Status = Status.Available;
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
