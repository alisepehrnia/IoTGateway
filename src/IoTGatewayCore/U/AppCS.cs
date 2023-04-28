using IoTGatewayCore.Models;
using IoTGatewayCore.Models.Config;
using Microsoft.Extensions.Options;

namespace IoTGatewayCore.U
{
	public class AppCS
	{
		private readonly IOptions<AppConfig> _appConfig;
		public readonly Database database;

		public AppCS(IOptions<AppConfig> appConfig)
		{
			_appConfig = appConfig;
			database = appConfig.Value.Database;
		}
	}
}
