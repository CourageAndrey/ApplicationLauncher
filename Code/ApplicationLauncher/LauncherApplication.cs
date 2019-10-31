using System;
using System.Diagnostics;
using System.ServiceModel;

namespace ApplicationLauncher
{
	internal class LauncherApplication
	{
		[STAThread]
		static void Main()
		{
			var config = LauncherConfig.Load();
			if (config == null) return;

			bool canLaunch = true;
			if (config.CheckUpdateStrategy != CheckUpdateStrategy.Never)
			{
				var updateServer = connectToUpdateServer(config.UpdateServerUrl);
				UpdateDescription update;
				try
				{
					update = updateServer.CheckUpdate(config.ApplicationName, config.ApplicationVersion);
				}
				catch (EndpointNotFoundException)
				{
					update = null;
				}

				if (update != null && update.Version != config.SkipVersion)
				{
					var updateDialog = new UpdateWindow
					{
						UpdateService = updateServer,
						Update = update,
						Config = config,
					};

					canLaunch = updateDialog.ShowDialog() == true;
				}
			}

			if (canLaunch)
			{
				launch(config.ApplicationStartupPath);
			}
		}

		private static IUpdateService connectToUpdateServer(string updateServerUrl)
		{
			try
			{
				var channelFactory = new ChannelFactory<IUpdateService>(
					new BasicHttpBinding(),
					new EndpointAddress(updateServerUrl));
				return channelFactory.CreateChannel();
			}
			catch
			{
				// Потому что здесь может быть множество ошибок, включая недоступную сеть - и ничто не должно при этом нарушить работу приложения.
				return null;
			}
		}

		private static void launch(string startupPath)
		{
			Process.Start(startupPath);
		}
	}
}
