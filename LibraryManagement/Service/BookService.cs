using LibraryManagement.Data;
using LibraryManagement.IService;
using LibraryManagement.Models;

namespace LibraryManagement.Service
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Delete(int bookId)
        {
            //var book = _context.Books.FirstOrDefault(x => x.BookId == bookId);
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return "Deleted";
        }
        public Book GetById(int bookId)
        {
            return _context.Books.SingleOrDefault(x => x.BookId == bookId);
        }
        public List<Book> GetAllBooks()
        {
            try
            {
                var books = _context.Books;
                if(books.Count() == 0)
                {
                    return new List<Book>();
                }
                return books.AsEnumerable().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void Add(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }
        public void Update(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }
    }
}
