using DocumentManagementWeb.Data;
using DocumentManagementWeb.Models;
using DocumentManagementWeb.VM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace DocumentManagementWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IWebHostEnvironment _webHostEnvironment;
        public UserManager<IdentityUser> _userManager;
        public ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            var email = User.Identity.Name;
            MainVM mainVM = new MainVM();
            if (email != null)
            {
                if (Directory.Exists(_webHostEnvironment.ContentRootPath + "\\UserFolders\\" + $"{email}\\"))
                {
                    string[] filePaths = Directory.GetFiles(_webHostEnvironment.ContentRootPath + "\\UserFolders\\" + $"{email}\\");
                    mainVM.files = new List<FileModel>();
                    foreach (string filePath in filePaths)
                    {
                        FileInfo file = new FileInfo(filePath);

                        mainVM.files.Add(new FileModel { FileName = Path.GetFileName(filePath), CreatedAt = file.CreationTime });

                    }

                    mainVM.notifications = _db.Notifications.Where(u => u.applicationUser.Email == email);
                    return View(mainVM);
                }


            }
            return View();
        }



        // POST: Devices/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteConfirmed(int id)
        {
            if (_db.Notifications == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Devices'  is null.");
            }
            var Notification =  _db.Notifications.Find(id);
            if (Notification != null)
            {
                _db.Notifications.Remove(Notification);
            }

             _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public ActionResult DownloadFile(string filename)
        {

            //Build the File Path.
            var email = User.Identity.Name;
            
            string path = _webHostEnvironment.ContentRootPath + "\\UserFolders\\" + $"{email}\\" ;
            //Read the File data into Byte Array.
            string[] filePaths = Directory.GetFiles(path);
            path += filename;
            if(filePaths.Contains(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);

                //Send the File to Download.
                return File(bytes, "application/octet-stream", filename);

            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var email = User.Identity.Name;
            MainVM mainVM = new MainVM();
            if (email != null)
            {
                if (Directory.Exists(_webHostEnvironment.ContentRootPath + "\\UserFolders\\" + $"{email}\\"))
                {
                    string[] filePaths = Directory.GetFiles(_webHostEnvironment.ContentRootPath + "\\UserFolders\\" + $"{email}\\");
                    mainVM.files = new List<FileModel>();
                    foreach (string filePath in filePaths)
                    {
                        FileInfo file = new FileInfo(filePath);

                        mainVM.files.Add(new FileModel { FileName = Path.GetFileName(filePath), CreatedAt = file.CreationTime, UserName = email });

                    }
                    mainVM.notifications = _db.Notifications.Where(u => u.applicationUser.Email == email);
                    return Json(new
                    {
                        data = mainVM.files
                    });
                }

            }
            return View();

        }
        #endregion
    }
}