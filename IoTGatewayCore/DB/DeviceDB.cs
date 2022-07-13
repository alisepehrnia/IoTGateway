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
	public class DeviceDB
	{
		private readonly ILogger<DeviceDB> _l;
		private readonly DBProvider<Device> _db;
		private MongoCollectionBase<BsonDocument> C => _db.Collection;

		public DeviceDB(ILogger<DeviceDB> l, DBProvider<Device> deviceDBProvider)
		{
			_l = l;
			_db = deviceDBProvider;
		}

		public void AddNewDevice(Device device) => _db.SafeAddNew(device, _l);

		/// <summary>
		/// Search and get a document from mongo collection that matches with provided key-value pair
		/// </summary>
		public Device GetMatchingDevice(string key, object value) => _db.GetMatchingDocument<Device>(C, key, value);
		public Device UpdateDevice(Device newDev, string key, object value) => _db.FindAndUpdate(newDev, key, value);
		public Device DeleteDevice(string key, object value) => _db.FindAndDelete<Device>(key, value);

		public List<Device> GetAll() => _db.SafeGetCollection<Device>();
	}
}
