using System;
using System.ServiceModel;

namespace ApplicationLauncher
{
	/// <summary>
	/// Update service contract.
	/// </summary>
	[ServiceContract]
	public interface IUpdateService
	{
		/// <summary>
		/// Check, if update is required.
		/// </summary>
		/// <param name="applicationName">updated application name</param>
		/// <param name="fromVersion">installed application version</param>
		/// <returns>incoming update details, or <b>null</b> if update is not required</returns>
		[OperationContract]
		UpdateDescription CheckUpdate(string applicationName, string fromVersion);

		/// <summary>
		/// Download update.
		/// </summary>
		/// <param name="applicationName">updated application name</param>
		/// <param name="fromVersion">installed application version</param>
		/// <param name="toVersion">requested version to update</param>
		/// <returns>update</returns>
		[OperationContract]
		Update DowloadUpdate(string applicationName, string fromVersion, string toVersion);
	}

	/// <summary>
	/// Default Update service host settings.
	/// </summary>
	public static class UpdateServiceHelper
	{
		/// <summary>
		/// Default transfer data buffers size.
		/// </summary>
		public const int DefaultMaxUpdateSize = 1024*1024*1024;

		/// <summary>
		/// Create default service host binding.
		/// </summary>
		/// <param name="maxUpdateSize">requested transfer data buffers size</param>
		/// <returns>binding</returns>
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

		/// <summary>
		/// Run update service host.
		/// </summary>
		/// <param name="updateService">update service instance</param>
		/// <param name="uri">update service host URI</param>
		/// <param name="maxUpdateSize">requested transfer data buffers size</param>
		/// <returns>opened update service host</returns>
		public static ServiceHost RunDefaultUpdateServer(this IUpdateService updateService, Uri uri, int maxUpdateSize = DefaultMaxUpdateSize)
		{
			var serviceHost = new ServiceHost(updateService, uri);
			serviceHost.AddServiceEndpoint(typeof(IUpdateService), CreateServiceHostBinding(maxUpdateSize), string.Empty);
			serviceHost.Open();
			return serviceHost;
		}
	}
}
