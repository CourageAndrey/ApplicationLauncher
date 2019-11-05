using System;
using System.Runtime.Serialization;

namespace ApplicationLauncher
{
	/// <summary>
	/// Description of incoming update.
	/// </summary>
	[Serializable, DataContract]
	public class UpdateDescription
	{
		[IgnoreDataMember]
		internal string InstalledVersion
		{ get; set; }

		/// <summary>
		/// Version.
		/// </summary>
		[DataMember]
		public string Version
		{ get; set; }

		/// <summary>
		/// Type.
		/// </summary>
		[DataMember]
		public UpdateType Type
		{ get; set; }

		/// <summary>
		/// Detailed information.
		/// </summary>
		[DataMember]
		public string Description
		{ get; set; }
	}
}
