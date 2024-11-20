using System.IO.Compression;
using AutomaticArchivation.Ignoring;
using AutomaticArchivation.Tasks;

namespace AutomaticArchivation
{
	public static class Archiver
	{
		public static void Archive(ArchiveTask task, IgnorePattern? ignorePattern = null)
		{
			DirectoryInfo targetDirectory = new DirectoryInfo(task.TargetDirectory);
			string targetFullName = targetDirectory.FullName + '\\' + task.TargetName + '_' + DateTime.Now.Date.ToString("dd_MM_yyyy").Replace('/', '_') + ".zip";

			if(ignorePattern == null)
				ignorePattern = IgnorePattern.Empty;

			FileStream stream = new FileStream(targetFullName, FileMode.Create, FileAccess.Write);
			ZipArchive archieve = new ZipArchive(stream, ZipArchiveMode.Create);
			if(task is ArchiveFileTask taskSingle)
			{
				FileInfo file = new FileInfo(taskSingle.SourceDirectory + '\\' + taskSingle.SourceFileName);
				AddFileToArchieve(archieve, file, string.Empty, ignorePattern);
			}
			else if(task is ArchiveDirectoryTask archiveDirectory)
			{
				DirectoryInfo sourceDirectory = new DirectoryInfo(task.SourceDirectory);
				string entryNameBase = string.Empty;
				if(archiveDirectory.IncludeSourceDirectory)
					entryNameBase = sourceDirectory.Name + '\\';

				AddDirectoryToArchieve(archieve, sourceDirectory, entryNameBase, ignorePattern);
			}
			else
			{
				throw new NotImplementedException();
			}
			archieve.Dispose();
			stream.Dispose();
		}



		private static void AddDirectoryToArchieve(ZipArchive archive, DirectoryInfo sourceDirectory, string entryNameBase, IgnorePattern ignorePattern)
		{
			FileInfo[] files = sourceDirectory.GetFiles();
			foreach(FileInfo file in files)
			{
				if(ignorePattern.IsMatch(file))
					continue;

				AddFileToArchieve(archive, file, entryNameBase, ignorePattern);
			}

			DirectoryInfo[] subdirectories = sourceDirectory.GetDirectories();
			foreach(DirectoryInfo subdirectory in subdirectories)
			{
				if(ignorePattern.IsMatch(subdirectory))
					continue;

				AddDirectoryToArchieve(archive, subdirectory, entryNameBase + subdirectory.Name + '\\', ignorePattern);
			}
		}

		private static void AddFileToArchieve(ZipArchive archive, FileInfo sourceFile, string entryNameBase, IgnorePattern ignorePattern)
		{
			string entryName = entryNameBase + sourceFile.Name;
			archive.CreateEntryFromFile(sourceFile.FullName, entryName);
		}
	}
}
