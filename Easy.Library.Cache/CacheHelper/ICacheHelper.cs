using System;
using System.Collections.Generic;

namespace Easy.Library.Cache
{
    public interface ICacheHelper
    {
        /// <summary>
        /// 向 Cache 对象插入项。如果存在，则覆盖,默认1小时过期
        /// </summary>
        void Insert<T>(string key, T entry);
        /// <summary>
        /// 向 Cache 对象插入项。如果存在，则覆盖
        /// </summary>
        void Insert<T>(string key, T entry, TimeSpan utcExpiry);
        /// <summary>
        /// 将指定项添加到 Cache 对象，该对象具有依赖项、过期和优先级策略以及一个委托。如果 Cache 中已保存了具有相同 key 参数的项，则添加失败,默认1小时过期
        /// </summary>
        bool Add<T>(string key, T entry);
        /// <summary>
        /// 将指定项添加到 Cache 对象，该对象具有依赖项、过期和优先级策略以及一个委托。如果 Cache 中已保存了具有相同 key 参数的项，则添加失败
        /// </summary>
        bool Add<T>(string key, T entry, TimeSpan utcExpiry);
        /// <summary>
        /// 根据指定的key获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取一个对象，如果对象没有获取到，调用func重新加载缓存
        /// </summary>
        T Get<T>(string key, Func<T> func, TimeSpan utcExpiry);
        /// <summary>
        /// 根据指定的key移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(string key);
        /// <summary>
        /// 批量移除缓存
        /// </summary>
        /// <param name="keys">多个缓存key</param>
        /// <returns></returns>
        bool Remove(List<string> keys);
        /// <summary>
        /// 使所有缓存失效（清除Redis所有数据库的所有Key）
        /// </summary>
        /// <returns></returns>
        void Clean();
    }
}
