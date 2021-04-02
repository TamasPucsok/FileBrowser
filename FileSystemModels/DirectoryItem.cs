using FileSystemModels.Common;
using FileSystemModels.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileSystemModels
{
    public class DirectoryItem : FileSystemItemBase
    {
        protected DirectoryInfo info;
        protected DirectoryInfo Info
        {
            get
            {
                info.Refresh();
                return info;
            }
            set => info = value;
        }

        public override string Name => Info.Name;

        public DirectoryItem(string path, FileSystemItemBase parent) : base(parent)
        {
            Info = new DirectoryInfo(path);
        }

        public override long GetSizeInBytes()
        {
            long result = 0;

            foreach (DirectoryInfo dir in Info.GetDirectories("*", SearchOption.AllDirectories))
            {
                result += dir.GetFiles().Sum(x => x.Length);
            }

            return result;
        }

        public void CreateNewSubDirectory()
        {

            if (Info.GetDirectories("New Folder", SearchOption.TopDirectoryOnly).Length < 1)
            {
                new DirectoryItem(Info.CreateSubdirectory("New Folder").FullName, this);
            }
            else
            {
                List<int> itemCopyNumbers = new();

                foreach (DirectoryInfo dir in Info.GetDirectories())
                {
                    List<string> splittedDirName = dir.Name.Split(" ").ToList();
                    if (dir.Name.StartsWith("New Folder ") && Regex.IsMatch(splittedDirName[^1], @"[(]\d{1,}[)]"))
                    {
                        itemCopyNumbers.Add(int.Parse(splittedDirName[^1].Trim(new char[] { '(', ')' })));
                    }
                }

                for (int dirnum = 2; dirnum <= int.MaxValue; dirnum++)
                {
                    if (!itemCopyNumbers.Any(x => x == dirnum))
                    {
                        new DirectoryItem(Info.CreateSubdirectory("New Folder (" + dirnum + ")").Name, this);
                        return;
                    }
                }
            }
        }

        public override void CreateCopy()
        {
            string copyName = GetNewCopyName();

            new DirectoryHandler().DirectoryDeepCopy(Info.FullName, copyName);

            new DirectoryItem(copyName, Parent).MapDirectoryChildren();
        }

        public override bool Delete()
        {
            int undeletedChildren = 0;

            while (Children.Count > undeletedChildren)
            {
                if (!Children[undeletedChildren].Delete())
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
            return false;
        }

        public override void Rename(string newName)
        {
            string newFullName = Path.Combine(parent.GetPath(), newName);

            if (Directory.Exists(newFullName))
                throw new NameAlreadyTakenException();
            else
            {
                new DirectoryHandler().DirectoryDeepCopy(GetPath(), newFullName);

                new DirectoryItem(newFullName, Parent).MapDirectoryChildren();

                this.Delete();
            }
        }

        public override string GetPath()
        {
            return Info.FullName;
        }

        public void MapDirectoryChildren()
        {
            DirectoryInfo rootInfo = new(GetPath());
            IEnumerable<string> subDirPaths = rootInfo.GetDirectories().Select(x => x.FullName);

            if (subDirPaths.Any())
            {
                foreach (string path in subDirPaths)
                {
                    DirectoryItem subDirItem = new(path, this);

                    subDirItem.MapDirectoryChildren();
                }
            }

            IEnumerable<string> subFilePaths = rootInfo.GetFiles().Select(x => x.FullName);

            if (subFilePaths.Any())
            {
                foreach (string path in subFilePaths)
                {
                    FileItem subFileItem = new(path, this);
                }
            }
        }

        protected override string GetNewCopyName()
        {
            string dirName = Info.Name;

            //Strip name from added copy tags to avoid naming them "Copy of Copy of..."
            List<string> splittedDirName = dirName.Split(" ").ToList();
            if (splittedDirName.Count > 2 && splittedDirName[0] == "Copy" && splittedDirName[1] == "of")
            {
                if (splittedDirName.Count > 3 && Regex.IsMatch(splittedDirName[^1], @"[(]\d{1,}[)]"))
                {
                    splittedDirName.RemoveAt(splittedDirName.Count - 1);
                }

                splittedDirName.RemoveRange(0, 2);

                dirName = splittedDirName[0];

                for (int i = 1; i < splittedDirName.Count; i++)
                {
                    dirName += " " + splittedDirName[i];
                }
            }

            if (Info.Parent.GetDirectories("Copy of " + dirName, SearchOption.TopDirectoryOnly).Length < 1)
            {
                return Path.Combine(Info.Parent.FullName, "Copy of " + dirName);
            }
            else
            {
                List<int> itemCopyNumbers = new();

                foreach (DirectoryInfo dir in Info.Parent.GetDirectories())
                {
                    splittedDirName = dir.Name.Split(" ").ToList();
                    if (dir.Name.StartsWith("Copy of " + dirName) && Regex.IsMatch(splittedDirName[^1], @"[(]\d{1,}[)]"))
                    {
                        itemCopyNumbers.Add(int.Parse(splittedDirName[^1].Trim(new char[] { '(', ')' })));
                    }
                }

                for (int filenum = 2; filenum <= int.MaxValue; filenum++)
                {
                    if (!itemCopyNumbers.Any(x => x == filenum))
                    {
                        return Path.Combine(Info.Parent.FullName, "Copy of " + dirName + " (" + filenum + ")");
                    }
                }
            }
            return null;
        }

    }
}
