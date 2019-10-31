using System.IO;

using Test.Common;

namespace Test.Application
{
	class Program
	{
		static void Main(string[] args)
		{
			File.WriteAllText(StringConstants.TestResultFullPath, StringConstants.ResultBeforeUpdate);
			//File.WriteAllText(StringConstants.TestResultFullPath, StringConstants.ResultAfterUpdate);
		}
	}
}
