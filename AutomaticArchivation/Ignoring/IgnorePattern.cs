using System.Text.RegularExpressions;

namespace AutomaticArchivation.Ignoring
{
	[Serializable]
	public class IgnorePattern
	{
		public string Name { get; set; }

		public string[] StringPatterns { get; set; }

		private Regex[] _fileRegexes;
		private Regex[] _directoryRegexes;
		private bool _isInitialized;



		public IgnorePattern()
		{
			Name = string.Empty;
			StringPatterns = [];

			_fileRegexes = [];
			_directoryRegexes = [];
		}

		public static IgnorePattern Empty
		{
			get => new IgnorePattern() { Name = "Empty" };
		}

		public static IgnorePattern VisualStudioCSharp
		{
			get => new IgnorePattern()
			{
				Name = "VisualStudioC#",
				StringPatterns = [
					"^[Bb]in$/",
					"^[Oo]bj$/",
					"^.vs$/"]
			};
		}



		public bool IsMatch(FileInfo file)
		{
			if(!_isInitialized)
				Initialize();

			foreach(var regex in _fileRegexes)
			{
				if(regex.IsMatch(file.Name))
					return true;
			}

			return false;
		}

		public bool IsMatch(DirectoryInfo directory)
		{
			if(!_isInitialized)
				Initialize();

			foreach(var regex in _directoryRegexes)
			{
				if(regex.IsMatch(directory.Name))
					return true;
			}

			return false;
		}



		private void Initialize()
		{
			List<Regex> fileRegexes = new List<Regex>();
			List<Regex> directoryRegexes = new List<Regex>();

			foreach(var pattern in StringPatterns)
			{
				if(pattern.EndsWith('/'))
				{
					var temp = pattern.TrimEnd('/');
					directoryRegexes.Add(CreateRegex(temp));
				}
				else
				{
					var temp = pattern.Replace("*", ".*");
					fileRegexes.Add(CreateRegex(temp));
				}
			}

			_fileRegexes = fileRegexes.ToArray();
			_directoryRegexes = directoryRegexes.ToArray();
			_isInitialized = true;
		}

		private Regex CreateRegex(string pattern)
		{
			return new Regex(pattern, RegexOptions.Compiled);
		}
	}
}
