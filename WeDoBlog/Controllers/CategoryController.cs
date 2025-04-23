using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeDoBlog.Data;
using WeDoBlog.Models.ViewModel;

namespace WeDoBlog.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext context;

        public CategoryController(AppDbContext context)
        {
            this.context = context;
        }


        public async Task<IActionResult> Index()
        {
            var category= await context.Categories.ToListAsync();
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }


         public IActionResult Create()
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            return View(categoryViewModel);
        }


         public IActionResult Edit()
        {
            return View();
        }



    }
}
