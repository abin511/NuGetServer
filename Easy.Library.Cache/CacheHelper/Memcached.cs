using System;
using System.Collections.Generic;

namespace Easy.Library.Cache
{
    internal class Memcached : ICacheHelper
    {
        private const string Prefix = "cache.memcached.";
        public void Insert<T>(string key, T entry)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T entry, TimeSpan utcExpiry)
        {
            throw new NotImplementedException();
        }

        public bool Add<T>(string key, T entry)
        {
            throw new NotImplementedException();
        }

        public bool Add<T>(string key, T entry, TimeSpan utcExpiry)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key, Func<T> func, TimeSpan utcExpiry)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(List<string> keys)
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }
    }
}
