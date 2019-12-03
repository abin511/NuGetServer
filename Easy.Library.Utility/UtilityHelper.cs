using System;
using System.Configuration;

namespace Easy.Library.Utility
{
    /// <summary>
    /// 常用方法
    /// </summary>
    public class UtilityHelper
    {
        /// <summary>
        /// 获取connectionStrings节点的值
        /// </summary>
        /// <returns></returns>
        public static string GetWebConfigConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
        /// <summary>
        /// 获取connectionStrings节点的驱动对象
        /// </summary>
        /// <returns></returns>
        public static string GetWebConfigProviderName(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return ConfigurationManager.ConnectionStrings[key].ProviderName;
        }
        /// <summary>
        /// 获取appSettings节点的数据
        /// </summary>
        /// <returns></returns>
        public static string GetWebConfigAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return ConfigurationManager.AppSettings[key];
        }
    }
}
