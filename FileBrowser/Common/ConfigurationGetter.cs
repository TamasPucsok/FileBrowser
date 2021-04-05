using System.IO;

namespace FileBrowser.Common

{
    /// <summary>
    /// Static class to provide configuration data when needed.
    /// This way, if the location of the config. source or the logic behind it changes this class will be the only one that is affected by it and it can be easily updated.
    /// </summary>
    public static class ConfigurationGetter
    {
        /// <summary>
        /// Returns the file storage location. Could be tied to a DB in case of user accounts or return it from a config file.
        /// For now it will just return the current directory for easier usage.
        /// </summary>
        public static string GetFileStorageRootDirectory()
        {
            //Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
            //Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)

            return Directory.GetCurrentDirectory() + "\\FileStorage";
        }
    }
}
