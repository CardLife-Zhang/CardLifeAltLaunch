using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CardLifeAltLaunch
{
    /// <summary>
    /// Checks/sets up environment for patching on alternative path
    /// </summary>
    class CPrePatch
    {
        public CPrePatch(string aCardLifeLocation)//, Action aFinishedHandler)
        {
            CardLifeLocation = aCardLifeLocation;
            //FinishedHandler = aFinishedHandler;
        }

        public string CardLifeLocation
        {
            get;
        }

        public Action FinishedHandler
        {
            get;
        }

        /// <summary>
        /// Check that we have a suitable copy of files, and copy them if not.
        /// </summary>
        public bool EnsureSaneEnvironment()
        {

            if(!isModVersionPresent())
            {
                createModCopy();
            }
            // Check we're not totally borked.
            if (!isModVersionPresent())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create a copy of required files for use with modding.
        /// </summary>
        public void createModCopy()
        {
            string aCardLifeLoc = Properties.Settings.Default.CardLifeExeLocation;
            string aModCardLifeLoc = Properties.Settings.Default.ModFilenameBase + ".exe";
            
            System.IO.File.Copy(aCardLifeLoc, aModCardLifeLoc, true);
            string aModLifeDataLocation = Properties.Settings.Default.ModFilenameBase + "_Data";

            DirectoryCopy(Properties.Settings.Default.CardLifeDataLocation, aModLifeDataLocation, true);
        }


        /// <summary>
        /// Check to see if mod files/directories are present
        /// </summary>
        /// <returns>true if files are present</returns>
        private bool isModVersionPresent()
        {
            // Check to see if exe is present
            // if so, check that directory has right files in it.

            // CardLifeLocation = CardLife.exe
            //https://www.wpf-tutorial.com/misc/multi-threading-with-the-backgroundworker/

            string aModCardLifeLoc = Properties.Settings.Default.ModFilenameBase + ".exe"; //ModCardLife.exe
            if (!File.Exists(aModCardLifeLoc))
            {
                return false;
            }

            // Check the directory has the right filenames in it at least
            string aModLifeDataLocation = Properties.Settings.Default.ModFilenameBase + "_Data";
            return compareDirectories(Properties.Settings.Default.CardLifeDataLocation, aModLifeDataLocation, true);
        }

        /// <summary>
        /// Compare the contents of DirNameTwo to ensure files/dirs in DirNameOne are present.
        /// </summary>
        /// <param name="DirNameOne">First directory to compare</param>
        /// <param name="DirNameTwo">Second diretory to compare</param>
        /// <param name="checkSubDirs">Check subdirectories</param>
        /// <returns>true if file/directory names in DirNameOne are the same in DirNameTwo</returns>
        private bool compareDirectories(string DirNameOne, string DirNameTwo, bool checkSubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(DirNameOne);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + DirNameOne);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, comparison fails.
            if (!Directory.Exists(DirNameTwo))
            {
                return false;
            }

            // Get the files in the directory and compare
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(DirNameTwo, file.Name);
                // Check each file is in the destination
                if (!File.Exists(temppath))
                {
                    return false;
                }
            }

            // do the same for subdirectories
            if (checkSubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(DirNameTwo, subdir.Name);
                    if (!compareDirectories(subdir.FullName, temppath, checkSubDirs))
                    {
                        return false;
                    }
                }
            }
            // Comparisons all succeeded.
            return true;
        }

        /// <summary>
        /// Copy directory, optionally including subdirectories. See https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        /// <param name="sourceDirName">Source directory</param>
        /// <param name="destDirName">Destination directory</param>
        /// <param name="copySubDirs">Copy sub directories flag</param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
