using DocumentManagementWeb.Data;
using DocumentManagementWeb.Models;
using DocumentManagementWeb.Utility;
using DocumentManagementWeb.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace DocumentManagementWeb.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminPannelController : Controller
    {
        public IWebHostEnvironment _webHostEnvironment;
        public ApplicationDbContext _db;
        public AdminPannelController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext db)
        {
            _webHostEnvironment = webHostEnvironment;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormFile file, string email, string notification)
        {
            var emails = _db.Users.ToList();
            var filePath = _webHostEnvironment.ContentRootPath + "\\UserFolders\\";
            filePath += $"{email}\\";

            



            if (emails.Where(u => u.UserName == email).IsNullOrEmpty())
            {
                ModelState.AddModelError("Email Doesn't Exist", "This Email Doesn't Exist , Please Enter A Valid Email.");

            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);

            }
            if (ModelState.IsValid)
            {
                filePath += $"{file.FileName}";
                if (filePath.Length >= 260)
                {
                    ModelState.AddModelError("name Too Large", "This File Name Is Too Large Please Rename It.");
                    return View();

                }
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                Notification notify = new Notification()
                {
                    applicationUser = _db.Users.FirstOrDefault(u => u.Email == email),

                    Msg = notification
                };
                //_db.FileModels.add
                FileModel fileModel = new()
                {
                    FileName = file.FileName,
                    CreatedAt = DateTime.Now,
                    UserName = email
                };
                _db.FileModels.Add(fileModel);
                _db.Notifications.Add(notify);
                _db.SaveChanges();


            }


            return View();
        }
        public IActionResult Pannel()
        {
            string path = _webHostEnvironment.ContentRootPath + "\\UserFolders\\";
            MainVM mainVM = new MainVM();
            mainVM.files = new List<FileModel>();

            if (Directory.Exists(path))
            {
                var emails = _db.Users.ToList();
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

                foreach (string dir in dirs)
                {

                    string[] filePaths = Directory.GetFiles(dir);
                    foreach (string filePath in filePaths)
                    {
                        FileInfo file = new FileInfo(filePath);


                        mainVM.files.Add(new FileModel { FileName = Path.GetFileName(filePath), CreatedAt = file.CreationTime, UserName = dir.Replace($"{path}", "") });

                    }
                }
                return View(mainVM);

            }
            return View();
        }



        public IActionResult DownloadFile(string filename)
        {
            if (ModelState.IsValid)
            {



                //Build the File Path.
                var file = _db.FileModels.FirstOrDefault(u => u.FileName == filename);
                if (file != null)
                {
                    var email = file.UserName;

                    string path = _webHostEnvironment.ContentRootPath + "\\UserFolders\\" + $"{email}\\";
                    //Read the File data into Byte Array.
                    string[] filePaths = Directory.GetFiles(path);
                    path += filename;

                    if (filePaths.Contains(path))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path);

                        //Send the File to Download.
                        return File(bytes, "application/octet-stream", filename);

                    }
                    else
                    {
                        return RedirectToAction(nameof(Pannel));

                    }
                }
                else
                {
                    return RedirectToAction(nameof(Pannel));
                }

            }
            else
            {
                return RedirectToAction(nameof(Pannel));
            }

        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            string path = _webHostEnvironment.ContentRootPath + "\\UserFolders\\";
            MainVM mainVM = new MainVM();
            mainVM.files = new List<FileModel>();

            if (Directory.Exists(path))
            {

                var emails = _db.Users.ToList();
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

                foreach (string dir in dirs)
                {

                    string[] filePaths = Directory.GetFiles(dir);
                    foreach (string filePath in filePaths)
                    {
                        FileInfo file = new FileInfo(filePath);


                        mainVM.files.Add(new FileModel { FileName = Path.GetFileName(filePath), CreatedAt = file.CreationTime, UserName = dir.Replace($"{path}", "") });

                    }

                }
                return Json(new
                {
                    data = mainVM.files
                });
            }
            return View();

        }
        #endregion


    }
}
