using System.IO;
using System.Threading.Tasks;

namespace AssemblySoft.IO
{
    /// <summary>
    /// Client for common file directory tasks
    /// </summary>
    public partial class DirectoryClient
    {
        /// <summary>
        /// Copies a directory asyncronously
        /// </summary>
        /// <param name="sourceDirName">source directory name</param>
        /// <param name="destDirName">destination directory name</param>
        /// <param name="copySubDirs">whether to copy sub directories, defaults to false</param>
        /// <returns></returns>
        /// <remarks>Always overwrites destination files whether they exist or not</remarks>
        public static async Task DirectoryCopyAsync(string sourceDirName, string destDirName, bool copySubDirs = false)
        {

            try
            {
                // Get the subdirectories for the specified directory.
                var dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                var dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    var temppath = Path.Combine(destDirName, file.Name);


                    using (StreamReader sourceFile = File.OpenText(file.FullName))
                    {
                        using (StreamWriter destinationFile = File.CreateText(temppath))
                        {
                            await CopyFileAsync(sourceFile, destinationFile);
                        }
                    }

                    file.CopyTo(temppath, true);

                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (var subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new IOException(e.Message, e);
            }
        }

        /// <summary>
        /// Read and Writes a stream asyncronously
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        private static async Task CopyFileAsync(StreamReader sourceFile, StreamWriter destinationFile)
        {
            try
            {
                char[] buffer = new char[0x1000];
                int numRead;
                while ((numRead = await sourceFile.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await destinationFile.WriteAsync(buffer, 0, numRead);
                }
            }
            catch (System.Exception e)
            {
                throw new IOException(e.Message, e);
            }
        }

        /// <summary>
        /// Copies a directory and optionally sub directories
        /// </summary>
        /// <param name="sourceDirName">source directory name</param>
        /// <param name="destDirName">destination directory name</param>
        /// <param name="copySubDirs">whether to copy subdirectories, defaults to false</param>
        /// <param name="overwriteExistingFiles">whether to overwrite existing files</param>
        /// <remarks>syncronous operation, possibly quicker than async if files remain on the same drive as only headers require updating</remarks>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = false, bool overwriteExistingFiles = true)
        {
            try
            {
                var dir = new DirectoryInfo(sourceDirName); // get the subdirectories for the specified directory.

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                var dirs = dir.GetDirectories();
                if (!Directory.Exists(destDirName))
                { //... doesn't exist, create it.
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                var files = dir.GetFiles();
                foreach (var file in files)
                {
                    try
                    {
                        var destPath = Path.Combine(destDirName, file.Name);

                        if (!overwriteExistingFiles)
                        {//we do not want to overwrite the existing file if it exists

                            if (File.Exists(destPath))
                            {
                                continue; //ignore the file and avoid having to handle the io exception
                            }
                        }

                        file.CopyTo(destPath, overwriteExistingFiles);

                    }
                    catch(IOException)
                    {//handle files that already exist when the flag is set not to overwrite                        
                    }
                }

                if (copySubDirs)
                {//... copy subdirectories and contents to new location.
                    foreach (var subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs, overwriteExistingFiles);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new IOException(e.Message, e);
            }
        }

        /// <summary>
        /// Copies an existing directory structure into an empty directory structure
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public static void CreateEmptyDirectoryStructureFromDirectory(string sourcePath, string destinationPath)
        {
            try
            {
                foreach (var dirPath in Directory.GetDirectories(sourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }
            catch (System.Exception e)
            {
                throw new IOException(e.Message, e);
            }
        }
    }
}
