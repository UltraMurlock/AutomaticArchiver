using AutomaticArchivation.Ignoring;

namespace AutomaticArchiver
{
	[Serializable]
	public class IgnorePatternList
	{
		public IgnorePattern[] IgnorePatterns { get; set; }



		public IgnorePatternList()
		{
			IgnorePatterns = [];
		}



		public static IgnorePatternList Template
		{
			get
			{
				return new IgnorePatternList()
				{
					IgnorePatterns = [IgnorePattern.VisualStudioCSharp]
				};
			}
		}

		public bool TryGetPatternByName(string name, out IgnorePattern ignorePattern)
		{
			foreach(var pattern in IgnorePatterns)
			{
				if(pattern.Name == name)
				{
					ignorePattern = pattern;
					return true;
				}
			}

			ignorePattern = new IgnorePattern();
			return false;
		}
	}
}
