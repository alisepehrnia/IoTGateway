using MongoDB.Driver;

namespace IoTGatewayCore.Models
{
	public class Database
	{
		public string Host { get; set; }
		public int Port { get; set; }
		public string DatabaseName { get; set; }

		private readonly string ConnectionString;

		public MongoClient Client { get; }

		public Database()
		{
			Host ??= "localhost";
			Port = Port <= 0 ? 27017 : Port;
			ConnectionString = $"mongodb://{Host}:{Port}";
			Client = new(ConnectionString);
		}
	}
}