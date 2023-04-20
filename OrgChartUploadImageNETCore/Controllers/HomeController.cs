using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;


namespace OrgChartUploadImageNETCore.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache cache;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IMemoryCache memoryCache, IHostingEnvironment environment)
        {
            cache = memoryCache;
            hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public EmptyResult UpdateNode(int id, string nodeJson)
        {
            Dictionary<int, string> nodes = null;
            cache.TryGetValue("nodes", out nodes);
            nodes[id] = nodeJson;
            return new EmptyResult();
        }


        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var uniqueFileName = GetUniqueFileName(file.FileName);
            var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, uniqueFileName);
            
            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return Ok(new { url = Url.Content("~/uploads/" + uniqueFileName) });
        }

        public JsonResult Read()
        {
            var nodes = new Dictionary<int, string>();

            if (!cache.TryGetValue("nodes", out nodes))
            {
                nodes = new Dictionary<int, string>();
                
                nodes.Add(1, JsonSerializer.Serialize(new { id = 1, FullName = "Rory White", Title = "CEO" }));
                nodes.Add(2, JsonSerializer.Serialize(new { id = 2, pid = 1, FullName = "Jess John", Title = "IT" }));
                nodes.Add(3, JsonSerializer.Serialize(new { id = 3, pid = 1, FullName = "Gail Talley", Title = "Marketing", Image = "//balkangraph.com/js/img/1.jpg" }));

                cache.Set("nodes", nodes);
            }

            return Json(new 
            { 
                nodes = nodes.Select(p => JsonSerializer.Deserialize<Dictionary<int, string>>(p.Value)) 
            });
        }


        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
    }
}
