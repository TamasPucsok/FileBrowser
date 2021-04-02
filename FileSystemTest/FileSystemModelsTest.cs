using FileBrowser;
using FileSystemModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemTest
{
    [TestClass]
    public class FileSystemModelsTest
    {
        [TestMethod]
        public void FindAddressTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            testHelper.TestDirInfo.CreateSubdirectory("test" + 1);
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test1\\test.txt").Close();

            testHelper.TestDirInfo.CreateSubdirectory("test" + 2).CreateSubdirectory("test2_1");
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test2\\test2_1\\test1.txt").Close();
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test2\\test2_1\\test2.txt").Close();

            testHelper.TestDirInfo.CreateSubdirectory("test" + 3);

            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);

            DirectoryItem root = FileSystemManager.RootItem;

            var searchitem1 = root.Children[0].Children[0];
            var searchterm1_add = root.Children[0].Children[0].Address;
            var searchterm1_hsh = root.Children[0].Children[0].GetPath().GetHashCode();

            var searchitem2 = root.Children[1].Children[0].Children[0];
            var searchterm2_add = root.Children[1].Children[0].Children[0].Address;
            var searchterm2_hsh = root.Children[1].Children[0].Children[0].GetPath().GetHashCode();

            var searchitem3 = root.Children[1].Children[0].Children[1];
            var searchterm3_add = root.Children[1].Children[0].Children[1].Address;
            var searchterm3_hsh = root.Children[1].Children[0].Children[1].GetPath().GetHashCode();

            var result1 = root.FindSubItem(searchterm1_add,ref searchterm1_hsh);
            var result2 = root.FindSubItem(searchterm2_add, ref searchterm2_hsh);
            var result3 = root.FindSubItem(searchterm3_add, ref searchterm3_hsh);
            var nullresult = root.FindSubItem("0.0.0.0.0.0.0",ref searchterm3_hsh);

            Assert.AreEqual(searchitem1, result1);
            Assert.AreEqual(searchitem2, result2);
            Assert.AreEqual(searchitem3, result3);
            Assert.AreEqual(null, nullresult);

            testHelper.CleanUpTestDirectory();
        }

        [TestMethod]
        public void DeleteTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            testHelper.TestDirInfo.CreateSubdirectory("test" + 1);
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test1\\test.txt").Close();

            testHelper.TestDirInfo.CreateSubdirectory("test" + 2).CreateSubdirectory("test2_1");
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test2\\test2_1\\test1.txt").Close();

            StreamWriter openFile =File.CreateText(testHelper.TestDirInfo.FullName + "\\test2\\test2_1\\test2.txt");

            testHelper.TestDirInfo.CreateSubdirectory("test" + 3);

            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);

            DirectoryItem root = FileSystemManager.RootItem;

            Assert.IsFalse(root.Delete());
            
            openFile.Close();

            Assert.IsTrue(root.Delete());
        }

        [TestMethod]
        public void FileCopyTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            File.CreateText(testHelper.TestDirInfo.FullName + "\\test.txt").Close();
            File.CreateText(testHelper.TestDirInfo.FullName + "\\Copy of test (12).txt").Close();
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test (42).txt").Close();

            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);

            DirectoryItem root = FileSystemManager.RootItem;

            root.Children.Where(x => x.Name == "test.txt").FirstOrDefault().CreateCopy();
            root.Children.Where(x => x.Name == "Copy of test (12).txt").FirstOrDefault().CreateCopy();
            root.Children.Where(x => x.Name == "test (42).txt").FirstOrDefault().CreateCopy();

            Assert.IsTrue(root.Children.Count == 6);
            Assert.IsTrue(root.Children.Any(x => x.Name == "Copy of test.txt"));
            Assert.IsTrue(root.Children.Any(x => x.Name == "Copy of test (2).txt"));
            Assert.IsTrue(root.Children.Any(x => x.Name == "Copy of test (42).txt"));

            testHelper.CleanUpTestDirectory();
        }

        [TestMethod]
        public void DirectoryCopyTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            testHelper.TestDirInfo.CreateSubdirectory("Copy of test1 (2)");

            DirectoryInfo dir = testHelper.TestDirInfo.CreateSubdirectory("test" + 1);
            dir.CreateSubdirectory("test" + 1).CreateSubdirectory("test" + 1);
            dir.CreateSubdirectory("test" + 2);

            File.CreateText(testHelper.TestDirInfo.FullName + "\\test1\\test1\\test1\\test.txt").Close();
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test1\\test2\\Copy of test (12).txt").Close();

            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);

            DirectoryItem root = FileSystemManager.RootItem;

            root.Children.Where(x => x.Name == "test1").FirstOrDefault().CreateCopy();
            root.Children.Where(x => x.Name == "test1").FirstOrDefault().CreateCopy();

            Assert.IsTrue(root.Children.Count == 4);
            Assert.IsTrue(root.Children.Where(x => x.Name == "Copy of test1 (3)").FirstOrDefault().Children[1].Children.Any(x => x.Name == "Copy of test (12).txt"));
            Assert.IsTrue(root.Children.Any(x=>x.Name == "Copy of test1"));

            testHelper.CleanUpTestDirectory();
        }

        [TestMethod]
        public void CreateNewSubDirectoryTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);
            DirectoryItem root = FileSystemManager.RootItem;

            for (int i = 0; i < 3; i++)
            {
                root.CreateNewSubDirectory();
            }

            Assert.IsTrue(root.Children.Count == 3);
            Assert.IsTrue(root.Children.Any(x => x.Name == "New Folder"));
            Assert.IsTrue(root.Children.Any(x => x.Name == "New Folder (3)"));

            testHelper.CleanUpTestDirectory();
        }

        [TestMethod]
        public void FileMoveTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            testHelper.TestDirInfo.CreateSubdirectory("test");
            File.CreateText(testHelper.TestDirInfo.FullName + "\\test.txt").Close();

            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);
            DirectoryItem root = FileSystemManager.RootItem;

            root.Children.Where(x => x.Name == "test.txt").FirstOrDefault().Move(root.Children.Where(x => x.Name == "test").FirstOrDefault(), false);

            File.CreateText(testHelper.TestDirInfo.FullName + "\\test.txt").Close();
            new FileItem(testHelper.TestDirInfo.FullName + "\\test.txt", root);

            Assert.IsFalse(root.Children.Where(x => x.Name == "test.txt").FirstOrDefault().Move(root.Children.Where(x => x.Name == "test").FirstOrDefault(), false));
            Assert.IsTrue(root.Children.Where(x => x.Name == "test.txt").FirstOrDefault().Move(root.Children.Where(x => x.Name == "test").FirstOrDefault(), true));

            Assert.IsTrue(root.Children.Count == 1);
            Assert.IsTrue(root.Children[0].Children[0].Name == "test.txt");
            Assert.IsTrue(root.Children[0].Children[0].Parent == root.Children[0]);

            testHelper.CleanUpTestDirectory();
        }
    }
}
