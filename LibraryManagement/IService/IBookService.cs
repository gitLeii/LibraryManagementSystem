using LibraryManagement.Models;

namespace LibraryManagement.IService
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
        Book GetById(int BookId);
        void Add(Book book);
        void Update(Book book);
        string Delete(int BookId);
    }
}
