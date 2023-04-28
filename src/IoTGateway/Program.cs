using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using MQTTnet.AspNetCore.Extensions;
using Microsoft.Extensions.Logging;

namespace IoTGateway
{
	public class Program
	{
		public static Dictionary<string, string> InternalDictionary { get; set; } = new()
		{
			{ "", "" }
		};

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddConsole();
			})
			.ConfigureAppConfiguration((hostingContext, config) =>
			{
				config.AddJsonFile("config.json", false, true);
				config.AddInMemoryCollection(InternalDictionary);
			})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseKestrel(o =>
				{
					o.ListenAnyIP(1883, l => l.UseMqtt()); //Mqtt Pipeline
					o.ListenAnyIP(5000); //HTTP Pipeline
				});
				webBuilder.UseStartup<Startup>();
			});
	}
}
