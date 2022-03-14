using LibraryManagement.Models;

namespace LibraryManagement.Validation
{
    public class BookTitleValidation
    {
        private const int maxTitleLength = 5;
                
        public bool IsValidTitle(string title)
        {
            var titleLength = title.Length;
            if (titleLength > maxTitleLength)
                return false;
            return true;
        }
        public bool IsValidBranch(string value)
        {
            List<Branch> branch = Enum.GetValues(typeof(Branch))
                             .Cast<Branch>()
                             .ToList();

            foreach (var item in branch)
            {
                if (item.ToString() == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
