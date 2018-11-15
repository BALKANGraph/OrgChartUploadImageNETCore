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
using Newtonsoft.Json;

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
            var links = new Dictionary<int, string>();
            var nodes = new Dictionary<int, string>();

            if (!cache.TryGetValue("links", out links))
            {
                links = new Dictionary<int, string>();

                links.Add(2, JsonConvert.SerializeObject(new { from = 2, to = 1}));
                links.Add(3, JsonConvert.SerializeObject(new { from = 3, to = 1}));

                cache.Set("links", links);
            }

            if (!cache.TryGetValue("nodes", out nodes))
            {
                nodes = new Dictionary<int, string>();

                nodes.Add(1, JsonConvert.SerializeObject(new { id = 1, FullName = "Rory White", Title = "CEO" }));
                nodes.Add(2, JsonConvert.SerializeObject(new { id = 2, FullName = "Jess John", Title = "IT" }));
                nodes.Add(3, JsonConvert.SerializeObject(new { id = 3, FullName = "Gail Talley", Title = "Marketing", Image = "//balkangraph.com/js/img/1.jpg" }));

                cache.Set("nodes", nodes);
            }

            return Json(new
            {
                links = links.Select(p => JsonConvert.DeserializeObject(p.Value)),
                nodes = nodes.Select(p => JsonConvert.DeserializeObject(p.Value))
            }, new JsonSerializerSettings(){ MaxDepth = 2 });
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
