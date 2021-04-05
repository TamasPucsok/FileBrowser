using System.IO;

namespace FileSystemTest
{
    internal class TestHelper
    {
        internal string testDirName = "UnitTestingTempDirectory";
        protected DirectoryInfo testDirInfo;
        internal DirectoryInfo TestDirInfo
        {
            get
            {
                testDirInfo.Refresh();
                return testDirInfo;
            }
            set => testDirInfo = value;
        }

        internal DirectoryInfo SetUpTestDirectory()
        {
            string loc = Directory.GetCurrentDirectory() + "\\" + testDirName;

            if (Directory.Exists(loc))
                Directory.Delete(loc, true);

            testDirInfo = new DirectoryInfo(Directory.GetCurrentDirectory()).CreateSubdirectory(testDirName);
            return testDirInfo;
        }

        internal void CleanUpTestDirectory()
        {
            testDirInfo.Delete(true);
        }
    }
}
