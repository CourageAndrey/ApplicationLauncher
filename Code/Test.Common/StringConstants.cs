using System;
using System.IO;

namespace Test.Common
{
	public static class StringConstants
	{
		public const string ApplicationName = "MegaTest";

		public static readonly string TestDirectory = AppDomain.CurrentDomain.BaseDirectory;

		public const string TestResultFileName = "CheckThisFile.txt";

		public static readonly string TestResultFullPath = Path.Combine(TestDirectory, TestResultFileName);

		public const string ResultBeforeUpdate = "Application is not updated.";

		public const string ResultAfterUpdate = "Application has been successfully updated.";

		public const string UpdateServerUri = "http://localhost:1987/testapplicationupdater";

		public const string UpdateWithExeParam = "exe";
	}
}
