using System;

using ApplicationLauncher;

using Test.Common;

namespace Test.UpdateService
{
	class Program
	{
		private static void Main(string[] args)
		{
			var updateService = new UpdateService(args);
			updateService.RunDefaultUpdateServer(new Uri(StringConstants.UpdateServerUri));
			Console.ReadKey();
		}
	}
}
