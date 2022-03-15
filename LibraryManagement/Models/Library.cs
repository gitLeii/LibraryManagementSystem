using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public enum Faculty
    {
        Science,
        Management,
        Humanities
    }
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public Faculty Faculty { get; set; }
        public int Fine { get; set; } = 0;
        public virtual ICollection<Issue>? Issues { get; set; }
    }

    public enum Status
    {
        Available = 0,
        Reserved = 1,
        Issued= 2        
    }
    public enum Branch
    {
        Masters=0,
        Bachelors=1,
        SchoolLevel=2
    }
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "The value cannot exceed 5 characters. ")]
        public string Title { get; set; }
        [Required]
        [StringLength(100)]
        public string Publication { get; set; }
        [Required]
        [StringLength(100)]
        public string Author { get; set; }
        [Required]
        public Branch Branch { get; set; }
        [Required]
        public int Quantity { get; set; }
        public virtual ICollection<Issue>? Issues { get; set; }
    }
    public class BooksPartial
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BookNumber { get; set; }
        public Status? Status { get; set; }
        public int BookId { get; set; }
        public virtual ICollection<Book>? Books {get; set;} 
    }
    public class Issue
    {
        [Key]
        public int IssueId { get; set; }
        public DateTime IssueDate { get; set; }
        public Status Status { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public virtual Student? Student { get; set; }
    }
}
