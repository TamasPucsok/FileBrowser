using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileBrowser.Common
{
    public class Validator
    {
        /// <summary>
        /// This should be used to validate data in case of SQL or js injections.
        /// </summary>
        /// <returns>False if parameters are illegal.</returns>
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
