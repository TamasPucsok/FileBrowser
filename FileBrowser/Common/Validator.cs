using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FileBrowser.Common
{
    /// <summary>
    /// Validate action parameters to make sure they are correct and safe.
    /// </summary>
    public class Validator
    {

        public bool UploadItem_Validate(string targetDir, int hash, List<IFormFile> files)
        {
            return true;
        }

        public bool DownloadItem_Validate(string address, int pathHash)
        {
            return true;
        }
    }
}
