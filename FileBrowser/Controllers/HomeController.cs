using FileBrowser.Providers;
using FileBrowser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using FileSystemModels;

namespace FileBrowser.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Basic_Usage()
        {
            return View();
        }

        //[HttpGet("DownloadItem")]
        public async Task<FileStreamResult> DownloadItem(string address, int pathHash, bool bOpenInBrowser )
        {
            FileSystemItemBase item = FileSystemManager.RootItem.FindSubItem(address,ref pathHash);

            item.ProcessDownload();

            var fPath = item.GetPath();

            FileStream fstream = new FileStream(fPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            string mimeType = MimeTypes.GetMimeType(fPath);

            if (mimeType == MimeTypes.FallbackMimeType || !bOpenInBrowser)
                return File(fstream, mimeType, item.Name);
            else
                return File(fstream, mimeType);
        }

        //[HttpPost("UploadFile")]
        public async Task<IActionResult> UploadItem(string address, int pathHash, List<IFormFile> files)
        {
            var size = files.Sum(x => x.Length);

            FileSystemItemBase item = FileSystemManager.RootItem.FindSubItem(address, ref pathHash);
            if (item is DirectoryItem dirItem)
            {
                var filePaths = new List<string>();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(dirItem.GetPath(), formFile.FileName);
                        filePaths.Add(filePath);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                            new FileItem(filePath, dirItem);
                        }
                    }
                }

                return Ok(new { files.Count, size, filePaths });
            }

            return BadRequest();
        }

        //public async Task<ActionResult> DownloadItem()
        //{
        //    FileInfo fInfo = new("E:\\Works\\Programming\\FileBrowser\\FileBrowser\\FileStorage\\CV.pdf");

        //    var fPath = fInfo.FullName;

        //    FileStream fstream = new FileStream(fPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        //    string mimeType = MimeTypes.GetMimeType(fPath);

        //    if (mimeType == MimeTypes.FallbackMimeType)
        //        return File(fstream, mimeType, fInfo.Name);
        //    else
        //        return File(fstream, mimeType);
        //}
    }
}
