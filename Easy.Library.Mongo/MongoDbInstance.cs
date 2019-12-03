using System;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Easy.Library.Mongo
{
    public class MongoDbInstance : IDisposable
    {
        private MongoServer _server;
        //设置GridFS文件中对应的集合前缀名
        private static readonly MongoGridFSSettings gdFsSetting = new MongoGridFSSettings() { Root = "Documents" };

        public MongoDatabase _mongoDatabase;
        public MongoGridFS _gridFs;
        public MongoDbInstance(string connStr, string dataBaseName)
        {
            var client = new MongoClient(connStr);
            _server = client.GetServer();
            _mongoDatabase = _server.GetDatabase(dataBaseName);
            //设置文档对象
            _gridFs = new MongoGridFS(_server, dataBaseName, gdFsSetting);
        }
        public void Dispose()
        {
            if (null != _server && _server.State == MongoServerState.Connected)
            {
                _server.Disconnect();
            }
        }
    }
}
