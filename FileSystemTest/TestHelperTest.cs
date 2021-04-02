using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemTest
{
    [TestClass]
    public class TestHelperTest
    {
        [TestMethod]
        public void TestFolderCreationAndRemoval()
        {
            TestHelper testHelper = new();
            testHelper.SetUpTestDirectory();

            Assert.IsTrue(testHelper.TestDirInfo.Exists);

            testHelper.CleanUpTestDirectory();

            Assert.IsFalse(testHelper.TestDirInfo.Exists);
        }
    }
}
