using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileSystemModels
{
    public class FileItem : FileSystemItemBase
    {
        protected FileInfo info;
        protected FileInfo Info
        {
            get
            {
                info.Refresh();
                return info;
            }
            set => info = value;
        }

        public override string Name => Info.Name;

        public FileItem(string path, FileSystemItemBase parent) : base(parent)
        {
            Info = new FileInfo(path);
        }

        public override long GetSizeInBytes() => Info.Length;

        public override void CreateCopy()
        {
            string copyPath = GetNewCopyName();

            if (copyPath != null)
            {
                File.Copy(Info.FullName, copyPath);
                new FileItem(copyPath, Parent);
            }
        }

        public override bool Delete()
        {
            int undeletedChildren = 0;

            while (Children.Count > undeletedChildren)
            {
                if (Children[undeletedChildren].Delete())
                {
                    undeletedChildren++;
                }
            }

            if (undeletedChildren == 0)
            {
                try 
                { 
                    Info.Delete();
                }
                catch (IOException)
                {
                    return false;
                }

                RemoveReferencesInParent();
                return true;
            }

            return false;
        }

        public override bool Move(FileSystemItemBase item, bool bOverWrite)
        {
            try
            {
                if (item is DirectoryItem dirItem)
                {
                    Info.MoveTo(Path.Combine(dirItem.GetPath(), Info.Name), bOverWrite);
                    RemoveReferencesInParent();
                    item.Children.Add(this);
                    parent = item;
                    CalculateAddress();

                    return true;
                }
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public override void Rename(string newName)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(Info.FullName, newName);
        }

        public override string GetPath()
        {
            return Info.FullName; ;
        }

        protected override string GetNewCopyName()
        {
            string fileName = Path.GetFileNameWithoutExtension(Info.FullName);

            List<string> splittedFileName = fileName.Split(" ").ToList();
            if (splittedFileName.Count > 2 && splittedFileName[0] == "Copy" && splittedFileName[1] == "of")
            {
                if (splittedFileName.Count > 3 && Regex.IsMatch(splittedFileName[^1], @"[(]\d{1,}[)]"))
                {
                    splittedFileName.RemoveAt(splittedFileName.Count - 1);
                }

                splittedFileName.RemoveRange(0, 2);

                fileName = splittedFileName[0];

                for (int i = 1; i < splittedFileName.Count; i++)
                {
                    fileName += " " + splittedFileName[i];
                }
            }

            string fileExtension = Path.GetExtension(Info.FullName);

            if (Info.Directory.GetFiles("Copy of " + fileName + fileExtension, SearchOption.TopDirectoryOnly).Length < 1)
            {
                return Path.Combine(Info.DirectoryName, "Copy of " + fileName + fileExtension);
            }
            else
            {
                List<int> itemCopyNumbers = new();

                foreach (FileInfo file in Info.Directory.GetFiles())
                {
                    splittedFileName = Path.GetFileNameWithoutExtension(file.FullName).Split(" ").ToList();
                    if (file.Name.StartsWith("Copy of " + fileName) && file.Name.EndsWith(fileExtension) && Regex.IsMatch(splittedFileName[^1], @"[(]\d{1,}[)]"))
                    {
                        itemCopyNumbers.Add(int.Parse(splittedFileName[^1].Trim(new char[] { '(', ')' })));
                    }
                }

                for (int filenum = 2; filenum <= int.MaxValue; filenum++)
                {
                    if (!itemCopyNumbers.Any(x => x == filenum))
                    {
                        return Path.Combine(Info.DirectoryName, "Copy of " + fileName + " (" + filenum + ")" + fileExtension);
                    }
                }
            }
            return null;
        }
    }
}
