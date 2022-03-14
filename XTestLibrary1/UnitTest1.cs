using LibraryManagement.Controllers;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Service;
using LibraryManagement.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace XTestLibrary1
{
    public class AdminControllerValidationTests
    {
        private readonly BookTitleValidation _validation;
        public AdminControllerValidationTests() => _validation = new BookTitleValidation();
        [Theory]
        [InlineData("Title")]
        [InlineData("TitleAgain")]
        public void AdminController_TitleValidation(string title)
        {
            Assert.False(_validation.IsValidTitle(title));           
        }
        [Theory]
        [InlineData("Masters")]
        [InlineData("DummyValue")]
        public void AdminController_BranchValidation(string branch)
        {
            Assert.False(_validation.IsValidBranch(branch));
        }
    }
}