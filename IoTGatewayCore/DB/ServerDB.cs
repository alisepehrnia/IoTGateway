using System.Collections.Generic;
using IoTGatewayCore.Models.Object;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IoTGatewayCore.DB
{
	/// <summary>
	/// Provides methods to manage Device Configs database ("DeviceConfig" Collection)
	/// </summary>
	public class ServerDB
	{
		private readonly ILogger<ServerDB> _l;
		private readonly DBProvider<Server> _db;
		private MongoCollectionBase<BsonDocument> C => _db.Collection;

		public ServerDB(ILogger<ServerDB> l, DBProvider<Server> deviceDBProvider)
		{
			_l = l;
			_db = deviceDBProvider;
		}

		public void AddNewServer(Server server) => _db.SafeAddNew(server, _l);

		/// <summary>
		/// Search and get a document from mongo collection that matches with provided key-value pair
		/// </summary>
		public Server GetMatchingServerConfig(string key, object value) => _db.GetMatchingDocument<Server>(C, key, value);

		public List<Server> GetAll() => _db.SafeGetCollection<Server>();
	}
}
