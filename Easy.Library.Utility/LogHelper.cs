namespace Easy.Library.Utility
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper
    {
        private static ILogHelper _instance = null;
        private static readonly object SyncRoot = new object();
        public static ILogHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            //readonly static string CacheTypeCfg = UtilityHelper.GetWebConfigAppSettings("CacheType") ?? "LocalCache";
                            LogTypeEnum type = LogTypeEnum.Log4Net;
                            switch (type)
                            {
                                default: _instance = new Log4Net(); break;
                            }
                        }
                    }
                }
                return _instance;
            }
        }
    }
}