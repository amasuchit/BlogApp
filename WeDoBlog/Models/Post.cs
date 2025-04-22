using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeDoBlog.Models
{
    public class Post
    {


        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Title is Required")]
        [MaxLength(400, ErrorMessage = "Title cannot be more than 400 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is Required")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Author is Required")]
        [MaxLength(100, ErrorMessage = "Author Name cannot be more than 100 characters")]
        public string Author { get; set; }

        [ValidateNever]
        public string FeatureImagePath { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }=DateTime.Now;


        [ForeignKey("Category")]
        [Required(ErrorMessage = "Category is Required")]
        public int? CategoryId { get; set; }

        
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public ICollection<Comment> Comments { get; set; }
    }
}
