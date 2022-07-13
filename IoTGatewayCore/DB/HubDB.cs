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
	public class HubDB
	{
		private readonly ILogger<HubDB> _l;
		private readonly DBProvider<Hub> _db;
		private MongoCollectionBase<BsonDocument> C => _db.Collection;

		public HubDB(ILogger<HubDB> l, DBProvider<Hub> deviceDBProvider)
		{
			_l = l;
			_db = deviceDBProvider;
		}

		public void AddNewHub(Hub hub) => _db.SafeAddNew(hub, _l);

		/// <summary>
		/// Search and get a document from mongo collection that matches with provided key-value pair
		/// </summary>
		public Hub GetMatchingHubConfig(string key, object value) => _db.GetMatchingDocument<Hub>(C, key, value);

		public List<Hub> GetAll() => _db.SafeGetCollection<Hub>();
	}
}
