using System;
using System.IO;
using System.ServiceModel;

using ApplicationLauncher;

using Test.Common;

namespace Test.UpdateService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class UpdateService : IUpdateService
	{
		private readonly bool _setExecutable;
		private readonly string _applicationName;
		private readonly string _latestVersion;

		internal UpdateService(string[] parameters)
		{
			_applicationName = parameters.Length > 0 ? parameters[0] : null;
			_latestVersion = parameters.Length > 1 ? parameters[1] : null;
			_setExecutable = parameters.Length > 2 && parameters[2] == StringConstants.UpdateWithExeParam;
		}

		public UpdateDescription CheckUpdate(string applicationName, string fromVersion)
		{
			return	(string.IsNullOrEmpty(_applicationName) || applicationName == _applicationName) &&
					(string.IsNullOrEmpty(_latestVersion) || string.Compare(_latestVersion, fromVersion, StringComparison.InvariantCulture) > 0)
				? new UpdateDescription
				{
					Description = "This update improves performance and fixes bugs",
					Type = UpdateType.Regular,
					Version = _latestVersion,
				}
				: null;
		}

		public Update DowloadUpdate(string applicationName, string fromVersion, string toVersion)
		{
			return _setExecutable
				? new Update
				{
					ZipWithChanges = File.ReadAllBytes(Path.Combine(StringConstants.TestDirectory, "WithInstaller.zip")),
					Executable = "Test.Installer.exe",
				}
				: new Update
				{
					ZipWithChanges = File.ReadAllBytes(Path.Combine(StringConstants.TestDirectory, "WithoutInstaller.zip")),
					Executable = null,
				};
		}
	}
}
