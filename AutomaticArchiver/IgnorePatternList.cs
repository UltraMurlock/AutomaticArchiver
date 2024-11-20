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
				IgnorePattern ignorePattern = new IgnorePattern()
				{
					Name = "Template",
					StringPatterns = new string[]
					{
						".*.ignore",
						"testignore.txt",
						"ignoredfolder/"
					}
				};
				IgnorePatternList template = new IgnorePatternList();
				template.IgnorePatterns = [ignorePattern];
				return template;
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
