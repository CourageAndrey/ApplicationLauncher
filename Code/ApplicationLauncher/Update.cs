using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace ApplicationLauncher
{
	/// <summary>
	/// Update itself.
	/// </summary>
	[Serializable, DataContract]
	public class Update
	{
		/// <summary>
		/// Zipped update files.
		/// </summary>
		[DataMember]
		public byte[] ZipWithChanges
		{ get; set; }

		/// <summary>
		/// Name of install file if required. Otherwise, all files just will be moved to application directory with overwrite.
		/// </summary>
		[DataMember]
		public string Executable
		{ get; set; }

		/// <summary>
		/// ctor.
		/// </summary>
		public Update()
		{
			ZipWithChanges = new byte[0];
		}

		#region Install

		internal void Install(DirectoryInfo updateDirectory)
		{
			string zipFileName = Path.Combine(updateDirectory.FullName, "_update.zip");
			File.WriteAllBytes(zipFileName, ZipWithChanges);

			ZipFile.ExtractToDirectory(zipFileName, updateDirectory.FullName);
			if (!string.IsNullOrEmpty(Executable))
			{
				run(updateDirectory.FullName, Executable);
			}
			else
			{
				moveFilesUp(updateDirectory, zipFileName);
			}
		}

		private static void run(string directory, string exe)
		{
			string installerPath = Path.Combine(directory, exe);
			var process = Process.Start(installerPath);
			if (process != null)
			{
				process.WaitForExit();
			}
		}

		private static void moveFilesUp(DirectoryInfo fromDirectory, string zipFileName)
		{
			string upperDirectory = fromDirectory.Parent.FullName;
			foreach (var fsInfo in fromDirectory.GetFileSystemInfos())
			{
				if (fsInfo.FullName == zipFileName) continue;

				string newPath = Path.Combine(upperDirectory, fsInfo.Name);

				if (fsInfo is FileInfo)
				{
					if (File.Exists(newPath))
					{
						File.Delete(newPath);
					}
					(fsInfo as FileInfo).MoveTo(newPath);
				}
				else if (fsInfo is DirectoryInfo)
				{
					if (Directory.Exists(newPath))
					{
						Directory.Delete(newPath);
					}
					(fsInfo as DirectoryInfo).MoveTo(newPath);
				}
			}
		}

		#endregion
	}
}
