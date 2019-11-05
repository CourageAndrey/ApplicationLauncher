using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace ApplicationLauncher
{
	/// <summary>
	/// Application launcher configuration.
	/// </summary>
	[XmlType, XmlRoot]
	public class LauncherConfig
	{
		#region Properties

		/// <summary>
		/// URL of update server.
		/// </summary>
		[XmlElement]
		public string UpdateServerUrl
		{ get; set; }

		/// <summary>
		/// Application name, checking on server.
		/// </summary>
		[XmlElement]
		public string ApplicationName
		{ get; set; }

		/// <summary>
		/// Path to application EXE-file.
		/// </summary>
		[XmlElement]
		public string ApplicationStartupPath
		{ get; set; }

		/// <summary>
		/// Installed application version.
		/// </summary>
		[XmlElement]
		public string ApplicationVersion
		{ get; set; }

		/// <summary>
		/// Update version, which has to be ignored.
		/// </summary>
		[XmlElement]
		public string SkipVersion
		{ get; set; }

		/// <summary>
		/// Update checking method.
		/// </summary>
		[XmlElement]
		public CheckUpdateStrategy CheckUpdateStrategy
		{ get; set; }

		#endregion

		#region [De]Serialization

		[XmlIgnore]
		private static readonly XmlSerializer _configSerializer;
		[XmlIgnore]
		private static readonly string _configFileName;

		static LauncherConfig()
		{
			_configSerializer = new XmlSerializer(typeof(LauncherConfig));
			string startupPath = AppDomain.CurrentDomain.BaseDirectory;
			_configFileName = Path.Combine(startupPath, "LauncherConfig.xml");
		}

		/// <summary>
		/// Load from XML file.
		/// </summary>
		/// <returns>launcher configuration</returns>
		public static LauncherConfig Load()
		{
			try
			{
				using (var xmlReader = XmlReader.Create(_configFileName))
				{
					return (LauncherConfig) _configSerializer.Deserialize(xmlReader);
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString(), "Ошибка при загрузке настроек запуска приложения.", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}
		}

		/// <summary>
		/// Save to XML file.
		/// </summary>
		public void Save()
		{
			var xmlDocument = new XmlDocument();
			using (var writer = new StringWriter())
			{
				_configSerializer.Serialize(writer, this);
				xmlDocument.LoadXml(writer.ToString());
				xmlDocument.Save(_configFileName);
			}
		}

		#endregion
	}
}
