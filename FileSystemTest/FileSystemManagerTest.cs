using FileBrowser;
using FileSystemModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FileSystemTest
{
    [TestClass]
    public class FileSystemManagerTest
    {
        [TestMethod]
        public void CreateFileStructureTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            for (int i = 1; i <= 3; i++)
            {
                testHelper.TestDirInfo.CreateSubdirectory("test" + i);
            }
            FileSystemManager.MapRootItem(testHelper.TestDirInfo.FullName);

            DirectoryItem root = FileSystemManager.RootItem;

            string rootFullName = root.GetPath();
            string testDirFullName = testHelper.TestDirInfo.FullName;
            Assert.IsTrue(
                rootFullName == testDirFullName
                && root.Children.Count == 3
                && root.Children.All(x => x.Name.Contains("test"))
                );

            testHelper.CleanUpTestDirectory();
        }
    }
}
