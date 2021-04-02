using FileSystemModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileBrowser.Providers
{
    /// <summary>
    /// Provides data regarding directories.
    /// </summary>
    public static class DirectoryDataProvider
    {


        public static string[] GetAllDirectoriesFromLocation()
        {
            return Directory.GetDirectories(ConfigurationGetter.GetFileStorageRootDirectory(), "*", SearchOption.AllDirectories);
        }

        public static string[] GetAllFilesFromLocation()
        {
            return Directory.GetFiles(ConfigurationGetter.GetFileStorageRootDirectory(), "*", SearchOption.AllDirectories);
        }

        
    }
}
