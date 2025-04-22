using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeDoBlog.Data;
using WeDoBlog.Models.ViewModel;

namespace WeDoBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string[] allowedextensions = { ".jpg", ".png", ".jpeg" };

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index(int? categoryId)
        {
            var postQuery = context.Posts.Include(p => p.Category).AsQueryable();
            if(categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId);
            }
            var posts= postQuery.ToList();

            ViewBag.Categories = context.Categories.ToList();

            return View(posts);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var post = await context.Posts.Include(p => p.Category).Include(p=>p.Comments).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }



        [HttpGet]
        public IActionResult Create()
        {

            var postViewModel= new PostViewModel();
            postViewModel.Categories = context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            
            return View(postViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed= allowedextensions.Contains(inputFileExtension);
                if(!isAllowed)
                {
                    ModelState.AddModelError("FeatureImage", "Invalid file type. Allowed types are: .jpg, .jpeg, .png");
                    return View(postViewModel);
                }

                postViewModel.Post.FeatureImagePath = await UploadFiletoFolder(postViewModel.FeatureImage);
                await context.Posts.AddAsync(postViewModel.Post);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Post");



            }
            postViewModel.Categories = context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(postViewModel);
        }


        private  async Task<string> UploadFiletoFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName= Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var imagesFolderPath= Path.Combine(wwwRootPath, "images");
            if(!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }
            var filePath = Path.Combine(imagesFolderPath, fileName);
            try
            {
                await using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex) 
            {
                return "Error Uploading Image" + ex.Message;
            }

            return "/images/" + fileName;
        }



    }
}
