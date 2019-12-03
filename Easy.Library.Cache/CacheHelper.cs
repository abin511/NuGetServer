using System;
using System.IO;
using System.Web;
using System.Xml;

namespace Easy.Library.Cache
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class CacheHelper
    {
        private static ICacheHelper _instance = null;
        private static readonly object SyncRoot = new object();
        private const string FileName = "Cache.config";
        public static ICacheHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            var node = GetXmlNode("root/*[@enabled=\"true\"]");
                            if (node != null)
                            {
                                CacheTypeEnum cacheType;
                                if (Enum.TryParse(node.Name, true, out cacheType))
                                {
                                    switch (cacheType)
                                    {
                                        case CacheTypeEnum.Memcached: _instance = new Memcached(); break;
                                        case CacheTypeEnum.Redis: _instance = new Redis(); break;
                                        default: _instance = new LocalCache(); break;
                                    }
                                }
                            }
                        }
                    }
                }
                return _instance;
            }
        }
        internal static XmlNode GetXmlNode(string xPath)
        {
            HttpContext current = HttpContext.Current;
            var fName = ((current != null)? current.Request.MapPath("/" + FileName): Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName));
            if (File.Exists(fName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fName);
                return doc.SelectSingleNode(xPath);
            }
            return null;
        }
    }
}
