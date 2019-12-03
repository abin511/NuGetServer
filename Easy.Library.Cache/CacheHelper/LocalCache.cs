using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Easy.Library.Cache
{
    internal class LocalCache : ICacheHelper
    {
        private const string Prefix = "cache.local.";
        static readonly System.Web.Caching.Cache ObjCache = HttpRuntime.Cache;
        private static readonly TimeSpan ExpiresIn = TimeSpan.FromHours(1);

        /// <summary>
        /// 向 Cache 对象插入项。默认24小时有效期
        /// 如果 Cache 中已保存了具有相同 key 参数的项，则覆盖改key中包含的Value值
        /// </summary>
        public void Insert<T>(string key, T entry)
        {
            //弹性过期时间
            ObjCache.Insert(Prefix + key, entry, null, System.Web.Caching.Cache.NoAbsoluteExpiration, ExpiresIn, System.Web.Caching.CacheItemPriority.Default, null);
        }

        public void Insert<T>(string key, T entry, TimeSpan utcExpiry)
        {
            //弹性过期时间
            ObjCache.Insert(Prefix + key, entry, null, System.Web.Caching.Cache.NoAbsoluteExpiration, utcExpiry, System.Web.Caching.CacheItemPriority.Default, null);
        }

        public bool Add<T>(string key, T entry)
        {
            //弹性过期时间
            var result = ObjCache.Add(Prefix + key, entry, null, System.Web.Caching.Cache.NoAbsoluteExpiration, ExpiresIn, System.Web.Caching.CacheItemPriority.Default, null);
            return result == null;
        }
        /// <summary>
        /// 将指定项添加到 Cache 对象，该对象具有依赖项、过期和优先级策略以及一个委托。
        /// 如果 Cache 中已保存了具有相同 key 参数的项，则对此方法的调用将失败
        /// </summary>
        public bool Add<T>(string key, T entry, TimeSpan utcExpiry)
        {
            //弹性过期时间
            var result = ObjCache.Add(Prefix + key, entry, null, System.Web.Caching.Cache.NoAbsoluteExpiration, utcExpiry, System.Web.Caching.CacheItemPriority.Default,null);
            return result == null;
        }
        public T Get<T>(string key)
        {
            var value = ObjCache[Prefix + key];
            if (value == null) return default(T);
            return (T)value;
        }
        /// <summary>
        /// 获取一个对象，如果对象没有获取到，调用func重新加载缓存
        /// </summary>
        public T Get<T>(string key, Func<T> func, TimeSpan utcExpiry)
        {
            var value = this.Get<T>(key);
            if (value != null)
            {
                return value;
            }
            if (func == null)
            {
                return default(T);
            }
            var data = func();
            if (data != null)
            {
                this.Insert(key, data, utcExpiry);
                return data;
            }
            return default(T);
        }
        public bool Remove(string key)
        {
            var result = ObjCache.Remove(Prefix + key);
            return result != null;
        }

        public bool Remove(List<string> keys)
        {
            int iRet = keys.Select(key => ObjCache.Remove(Prefix + key)).Select(result => result != null ? 1 : 0).Sum();
            return iRet > 0;
        }

        public void Clean()
        {
            var cacheEnum = ObjCache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                string key = cacheEnum.Key.ToString();
                ObjCache.Remove(key);
            }
        }
    }
}
