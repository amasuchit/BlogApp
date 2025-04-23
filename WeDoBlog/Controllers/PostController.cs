using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeDoBlog.Data;
using WeDoBlog.Models;
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


        [HttpPost]
        public JsonResult AddComment([FromBody]Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            context.Add(comment);
            context.SaveChanges();

            return Json(new 
            {
                success = true,
                username = comment.UserName,
                commentDate = comment.CommentDate.ToString("MMM dd, yyyy"),
                content = comment.Content
            });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }

            var datafromdb= await context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (datafromdb == null)
            {
                return NotFound();
            }
            EditViewModel editViewModel = new EditViewModel
            {
                Post = datafromdb,
                Categories = context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult > Edit(EditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            var postFromDb = await context.Posts.FirstOrDefaultAsync(p => p.Id == editViewModel.Post.Id);
            if (postFromDb == null)
            {
                return NotFound();
            }
            if (editViewModel.FeatureImage!=null)
            {
                var inputFileExtension = Path.GetExtension(editViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = allowedextensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("FeatureImage", "Invalid file type. Allowed types are: .jpg, .jpeg, .png");
                    return View(editViewModel);
                }
                var existingfilePath = Path.Combine(webHostEnvironment.WebRootPath,"Images", Path.GetFileName(postFromDb.FeatureImagePath));
                if (System.IO.File.Exists(existingfilePath))
                {
                    System.IO.File.Delete(existingfilePath);
                }

                editViewModel.Post.FeatureImagePath = await UploadFiletoFolder(editViewModel.FeatureImage);

            }
            else
            {
                editViewModel.Post.FeatureImagePath = postFromDb.FeatureImagePath;
            }
            context.Posts.Update(editViewModel.Post);
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Post");

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
