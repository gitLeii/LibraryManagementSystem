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
        public virtual ICollection<Issue>? Issues { get; set; }
    }

    public enum Status
    {
        Reserved,
        Issued,
        Available
    }
    public enum Branch
    {
        Masters,
        Bachelors,
        SchoolLevel
    }
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(100)]
        public string Author { get; set; }
        [Required]
        public Branch Branch { get; set; }
        public virtual ICollection<Issue>? Issues { get; set; }

    }
    public class Issue
    {
        public int IssueId { get; set; }
        public DateTime IssueDate { get; set; }
        public Status Status { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public virtual Student Student { get; set; }
    }
}
