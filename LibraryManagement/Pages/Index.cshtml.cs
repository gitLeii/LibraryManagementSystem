using LibraryManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult OnGetDisplay()
        {
            var books = _context.Books.ToList();
            return new JsonResult(books);
        }
        public void OnGet()
        {
        }
    }
}
