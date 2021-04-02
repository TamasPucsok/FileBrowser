using FileSystemModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileBrowser
{
    public static class FileSystemManager
    {
        private static DirectoryItem rootItem;
        public static DirectoryItem RootItem { get => rootItem; }

        /// <summary>
        /// Maps the content of a directory to the file system models.
        /// </summary>
        /// <param name="rootPath"></param>
        public static DirectoryItem MapFileStructure(string path)
        {
            DirectoryItem root = new(path, null);
            root.MapDirectoryChildren();

            return root;
        }

        public static void MapRootItem(string path)
        {
            rootItem = MapFileStructure(path);
        }


    }
}
