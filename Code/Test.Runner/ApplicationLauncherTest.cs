using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using ApplicationLauncher;

using NUnit.Framework;

using Test.Common;

namespace Test.Runner
{
	public class ApplicationLauncherTest
	{
		#region Data & initialization

		private const string _testApplicationName = "Test.Application";
		private const string _testApplicationExe = _testApplicationName + ".exe";
		private const string _testServiceName = "Test.UpdateService";
		private const string _testServiceExe = _testServiceName + ".exe";
		private const string _launcherName = "ApplicationLauncher";
		private const string _launcherExe = _launcherName + ".exe";
		private static string _testApplicationFullPath, _testLauncherFullPath, _testApplicationCopy;

		[SetUp]
		public void Setup()
		{
			_testApplicationFullPath = Path.Combine(StringConstants.TestDirectory, _testApplicationExe);
			_testLauncherFullPath = Path.Combine(StringConstants.TestDirectory, _launcherExe);
			_testApplicationCopy = _testApplicationFullPath + ".copy";

			if (File.Exists(StringConstants.TestResultFullPath))
			{
				File.Delete(StringConstants.TestResultFullPath);
			}

			File.Copy(_testApplicationFullPath, _testApplicationCopy, true);
		}

		[TearDown]
		public void Cleanup()
		{
			var processes = Process.GetProcesses();
			var serverProcess = processes.FirstOrDefault(process => process.ProcessName == _testServiceName);
			if (serverProcess != null)
			{
				serverProcess.CloseMainWindow();
				serverProcess.WaitForExit();
			}
			var launcherProcess = processes.FirstOrDefault(process => process.ProcessName == _launcherName);
			if (launcherProcess != null)
			{
				launcherProcess.Close();
			}
			var appProcess = processes.FirstOrDefault(process => process.ProcessName == _testApplicationName);
			if (appProcess != null)
			{
				appProcess.Close();
			}

			if (File.Exists(StringConstants.TestResultFullPath))
			{
				File.Delete(StringConstants.TestResultFullPath);
			}

			File.Copy(_testApplicationCopy, _testApplicationFullPath, true);
			File.Delete(_testApplicationCopy);
		}

		#endregion

		#region Tests

		[Test]
		public void WhenRunApplicationItselfWithoutLauncherThenItRunsUnchainged()
		{
			initializeConfig("1.0", null);
			var launcherProcess = Process.Start(_testApplicationFullPath);
			if (launcherProcess != null)
			{
				launcherProcess.WaitForExit();
			}
			verify(StringConstants.ResultBeforeUpdate, "1.0");
		}

		[Test]
		public void GivenUpdateServerIsUnavailableWhenRunApplicationThenItRunsUnchainged()
		{
			initializeConfig("1.0", null);
			executeLauncher();
			waitTestApplicationFinishes();
			verify(StringConstants.ResultBeforeUpdate, "1.0");
		}

		[Test]
		public void GivenNoUpdateFromServerWhenRunApplicationThenItRunsUnchainged()
		{
			initializeConfig("2.0", null);
			runUpdateServer(StringConstants.ApplicationName, "1.0", null);
			executeLauncher();
			waitTestApplicationFinishes();
			verify(StringConstants.ResultBeforeUpdate, "2.0");
		}

		[Test]
		public void GivenUpdateFromServerWhenRunApplicationThenItUpdatesAndRuns()
		{
			initializeConfig("1.0", null);
			runUpdateServer(StringConstants.ApplicationName, "2.0", null);
			executeLauncher();
			waitTestApplicationFinishes();
			verify(StringConstants.ResultAfterUpdate, "2.0");
		}

		[Test]
		public void GivenSkippedUpdateFromServerWhenRunApplicationThenItRunsUnchainged()
		{
			initializeConfig("1.0", "1.1");
			runUpdateServer(StringConstants.ApplicationName, "1.1", null);
			executeLauncher();
			waitTestApplicationFinishes();
			verify(StringConstants.ResultBeforeUpdate, "1.0");
		}

		[Test]
		public void GivenNewestUpdateFromServerWhenRunApplicationThenItUpdatesAndRuns()
		{
			initializeConfig("1.0", "1.1");
			runUpdateServer(StringConstants.ApplicationName, "1.2", null);
			executeLauncher();
			waitTestApplicationFinishes();
			verify(StringConstants.ResultAfterUpdate, "1.2");
		}

		[Test]
		public void GivenUpdateWithInstallerFromServerWhenRunApplicationThenItUpdatesAndRuns()
		{
			initializeConfig("1.0", null);
			runUpdateServer(StringConstants.ApplicationName, "2.0", true);
			executeLauncher();
			waitTestApplicationFinishes();
			verify(StringConstants.ResultAfterUpdate, "2.0");
		}

		#endregion

		#region Helpers

		private static void initializeConfig(string applicationVersion, string skipVersion)
		{
			var config = new LauncherConfig
			{
				UpdateServerUrl = StringConstants.UpdateServerUri,
				ApplicationName = StringConstants.ApplicationName,
				ApplicationStartupPath = _testApplicationExe,
				ApplicationVersion = applicationVersion,
				CheckUpdateStrategy = CheckUpdateStrategy.Automatically,
				SkipVersion = skipVersion,
			};
			config.Save();
		}

		private static void runUpdateServer(string applicationName, string latestVersion, bool? setExecutable)
		{
			var parameters = new List<string>();
			if (!string.IsNullOrEmpty(applicationName))
			{
				parameters.Add(applicationName);
			}
			if (!string.IsNullOrEmpty(latestVersion))
			{
				parameters.Add(latestVersion);
			}
			if (setExecutable.HasValue)
			{
				parameters.Add(StringConstants.UpdateWithExeParam);
			}

			var serverProcess = Process.Start(
				Path.Combine(StringConstants.TestDirectory, _testServiceExe),
				string.Join(" ", parameters));

			Thread.Sleep(500);
			Assert.IsNotNull(serverProcess != null && !serverProcess.HasExited);
		}

		private static void executeLauncher()
		{
			var launcherProcess = Process.Start(_testLauncherFullPath);
			if (launcherProcess != null)
			{
				launcherProcess.WaitForExit();
			}
		}

		private static void waitTestApplicationFinishes()
		{
			var appProcess = Process.GetProcesses().FirstOrDefault(process => process.ProcessName == _testApplicationName);
			if (appProcess != null)
			{
				appProcess.WaitForExit();
			}
		}

		private static void verify(string expectedResult, string expectedVersion)
		{
			string testResult = File.ReadAllText(StringConstants.TestResultFullPath);
			Assert.AreEqual(expectedResult, testResult);

			var config = LauncherConfig.Load();
			Assert.AreEqual(expectedVersion, config.ApplicationVersion);
		}

		#endregion
	}
}
