using System;
using System.Collections.Generic;
using System.Linq;
using IoTGatewayCore.Models;
using IoTGatewayCore.U;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IoTGatewayCore.DB
{
	public class DBProvider<T>
	{
		private readonly ILogger<DBProvider<T>> _l;

		public string DatabaseName { get; set; }
		public readonly string CollectionName;

		public MongoDatabaseBase DB { get; set; }
		public MongoCollectionBase<BsonDocument> Collection { get; set; }
		public MongoClientBase Client { get; set; }

		public DBProvider(ILogger<DBProvider<T>> l, AppCS appCP)
		{
			_l = l;
			CollectionName = typeof(T).Name.Replace("Model", string.Empty);
			Config(appCP.database);
		}

		private void Config(Database db)
		{
			Client = db.Client;
			DatabaseName = db.DatabaseName;
			DB = SafeGetDatabase(DatabaseName);
			Collection = SafeGetCollection(DatabaseName, CollectionName);
		}

		public MongoDatabaseBase SafeGetDatabase(MongoClientBase client, string dbName)
		{
			if (client == null)
			{
				_l.LogWarning("DBHelper.SafeGetDatabase() client is null!");
				return default;
			}

			if (string.IsNullOrEmpty(dbName))
			{
				_l.LogWarning("DBHelper.SafeGetDatabase() dbName is null or empty!");
				return default;
			}

			MongoDatabaseBase db = default;
			try
			{
				db = client.GetDatabase(dbName) as MongoDatabaseBase;
			}
			catch (Exception e)
			{
				_l.LogError(e, "");
			}

			return db;
		}
		public MongoDatabaseBase SafeGetDatabase(string dbName) => SafeGetDatabase(Client, dbName);

		/// <summary>
		/// Safe version of IMongoDatabase.GetCollection&lt;BsonDocument&gt;() (Creates collection before getting it)
		/// </summary>
		private MongoCollectionBase<BsonDocument> SafeGetCollection(string dbName, string collectionName)
		{
			if (string.IsNullOrEmpty(collectionName))
				_l.LogWarning("DBHelper.SafeGetCollection() collectionName is null or empty!");

			var db = SafeGetDatabase(Client, dbName);

			try
			{
				db?.CreateCollection(collectionName);

			}
			catch (Exception e)
			{
				if (e.GetType() != typeof(MongoCommandException))
					_l.LogError(e, "");
			}

			MongoCollectionBase<BsonDocument> collection = default;
			try
			{
				collection = db?.GetCollection<BsonDocument>(collectionName) as MongoCollectionBase<BsonDocument>;
			}
			catch (Exception e)
			{
				_l.LogError(e, "");
			}

			return collection;
		}

		public void SafeAddNew<T>(T obj, ILogger l)
		{
			if (obj == null)
			{
				l.LogWarning($"SafeAddNew<{nameof(T)}> with null data!");
				l.LogInformation($"SafeAddNew<{nameof(T)}> is skipping add...");
				return;
			}

			try
			{
				var doc = obj.ToBsonDocument();
				Collection.InsertOne(doc);
				l.LogDebug($"{nameof(T)} Added");
			}
			catch (Exception e)
			{
				l.LogError(e, $"SafeAddNew<{nameof(T)}> Exception");
			}
		}

		/// <summary>
		/// Safe version of IMongoDatabase.GetCollection&lt;BsonDocument&gt;() (Creates collection before getting it)
		/// </summary>
		public List<Y> SafeGetCollection<Y>(string dbName, string collectionName)
			=> SafeGetCollection(dbName, collectionName).FindSync<Y>(FilterDefinition<BsonDocument>.Empty).ToList();

		/// <summary>
		/// Safe version of IMongoDatabase.GetCollection&lt;BsonDocument&gt;() (Creates collection before getting it)
		/// </summary>
		public List<Y> SafeGetCollection<Y>(string collectionName)
			=> SafeGetCollection<Y>(DatabaseName, collectionName);

		/// <summary>
		/// Safe version of IMongoDatabase.GetCollection&lt;BsonDocument&gt;() (Creates collection before getting it)
		/// </summary>
		public List<Y> SafeGetCollection<Y>()
			=> SafeGetCollection<Y>(DatabaseName, CollectionName);

		/// <summary>
		/// Search and get a document from mongo collection that matches with provided key-value pair
		/// </summary>
		/// <param name="collection">MongoCollection to search in</param>
		/// <param name="key">Key to find it</param>
		/// <param name="value">Value to check equality with it</param>
		/// <returns>BsonDocument wich contains provided key-value. Returns <i>null</i> if the matching document is not found</returns>
		public Y GetMatchingDocument<Y>(MongoCollectionBase<BsonDocument> collection, string key, object value = null)
		{
			if (string.IsNullOrEmpty(key))
				_l.LogWarning("DBHelper.GetMatchingDocument() with empty key!");

			var filter = key is null
				? FilterDefinition<BsonDocument>.Empty
				: Builders<BsonDocument>.Filter.Eq(key, value);

			return collection.FindSync<Y>(filter).FirstOrDefault();
		}

		public Y FindAndUpdate<Y>(Y obj, string key, object value = null)
		{
			var filter = key is null
				? FilterDefinition<BsonDocument>.Empty
				: Builders<BsonDocument>.Filter.Eq(key, value);

			return Collection.FindOneAndReplace<Y>(filter, obj.ToBsonDocument());
		}

		public Y FindAndDelete<Y>(string key, object value = null)
		{
			var filter = key is null
				? FilterDefinition<BsonDocument>.Empty
				: Builders<BsonDocument>.Filter.Eq(key, value);

			return Collection.FindOneAndDelete<Y>(filter);
		}

		/// <summary>
		/// Get value of provided key in a BsonDocument
		/// </summary>
		/// <param name="document"></param>
		/// <param name="key"></param>
		/// <returns>BsonValue of provided key.Returns <i>default</i> if key not found.</returns>
		public BsonValue SafeGetValue(BsonDocument document, string key)
		{
			if (document is null)
				_l.LogWarning("DBHelper.GetValue() from null document?!");

			if (string.IsNullOrEmpty(key))
			{
				_l.LogWarning("DBHelper.GetValue() with empty key!");
				_l.LogInformation("DBHelper.GetValue() is returning default value");
				return default;
			}

			BsonValue result = default;
			try
			{
				document?.TryGetValue(key, out result);
			}
			catch (Exception e)
			{
				_l.LogError(e, "");
			}

			return result;
		}

		/// <summary>
		/// Inserts many BsonDocuments in a MongoCollection
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="docs"></param>
		public void BulkInsertDocument(MongoCollectionBase<BsonDocument> collection, params BsonDocument[] docs)
		{
			if (collection is null)
				_l.LogWarning("DBHelper.BulkInsertDocument() collection is null!");

			if (docs is null || !docs.Any())
			{
				_l.LogWarning("DBHelper.BulkInsertDocument() docs is null or empty!");
				_l.LogInformation("DBHelper.BulkInsertDocument() is skipping insert...");
				return;
			}

			try
			{
				collection?.InsertMany(docs);
			}
			catch (Exception e)
			{
				_l.LogError(e, "");
			}
		}

		/// <summary>
		/// Deletes a collection from db
		/// </summary>
		/// <param name="collectionName"></param>
		public void DeleteCollection(string collectionName)
		{
			if (string.IsNullOrEmpty(collectionName))
			{
				_l.LogWarning("DBHelper.DeleteCollection() with null or empty name!");
				_l.LogInformation("DBHelper.DeleteCollection() is skipping delete...");
				return;
			}

			try
			{
				DB.DropCollection(collectionName);
			}
			catch (Exception e)
			{
				_l.LogError(e, "");
			}
		}
	}
}
