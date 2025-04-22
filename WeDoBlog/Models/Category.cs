using System.ComponentModel.DataAnnotations;

namespace WeDoBlog.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(200, ErrorMessage = "Category name cannot be more than 200 characters")]
        public string Name { get; set; }

        public string? Description { get; set; }


        public ICollection<Post> Posts { get; set; }
    }
}
