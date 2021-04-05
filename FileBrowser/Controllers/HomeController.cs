using FileBrowser.Common;
using FileBrowser.Models;
using FileSystemModels;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            List<TreeViewItemModel> treeModel = GetTreeViewItemData();
            ViewBag.inlineItemTreeData = treeModel;
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

        public async Task<ActionResult> GetSize()
        {
            return View();
        }

        //[HttpGet("DownloadItem")]
        public FileStreamResult DownloadItem(string address, int pathHash, bool bOpenInBrowser)
        {
            if (!new Validator().DownloadItem_Validate(address, pathHash))
            {
                return null;
            }


            FileSystemItemBase item = FileSystemManager.RootItem.FindSubItem(address, ref pathHash);

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
        public async Task<IActionResult> UploadItem(string targetDir, int hash, List<IFormFile> files)
        {
            var size = files.Sum(x => x.Length);

            FileSystemItemBase item = FileSystemManager.RootItem.FindSubItem(targetDir, ref hash);
            if (item is DirectoryItem dirItem && new Validator().UploadItem_Validate(targetDir, hash, files))
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
                            new FileItem(filePath, dirItem).ProcessUpload();
                        }
                    }
                }

                return Ok(new { files.Count, size, filePaths });
            }

            return BadRequest();
        }

        public List<TreeViewItemModel> GetTreeViewItemData()
        {
            return FileSystemManager.RootItem.BuildTreeViewModel();
        }

        /**This is only a placeholder to test the download function*/
        public ActionResult DownloadOpenable(bool bOpenInBrowser = true)
        {
            FileSystemItemBase item = FileSystemManager.RootItem.Children.Where(x => x.Name == "CV.pdf").FirstOrDefault();

            item.ProcessDownload();

            var fPath = item.GetPath();

            FileStream fstream = new FileStream(fPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            string mimeType = MimeTypes.GetMimeType(fPath);

            if (mimeType == MimeTypes.FallbackMimeType || !bOpenInBrowser)
                return File(fstream, mimeType, item.Name);
            else
                return File(fstream, mimeType);
        }

        //This is only a placeholder to test the download function
        public ActionResult DownloadNonOpenable(bool bOpenInBrowser = true)
        {
            FileSystemItemBase item = FileSystemManager.RootItem.Children.Where(x => x.Name == "GitHubDesktopSetup.exe").FirstOrDefault();

            item.ProcessDownload();

            var fPath = item.GetPath();

            FileStream fstream = new FileStream(fPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            string mimeType = MimeTypes.GetMimeType(fPath);

            if (mimeType == MimeTypes.FallbackMimeType || !bOpenInBrowser)
                return File(fstream, mimeType, item.Name);
            else
                return File(fstream, mimeType);
        }
    }
}
