using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace ApplicationLauncher
{
	[XmlType, XmlRoot]
	public class LauncherConfig
	{
		#region Properties

		[XmlElement]
		public string UpdateServerUrl
		{ get; set; }

		[XmlElement]
		public string ApplicationName
		{ get; set; }

		[XmlElement]
		public string ApplicationStartupPath
		{ get; set; }

		[XmlElement]
		public string ApplicationVersion
		{ get; set; }

		[XmlElement]
		public string SkipVersion
		{ get; set; }

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
