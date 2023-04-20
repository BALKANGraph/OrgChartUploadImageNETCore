using Microsoft.AspNetCore.Mvc;
using OrgChartUploadImageNETCore.Models;
using System.Diagnostics;

namespace OrgChartUploadImageNETCore.Controllers
{
    public class HomeController : Controller
    {

        private IWebHostEnvironment _host;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _host = webHostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadPhoto(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var file = files.First();
            var path = Path.Combine(_host.WebRootPath, "photos", file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }            

            return Json(new
            {
                url = new Uri(new Uri(Request.Scheme + "://" + Request.Host.Value), Url.Content("~/photos/" + file.FileName)).ToString()
            });
        }
    }
}