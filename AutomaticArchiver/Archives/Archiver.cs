using System.IO.Compression;
using AutomaticArchiver.Tasks;

namespace AutomaticArchiver.Archives
{
    public static class Archiver
    {
        public static void Archive(ArchiveTask task)
        {
            DirectoryInfo targetDirectory = new DirectoryInfo(task.TargetDirectory);
            string targetFullName = targetDirectory.FullName + '\\' + task.TargetName + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";

            FileStream stream = new FileStream(targetFullName, FileMode.Create, FileAccess.Write);
            ZipArchive archieve = new ZipArchive(stream, ZipArchiveMode.Create);
            if(task is ArchiveFileTask taskSingle)
            {
                FileInfo file = new FileInfo(taskSingle.SourceDirectory + '\\' + taskSingle.SourceFileName);
                AddFileToArchieve(archieve, file, string.Empty);
            }
            else if(task is ArchiveDirectoryTask archiveDirectory)
            {
                DirectoryInfo sourceDirectory = new DirectoryInfo(task.SourceDirectory);
                string entryNameBase = string.Empty;
                if(archiveDirectory.IncludeSourceDirectory)
                    entryNameBase = sourceDirectory.Name + '\\';

                AddDirectoryToArchieve(archieve, sourceDirectory, entryNameBase);
            }
            else
            {
                throw new NotImplementedException();
            }
            archieve.Dispose();
            stream.Dispose();

            Cleaner.CleanUp(targetDirectory, task.TargetName);
        }



        private static void AddDirectoryToArchieve(ZipArchive archive, DirectoryInfo sourceDirectory, string entryNameBase)
        {
            FileInfo[] files = sourceDirectory.GetFiles();
            foreach(FileInfo file in files)
                AddFileToArchieve(archive, file, entryNameBase);

            DirectoryInfo[] subdirectories = sourceDirectory.GetDirectories();
            foreach(DirectoryInfo subdirectory in subdirectories)
                AddDirectoryToArchieve(archive, subdirectory, entryNameBase + subdirectory.Name + '\\');
        }

        private static void AddFileToArchieve(ZipArchive archive, FileInfo sourceFile, string entryNameBase)
        {
            string entryName = entryNameBase + sourceFile.Name;
            archive.CreateEntryFromFile(sourceFile.FullName, entryName);
        }
    }
}
