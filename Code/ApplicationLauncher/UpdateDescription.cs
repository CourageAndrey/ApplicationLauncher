using System;
using System.Runtime.Serialization;

namespace ApplicationLauncher
{
	[Serializable, DataContract]
	public class UpdateDescription
	{
		[IgnoreDataMember]
		public string InstalledVersion
		{ get; set; }

		[DataMember]
		public string Version
		{ get; set; }

		[DataMember]
		public UpdateType Type
		{ get; set; }

		[DataMember]
		public string Description
		{ get; set; }
	}
}
