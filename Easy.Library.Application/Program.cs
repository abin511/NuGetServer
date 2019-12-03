
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Easy.Library.Utility;

namespace Easy.Library.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            //var result = HttpHelper.HttpGet<string>("http://api.elvbus.com/api/Login/MemUserLoginByWeiXin?openId=oaxHIvwXc2xN-lcS9Uw3lqFqW1LA");

            // result = HttpHelper.HttpPost<string>("http://api.elvbus.com/api/BacGrabOrder/GetVehiclePricing",new Dictionary<string, string>());
            //CacheHelper.Init(CacheTypeEnum.Redis, null);
            //CacheHelper.Instance.Add("ss", "wshi");
            //var a = CacheHelper.Instance.Get<string>("ss");
            //return;
            //DateTime? now = DateTime.Now;
            //string ss = now.ToStr() + "{0}";
            //Stopwatch ws = new Stopwatch();
            //ws.Start();
            //for (int i = 0; i < 100; i++)
            //{
            //    LogHelper.Instance.Error(ss, "tinghao");
            //}
            //ws.Stop();
            //LogHelper.Instance.Info("总耗时:{0}", ws.ElapsedMilliseconds);
            
            XmlDocument DOC = new XmlDocument();
            DOC.Load("test.xml");
            string sss = DOC.ToXmlString();

            Dictionary<string,int> dic = new Dictionary<string, int>();
            dic.Add("测试1",1);
            dic.Add("测试2", 2);
            dic.Add("cs3", 3);
            string test = dic.ToJsonString();
            LogHelper.Instance.DebugFormat("除非{0},shijian:{1},dd:{2}","我的","你的","他的");
            LogHelper.Instance.Debug(test);
        }
    }
}
