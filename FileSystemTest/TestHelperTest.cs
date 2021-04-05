using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileSystemTest
{
    [TestClass]
    public class TestHelperTest
    {
        [TestMethod]
        public void TestFolderCreationAndRemovalTest()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            Assert.IsTrue(testHelper.TestDirInfo.Exists);

            testHelper.CleanUpTestDirectory();

            Assert.IsFalse(testHelper.TestDirInfo.Exists);
        }
    }
}
