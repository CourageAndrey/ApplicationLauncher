using System;
using System.IO;

namespace Test.Installer
{
	class Program
	{
		static void Main(string[] args)
		{
			const string exeFileName = "Test.Application.exe";
			var currentFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			File.Copy(
				Path.Combine(currentFolder.FullName, exeFileName),
				Path.Combine(currentFolder.Parent.FullName, exeFileName),
				true);
		}
	}
}
