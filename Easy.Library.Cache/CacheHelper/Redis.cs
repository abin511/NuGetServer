using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ServiceStack.Redis;

//[assembly: Easy.Library.Cache...Config.XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]
namespace Easy.Library.Cache
{
    internal class Redis : ICacheHelper
    {
        private const string Prefix = "cache.redis.";
        private static readonly TimeSpan ExpiresIn = TimeSpan.FromHours(1);

        private static PooledRedisClientManager _instance = null;
        private static readonly object SyncRoot = new object();
        public PooledRedisClientManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            var config = CacheHelper.GetXmlNode("root/Redis");
                            if (config != null)
                            {
                                XmlNode writeServerListXmlNode = config.SelectSingleNode("WriteServerList");
                                string[] writeServerList = writeServerListXmlNode == null ? new[] { "127.0.0.1:6379" } : writeServerListXmlNode.InnerText.Split(new char[] { ',', '/' });

                                XmlNode readServerListXmlNode = config.SelectSingleNode("ReadServerList");
                                string[] readServerList = readServerListXmlNode == null ? new[] { "127.0.0.1:6379" } : readServerListXmlNode.InnerText.Split(new char[] { ',', '/' });

                                XmlNode maxWritePoolSizeXmlNode = config.SelectSingleNode("MaxWritePoolSize");
                                int maxWritePoolSize = maxWritePoolSizeXmlNode == null ? 60 : int.Parse(maxWritePoolSizeXmlNode.InnerText);

                                XmlNode maxReadPoolSizeXmlNode = config.SelectSingleNode("MaxReadPoolSize");
                                int maxReadPoolSize = maxReadPoolSizeXmlNode == null ? 60 : int.Parse(maxReadPoolSizeXmlNode.InnerText);

                                _instance = new PooledRedisClientManager(readServerList, writeServerList, new RedisClientManagerConfig { MaxWritePoolSize = maxWritePoolSize, MaxReadPoolSize = maxReadPoolSize, AutoStart = true });
                            }
                        }
                    }
                }
                return _instance;
            }
        }
        public void Insert<T>(string key, T entry)
        {
            using (var client = this.Instance.GetClient())
            {
                client.Set(Prefix + key, entry, ExpiresIn);
            }
        }
        public void Insert<T>(string key, T entry, TimeSpan utcExpiry)
        {
            using (var client = this.Instance.GetClient())
            {
                client.Set(Prefix + key, entry, utcExpiry);
            }
        }
        public bool Add<T>(string key, T entry)
        {
            using (var client = this.Instance.GetClient())
            {
               return client.Add(Prefix + key, entry, ExpiresIn);
            }
        }
        public bool Add<T>(string key, T entry, TimeSpan utcExpiry)
        {
            using (var client = this.Instance.GetClient())
            {
                return client.Add(Prefix + key, entry, utcExpiry);
            }
        }
        public T Get<T>(string key)
        {
            using (var client = this.Instance.GetClient())
            {
                return client.Get<T>(Prefix + key);
            }
        }
        public T Get<T>(string key, Func<T> func)
        {
            return this.Get<T>(key, func, ExpiresIn);
        }
        public T Get<T>(string key, Func<T> func, TimeSpan utcExpiry)
        {
            var value = this.Get<T>(key);
            if (value == null)
            {
                value = func();
                if (value != null)
                {
                    this.Insert(key, value, utcExpiry);
                }
            }
            return value == null ? default(T) : value;
        }

        public bool Remove(string key)
        {
            using (var client = this.Instance.GetClient())
            {
               return client.Remove(Prefix + key);
            }
        }

        public bool Remove(List<string> keys)
        {
            var ks = keys.Select(m => Prefix + m);
            using (var client = this.Instance.GetClient())
            {
                client.RemoveAll(ks);
                return true;
            }
        }
        public void Clean()
        {
            using (var client = this.Instance.GetClient())
            {
                client.FlushAll();
            }
        }
    }
}
