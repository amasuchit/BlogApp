using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeDoBlog.Data;
using WeDoBlog.Models;
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

        [HttpGet]
         public async Task<IActionResult> Create()
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            return View(categoryViewModel);
        }


        [HttpPost]
         public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {

            
            if (ModelState.IsValid)
            {
                var category= new Category
                {
                    Name = categoryViewModel.Name,
                    Description = categoryViewModel.Description
                };
                context.Categories.Add(category);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(categoryViewModel);
        }


         public IActionResult Edit(int id)
        {
            return View();
        }



    }
}
