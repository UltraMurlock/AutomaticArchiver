using System.Text.RegularExpressions;
using AutomaticArchivation;
using AutomaticArchivation.Ignoring;
using AutomaticArchivation.Tasks;

namespace AutomaticArchiver.ConsoleApp
{
	internal class Program
	{
		public static Regex FileTaskRegex = new Regex("^((file)|(f))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static Regex DirectoryTaskRegex = new Regex("^((directory)|(dir)|(d))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static Regex SourceDirectoryRegex = new Regex("^(((source)|(s))-((directory)|(dir)))=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static Regex SourceFileRegex = new Regex("^(((source)|(s))-((file)|(f)))=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static Regex TargetDirectoryRegex = new Regex("^(((target)|(t))-((directory)|(dir)))=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static Regex TargetArchiveNameRegex = new Regex("^(((target)|(t))-((name)|(n)))=", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static Regex IgnorePatternRegex = new Regex("^((ignore)|(ign))=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public static Regex IncludeSourceDirectoryRegex = new Regex("^(include-(source)|(s))=", RegexOptions.Compiled | RegexOptions.IgnoreCase);



		static void Main(string[] args)
		{
			while (true)
			{
				Console.WriteLine("Введите задачу:");
				string taskText = Console.ReadLine() ?? string.Empty;
				ArchiveTask? task = ParseTask(taskText);
				if(task == null)
					continue;

				IgnorePattern? ignorePattern = null;
				if(task.IgnorePattern == "VisualStudioC#")
					ignorePattern = IgnorePattern.VisualStudioCSharp;

				Archiver.Archive(task, ignorePattern);
				Console.WriteLine("Успешно");
			}
		}



		private static ArchiveTask? ParseTask(string taskText)
		{
			ArchiveTask task;

			Queue<string> queue = new Queue<string>(taskText.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList());

			string rawTaskType = queue.Dequeue();
			if(FileTaskRegex.IsMatch(rawTaskType))
			{
				task = new ArchiveFileTask();
			}
			else if(DirectoryTaskRegex.IsMatch(rawTaskType))
			{
				task = new ArchiveDirectoryTask();
			}
			else
			{
				Console.WriteLine($"Ожидался тип задачи. Получено {rawTaskType[0]}");
				return null;
			}

			while(queue.Count > 0)
			{
				string parameter = queue.Dequeue();
				if(task is ArchiveDirectoryTask && SourceDirectoryRegex.IsMatch(parameter))
				{
					task.SourceDirectory = parameter.Split('=')[1];
					continue;
				}

				if(task is ArchiveFileTask archiveFileTask && SourceFileRegex.IsMatch(parameter))
				{
					string fullPath = parameter.Split('=')[1];
					int lastDelimiter = fullPath.LastIndexOf('\\');
					if(lastDelimiter < 0) lastDelimiter = fullPath.LastIndexOf('/');
					string directoryName = fullPath.Substring(0, lastDelimiter);
					string fileName = fullPath.Substring(lastDelimiter + 1, fullPath.Length - lastDelimiter - 1);
					task.SourceDirectory = directoryName;
					archiveFileTask.SourceFileName = fileName;
					continue;
				}

				if(TargetDirectoryRegex.IsMatch(parameter))
				{
					task.TargetDirectory = parameter.Split('=')[1];
					continue;
				}

				if(TargetArchiveNameRegex.IsMatch(parameter))
				{
					task.TargetName = parameter.Split('=')[1];
					continue;
				}

				if(IgnorePatternRegex.IsMatch(parameter))
				{
					task.IgnorePattern = parameter.Split("=")[1];
					continue;
				}

				if(task is ArchiveDirectoryTask archiveDirectoryTask && IncludeSourceDirectoryRegex.IsMatch(parameter))
				{
					archiveDirectoryTask.IncludeSourceDirectory = bool.Parse(parameter.Split("=")[1]);
					continue;
				}
			}

			return task;
		}
	}
}
