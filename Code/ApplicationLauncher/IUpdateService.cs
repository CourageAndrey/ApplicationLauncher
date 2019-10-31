using System;
using System.ServiceModel;

namespace ApplicationLauncher
{
	[ServiceContract]
	public interface IUpdateService
	{
		[OperationContract]
		UpdateDescription CheckUpdate(string applicationName, string fromVersion);

		[OperationContract]
		Update DowloadUpdate(string applicationName, string fromVersion, string toVersion);
	}

	public static class UpdateServiceHelper
	{
		public const int DefaultMaxUpdateSize = 1024*1024*1024;

		public static System.ServiceModel.Channels.Binding CreateServiceHostBinding(int maxUpdateSize = DefaultMaxUpdateSize)
		{
			return new BasicHttpBinding
			{
				MaxReceivedMessageSize = maxUpdateSize,
				MaxBufferPoolSize = maxUpdateSize,
				MaxBufferSize = maxUpdateSize,
				TransferMode = TransferMode.Streamed,
				ReaderQuotas =
				{
					MaxDepth = maxUpdateSize,
					MaxStringContentLength = maxUpdateSize,
					MaxArrayLength = maxUpdateSize,
					MaxBytesPerRead = maxUpdateSize,
					MaxNameTableCharCount = maxUpdateSize,
				},
			};
		}

		public static ServiceHost RunDefaultUpdateServer(this IUpdateService updateService, Uri uri, int maxUpdateSize = DefaultMaxUpdateSize)
		{
			var serviceHost = new ServiceHost(updateService, uri);
			serviceHost.AddServiceEndpoint(typeof(IUpdateService), CreateServiceHostBinding(maxUpdateSize), string.Empty);
			serviceHost.Open();
			return serviceHost;
		}
	}
}
