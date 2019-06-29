using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MONGO_DB_SPACE
{
    public enum eRESULT
    {
        ERROR,
        ERROR_CONNECT_TO_DB,
        ALREADY_EXISTS,
        OBJ_ABSENT,
        SUCCESS,
        NUM_RESULT
    };

    public struct MONGO_DB_SETTINGS
    {
        public string dbName;
        public string collectionName;
                
    }

    public class MONGO_DB_CLIENT
    {
        MongoClient _client = null;
        MONGO_DB_SETTINGS _settings;

        public bool Connect(string connectionString)
        {
            bool result = false;
            try
            {
                if (_client == null){
                    _client = new MongoClient(connectionString);
                    result = IsConnected();
                }
            }
            catch
            {                
                result = false;
                throw;
            }
            return result;
        }

        public bool Connect(string connectionString, MONGO_DB_SETTINGS settings)
        {
            SetSettings(settings);
            return Connect(connectionString);
        }

        public bool IsConnected()
        {
            return isConnected();
        }

        public void SetSettings(MONGO_DB_SETTINGS settings)
        {
            _settings = settings;
        }

        
        public eRESULT InsertObj<T>(T obj)
        {
            eRESULT result = eRESULT.ERROR;

            if (_client != null){
                insertObj<T>(obj).GetAwaiter();
            } 
            else 
            {
                result = eRESULT.ERROR_CONNECT_TO_DB;
            }

            return result;
        }

        public eRESULT FindByName<T>(ref T obj, string objName)
        {
            eRESULT result = eRESULT.ERROR;

            if (_client != null)
            {
                var filter = new BsonDocument("$and", new BsonArray
                {              
                    new BsonDocument("Name", objName),
                    new BsonDocument("ObjType", typeof(T).Name)
                });

                obj = findObj<T>(filter).GetAwaiter().GetResult();
                if (obj != null)
                {
                    result = eRESULT.SUCCESS;
                } 
                else if (obj == null) 
                {
                   result = eRESULT.OBJ_ABSENT; 
                }
            }
            else 
            {
                result = eRESULT.ERROR_CONNECT_TO_DB;
            }

            return result;
        }

        private async Task<T> findObj<T>(BsonDocument filter)
        {
            var db = _client.GetDatabase(_settings.dbName);
            var col = db.GetCollection<T>(_settings.collectionName);        

            T obj = default(T);

            var objList = await col.Find(filter).ToListAsync();
            foreach (T item in objList)
            {
                obj = item;
                break;
            }
            return obj;
        }

        private async Task insertObj<T>(T obj)
        {
            var db = _client.GetDatabase(_settings.dbName);
            var col = db.GetCollection<T>(_settings.collectionName);        

            await col.InsertOneAsync(obj);
        }

        private bool isConnected()
        {
            try
            {
                if (_client != null)
                {
                    IMongoDatabase db = _client.GetDatabase("ping");
                    bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
                    return isMongoLive;
                } 
                else 
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
                throw;
            }
        }
    }
}