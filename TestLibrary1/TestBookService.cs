using LibraryManagement.Controllers;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestLibrary1
{
    [TestClass]
    public class TestBookService
    {        
        [TestMethod]
        public void GetBooks_GetsListOfBookViaContext()
        {
            var data = new List<Book>
            {
                new Book { Author = "TestAuthor", Publication = "TestAuthor", Branch = Branch.Masters, Title="TestTitle", Quantity = 2 },
                new Book { Author = "TestAuthor", Publication = "TestAuthor1", Branch = Branch.Masters, Title="TestTitle1", Quantity = 2 },
                new Book { Author = "TestAuthor", Publication = "TestAuthor2", Branch = Branch.Masters, Title="TestTitle2", Quantity = 2 },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Book>>();
            mockSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Books).Returns(mockSet.Object);

            var service = new Mock<BookService>(mockContext.Object);

            var books = service.Object.GetAllBooks();

            Assert.AreEqual(3, books.Count);
            Assert.AreEqual("TestTitle", books[0].Title);
            Assert.AreEqual("TestTitle1", books[1].Title);
            Assert.AreEqual("TestTitle2", books[2].Title);
        }
        [TestMethod]
        public void CreateBooks_SaveBookViaContext()
        {
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);

            var service = new BookService(mockContext.Object);
            service.Add(new Book { Author = "TestAuthor", Publication = "TestAuthor", Branch = Branch.Masters, Title = "TestTitle", Quantity = 2 });

            mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        [TestMethod]
        public void DeletesBooks_RemovesBookViaContext()
        {            
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Books).Returns(mockSet.Object);

            var service = new BookService(mockContext.Object); 
            string deleted = service.Delete(9);
            Assert.AreEqual("Deleted", deleted);
        }
        [TestMethod]
        public void AdminController_Details_returns_viewWithDetailsOfBooks()
        {            
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Books).Returns(mockSet.Object);

            var service = new Mock<BookService>(mockContext.Object);
            var controller = new AdminController(service.Object, mockContext.Object);
            var result = controller.Details(9);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }        
        
    }
}