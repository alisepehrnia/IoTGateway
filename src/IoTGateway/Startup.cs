using IoTGateway.Controllers;
using IoTGatewayCore.Connection;
using IoTGatewayCore.DB;
using IoTGatewayCore.IS;
using IoTGatewayCore.MC;
using IoTGatewayCore.Models.Config;
using IoTGatewayCore.Models.Object;
using IoTGatewayCore.MP;
using IoTGatewayCore.U;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;
using MQTTnet.Server;
using System;

namespace IoTGateway
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.Configure<AppConfig>(Configuration.GetSection(nameof(AppConfig)));

			services
				.AddSingleton<AppCS>()
				.AddSingleton<CS>()
				.AddSingleton<LS<SC>>()
				.AddSingleton<LS<DC>>()
				.AddSingleton<LS<HC>>()
				.AddSingleton<ServerIS>()
				.AddSingleton<DeviceIS>()
				.AddSingleton<HubIS>()
				.AddSingleton<IS>()
				.AddSingleton<Terminal>()
				.AddSingleton<MS>()
				.AddSingleton<MqttProvider>()
				.AddSingleton<HttpProvider>()
				.AddSingleton<ToExternal>()
				.AddSingleton<DBProvider<Device>>()
				.AddSingleton<DBProvider<Hub>>()
				.AddSingleton<DBProvider<Server>>()
				.AddSingleton<DeviceDB>()
				.AddSingleton<HubDB>()
				.AddSingleton<ServerDB>();
				

			services.AddHostedMqttServerWithServices(o => BuildMqttOptions(o))
					.AddMqttConnectionHandler();

			services.AddControllers();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "IoTGateway", Version = "v1" });
			});

			services.AddHttpClient();
		}

		private static void BuildMqttOptions(AspNetMqttServerOptionsBuilder o)
		{
			o.WithDefaultEndpoint()
				.WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(10))
				.WithPersistentSessions()
				.WithClientId("IoTGateway-MqttServer");

			o.ServiceProvider
				.GetRequiredService<MqttProvider>()
				.ConfigureMqttServerOptions(o);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoTGateway v1"));
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseMqttServer(s => ConfigMqttServer(app, s));
		}

		private static void ConfigMqttServer(IApplicationBuilder app, IMqttServer s) 
			=> app.ApplicationServices
				.GetRequiredService<MqttProvider>()
				.ConfigureMqttServer(s);

		private class SC
		{
		}

		private class DC
		{
		}

		private class HC
		{
		}
	}
}
