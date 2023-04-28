using IoTGatewayCore.DB;
using IoTGatewayCore.Models.Object;
using System.Collections.Generic;
using System.Linq;

namespace IoTGatewayCore.U
{
	public class CS
	{
		public readonly List<Device> _devices;
		public readonly List<Server> _servers;
		public readonly List<Hub> _hubs;

		private readonly DeviceDB _DDB;

		public CS(DeviceDB deviceDB, ServerDB serverDB, HubDB hubDB)
		{
			_DDB = deviceDB;

			_devices = deviceDB.GetAll();
			_servers = serverDB.GetAll();
			_hubs = hubDB.GetAll();

			if (!_servers.Any())
				serverDB.AddNewServer(new Server()
				{
					Id = 1,
					Name = "Server1",
					Token = "s1",
					Host = "localhost",
					Port = 8000
				});
		}

		public Server FindServer() => _servers.FirstOrDefault();
		public Server FindServer(string token) => _servers.Find(x => x.Token == token);

		public Device FindDevice() => _devices.FirstOrDefault();
		public Device FindDeviceByName(string deviceName) => _devices.Find(d => d.Name == deviceName);
		public Device FindDevice(string deviceName, int hubId) => _devices.Find(d => d.Name == deviceName && d.Hub_Id == hubId);
		public Device FindDevice(string token) => _devices.Find(d => d.Hub_Id == null && d.Token == token);
		public Device FindDevice(int id) => _devices.Find(d => d.Id == id);

		public Hub FindHub(string token) => _hubs.Find(h => h.Token == token);
		public Hub FindHub(int deviceHubId) => _hubs.Find(h => h.Id == deviceHubId);

		public void AddDevice(Device d) => _DDB.AddNewDevice(d);
		public void UpdateDevice(Device d, string key = null, object obj = null) => _DDB.UpdateDevice(d, key, obj);
		public void DeleteDevice(string key, object obj) => _DDB.DeleteDevice(key, obj);
	}
}
