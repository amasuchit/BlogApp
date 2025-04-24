using System.ComponentModel.DataAnnotations;

namespace WeDoBlog.Models.ViewModel
{
    public class CategoryViewModel
    {

       

        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(200, ErrorMessage = "Category name cannot be more than 200 characters")]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
