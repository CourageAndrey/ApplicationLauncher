using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ApplicationLauncher
{
	public partial class UpdateWindow
	{
		public UpdateWindow()
		{
			InitializeComponent();
		}

		#region Properties

		public IUpdateService UpdateService
		{ get; set; }

		public UpdateDescription Update
		{ get; set; }

		public LauncherConfig Config
		{ get; set; }

		#endregion

		private void windowLoaded(object sender, RoutedEventArgs e)
		{
			Update.InstalledVersion = Config.ApplicationVersion;
			updateGrid.DataContext = Update;

			switch (Config.CheckUpdateStrategy)
			{
				case CheckUpdateStrategy.AskBeforeInstall:
					if (Update.Type == UpdateType.Required)
					{
						buttonInstallLater.IsEnabled = false;
						buttonSkipUpdate.Content = "Выйти из программы";
					}
					break;
				case CheckUpdateStrategy.Automatically:
					performUpdate();
					break;
			}
		}

		#region Command buttons

		private void installNowClick(object sender, RoutedEventArgs e)
		{
			performUpdate();
		}

		private void installLaterClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void skipUpdateClick(object sender, RoutedEventArgs e)
		{
			if (Update.Type == UpdateType.Required)
			{
				DialogResult = false;
			}
			else
			{
				Config.SkipVersion = Update.Version;
				Config.Save();
				DialogResult = true;
			}
		}

		#endregion

		#region Installing update

		private async void performUpdate()
		{
			commandPanel.IsEnabled = false;
			var installationStatus = new UpdateInstallationStatus();
			panelProgress.DataContext = installationStatus;
			var updateDirectory = initializeUpdateDirectory(Config.ApplicationStartupPath);

			installationStatus.Change("Скачивание обновления...", 10);
			Update update = null;
			var installationResult = await Task<UpdateStageResult>.Run(() =>
			{
				try
				{
					update = UpdateService.DowloadUpdate(
						Config.ApplicationName,
						Config.ApplicationVersion,
						Update.Version);
					return new UpdateStageResult();
				}
				catch (Exception error)
				{
					return new UpdateStageResult(error);
				}
			});
			processStageResult(installationResult, "скачивания обновления", updateDirectory, Update.Type != UpdateType.Required);

			installationStatus.Change("Установка обновления...", 60);
			installationResult = await Task<UpdateStageResult>.Run(() =>
			{
				try
				{
					update.Install(updateDirectory);
					return new UpdateStageResult();
				}
				catch (Exception error)
				{
					return new UpdateStageResult(error);
				}
			});
			processStageResult(installationResult, "установки обновления", updateDirectory, false);

			installationStatus.Change("Обновление настроек программы...", 90);
			finalizeUpdateInstallation(Config, Update, updateDirectory);

			installationStatus.Change("Завершено", 100);
			commandPanel.IsEnabled = true;
			DialogResult = true;
		}

		private static DirectoryInfo initializeUpdateDirectory(string applicationStartupPath)
		{
			var applicationFolder = Path.GetDirectoryName(applicationStartupPath);
			var updateDirectory = new DirectoryInfo(Path.Combine(applicationFolder, "_Update"));

			if (updateDirectory.Exists)
			{
				updateDirectory.Delete(true);
			}
			updateDirectory.Create();

			return updateDirectory;
		}

		private void processStageResult(UpdateStageResult result, string stageName, DirectoryInfo updateDirectory, bool canRunApp)
		{
			if (!result.Success)
			{
				MessageBox.Show(
					result.Error,
					"Во время " + stageName + " произошла ошибка.",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
				updateDirectory.Delete(true);
				DialogResult = canRunApp;
			}
		}

		private static void finalizeUpdateInstallation(LauncherConfig config, UpdateDescription update, DirectoryInfo updateDirectory)
		{
			config.ApplicationVersion = update.Version;
			config.SkipVersion = null;
			config.Save();
			updateDirectory.Delete(true);
		}

		private class UpdateStageResult
		{
			public bool Success { get; }
			public string Error { get; }

			public UpdateStageResult()
			{
				Success = true;
				Error = null;
			}

			public UpdateStageResult(Exception error)
			{
				Success = false;
				Error = error.ToString();
			}
		}

		private class UpdateInstallationStatus : INotifyPropertyChanged
		{
			private string _status;
			private int _progress;

			public string Status
			{ get { return _status; } }

			public int Progress
			{ get { return _progress; } }

			public UpdateInstallationStatus()
			{
				_status = "Инициализация...";
				_progress = 0;
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public void Change(string status, int progress)
			{
				_status = status;
				_progress = progress;

				var handler = Volatile.Read(ref PropertyChanged);
				handler?.Invoke(this, new PropertyChangedEventArgs(null));
			}
		}

		#endregion
	}
}
