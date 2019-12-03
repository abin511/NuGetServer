
namespace Easy.Library.Cache
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    internal enum CacheTypeEnum
    {
        /// <summary>
        /// LocalCache
        /// </summary>
        LocalCache = 1,
        /// <summary>
        /// Memcached
        /// </summary>
        Memcached = 2,
        /// <summary>
        /// Redis
        /// </summary>
        Redis = 3,
    }
}
