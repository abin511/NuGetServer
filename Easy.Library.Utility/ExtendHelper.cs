using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Easy.Library.Utility
{
    /// <summary>
    /// description：枚举对象扩展
    /// </summary>
    public static class EnumExtesion
    {
        /// <summary>
        /// 自定义的一个属性类
        /// </summary>
        public class SortAttribute : Attribute
        {
            /// <summary>
            /// 排序号
            /// </summary>
            public int SortNum = 0;
            /// <summary>
            /// 排序构造函数
            /// </summary>
            /// <param name="sortNum"></param>
            public SortAttribute(int sortNum)
            {
                this.SortNum = sortNum;
            }
        }
        /// <summary>
        /// 对执行枚举，按照Sort特性重新排序
        /// </summary>
        /// <param name="enumName">枚举</param>
        /// <param name="asc">默认升序</param>
        /// <returns>描述内容</returns>
        public static Enum[] Sort(this Type enumName, bool asc = true)
        {
            var items = Enum.GetValues(enumName).Cast<Enum>().ToDictionary(item => item, item => item.SortNum());
            return (asc ? items.OrderBy(m => m.Value) : items.OrderByDescending(m => m.Value)).Select(m => m.Key).ToArray();
        }
        /// <summary>
        /// 排除的枚举项
        /// </summary>
        /// <param name="values">枚举</param>
        /// <param name="exclude">排除的枚举项</param>
        /// <returns>描述内容</returns>
        public static Enum[] Exclusive(this Enum[] values, params Enum[] exclude)
        {
            var items = values.Where(item => !exclude.Contains(item)).ToArray();
            return items;
        }
        /// <summary>
        /// 排除的枚举项
        /// </summary>
        /// <param name="enumName">枚举</param>
        /// <param name="exclude">排除的枚举项</param>
        /// <returns>描述内容</returns>
        public static Enum[] Exclusive(this Type enumName, params Enum[] exclude)
        {
            return Enum.GetValues(enumName).Cast<Enum>().Where(item => !exclude.Contains(item)).ToArray();
        }
        /// <summary>
        /// 输入包含的枚举项
        /// </summary>
        /// <param name="values">枚举</param>
        /// <param name="exclude">排除的枚举项</param>
        /// <returns>描述内容</returns>
        public static Enum[] Include(this Enum[] values, params Enum[] exclude)
        {
            var items = values.Where(item => exclude.Contains(item)).ToArray();
            return items;
        }
        /// <summary>
        ///输入包含的枚举项
        /// </summary>
        /// <param name="enumName">枚举</param>
        /// <param name="exclude">排除的枚举项</param>
        /// <returns>描述内容</returns>
        public static Enum[] Include(this Type enumName, params Enum[] exclude)
        {
            return Enum.GetValues(enumName).Cast<Enum>().Where(exclude.Contains).ToArray();
        }
        /// <summary>
        /// 从枚举中获取Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>描述内容</returns>
        public static string Description(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
        /// <summary>
        /// 从枚举中获取Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>描述内容</returns>
        public static int SortNum(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            SortAttribute[] attributes = (SortAttribute[])fi.GetCustomAttributes(typeof(SortAttribute), false);
            return (attributes.Length > 0) ? attributes[0].SortNum : 0;
        }
        /// <summary>
        /// 获取字段Description
        /// </summary>
        /// <param name="fieldInfo">FieldInfo</param>
        /// <returns>DescriptionAttribute[] </returns>
        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);
            }
            return null;
        }

        /// <summary>
        /// 根据Description获取枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举描述</param>
        /// <returns>枚举</returns>
        public static T GetEnumName<T>(string description)
        {
            Type type = typeof (T);
            foreach (FieldInfo field in type.GetFields())
            {
                DescriptionAttribute[] curDesc = field.GetDescriptAttr();
                if (curDesc != null && curDesc.Length > 0)
                {
                    if (curDesc[0].Description == description)
                        return (T) field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T) field.GetValue(null);
                }
            }
            T result = default(T);
            return result;
        }
        /// <summary>
        ///  字符串转换成枚举对象
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="enumName">字符串</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string enumName) where T : struct
        {
            T t;
            if (Enum.TryParse(enumName, out t))
            {
                return t;
            }
            return default(T);
        }
        public static string Name(this Enum enumObj)
        {
             return Enum.GetName(enumObj.GetType(), enumObj);
        }
        public static int Value(this Enum enumObj)
        {
            return Convert.ToInt32(enumObj);
        }
        /// <summary>
        /// 将枚举转换为ArrayList
        /// 说明：
        /// 若不是枚举类型，则返回NULL
        /// 单元测试-->通过
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>ArrayList</returns>
        public static ArrayList ToArrayList(this Type type)
        {
            if (type.IsEnum)
            {
                var array = new ArrayList();
                Array enumValues = Enum.GetValues(type);
                foreach (Enum value in enumValues)
                {
                    array.Add(new KeyValuePair<Enum, string>(value, Description(value)));
                }
                return array;
            }
            return null;
        }
        /// <summary>
        /// 将枚举转换为Dictionary
        /// 说明：
        /// 若不是枚举类型，则返回NULL
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>ArrayList</returns>
        public static Dictionary<int, string> ToDictionary(this Type type)
        {
            if (type.IsEnum)
            {
                var dic = new Dictionary<int, string>();
                Array enumValues = Enum.GetValues(type);
                foreach (Enum item in enumValues)
                {
                    dic.Add(item.Value(), item.Description());
                }
                return dic;
            }
            return null;
        }
    }
    /// <summary>
    /// description：字符扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// base编码
        /// </summary>
        /// <param name="str">待编码的字符</param>
        public static string Base64Code(this string str)
        {
            return str.Base64Code(Encoding.UTF8);
        }
        /// <summary>
        /// base编码
        /// </summary>
        /// <param name="str">待编码的字符</param>
        /// <param name="encoding">编码格式</param>
        public static string Base64Code(this string str, Encoding encoding)
        {
            byte[] byteArray = encoding.GetBytes(str);
            string content = Convert.ToBase64String(byteArray);
            return content;
        }
        /// <summary>  
        /// Base64解密  
        /// </summary>  
        /// <param name="base64Str">待解码的base64字符</param>  
        public static string Base64Decode(this string base64Str)
        {
            return base64Str.Base64Decode(Encoding.UTF8);
        }
        /// <summary>  
        /// Base64解密  
        /// </summary>  
        /// <param name="base64Str">待解码的base64字符</param>
        /// <param name="encoding">编码格式</param> 
        public static string Base64Decode(this string base64Str, Encoding encoding)
        {
            byte[] bytes = Convert.FromBase64String(base64Str);
            return encoding.GetString(bytes);
        }
        /// <summary>
        /// 生成特定位数的唯一字符串
        /// </summary>
        /// <param name="prefix">前缀编码</param>
        /// <returns></returns>
        public static string GenerateUniqueText(this string prefix)
        {
            prefix = string.IsNullOrEmpty(prefix) ? "" : prefix + "-";
            int num = 8;
            System.Threading.Thread.Sleep(1);
            string randomResult = string.Empty;
            string readyStr = "0123456789abcdefghijklmnopqrstuvwxyz";
            char[] rtn = new char[num];
            Guid gid = Guid.NewGuid();
            var ba = gid.ToByteArray();
            for (var i = 0; i < num; i++)
            {
                rtn[i] = readyStr[((ba[i] + ba[num + i]) % 35)];
            }
            var guid = rtn.Aggregate(randomResult, (current, r) => current + r);
            return string.Format("{0}{1}-{2}", prefix, DateTime.Now.ToString("yyMMddHHmmssfff"), guid);
        }
        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="format">时间格式</param>
        /// <returns></returns>
        public static string ToStr(this DateTime? dateTime, string format = null)
        {
            if (!dateTime.HasValue) return "";
            return string.IsNullOrEmpty(format) ? dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : dateTime.Value.ToString(format);
        }
    }
    /// <summary>
    /// description：Json对象扩展
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// json字符串转换成对象
        /// </summary>
        public static T ToObjByJson<T>(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return default(T);
            try
            {
                //注意：如果声明为struck类型，这里会报出异常
                var t = (typeof (T));
                if (t.IsValueType || t.Name.ToLower() == "string")
                {
                    return (T)Convert.ChangeType(jsonString, t);
                }
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                //LogHelper.Error("ToObjByJson异常！", ex);
                return default(T);
            }
        }
        /// <summary>
        /// 对象转换成json字符串
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString<T>(this T obj) where T : new()
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        /// <summary>
        /// 对象转换成json字符串
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj)
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
    }

    /// <summary>
    /// description：XML对象扩展
    /// </summary>
    public static class XmlExtension
    {
        /// <summary>
        /// XML字符串转换成对象
        /// </summary>
        public static T ToObjByXml<T>(this string xmlString) where T : class
        {
            if (string.IsNullOrEmpty(xmlString)) return default(T);
            using (StringReader sr = new StringReader(xmlString))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                return serializer.Deserialize(sr) as T;
            }
        }
        private static void KeyValue2Xml(XmlElement node, KeyValuePair<string, object> Source)
        {
            object kValue = Source.Value;
            if (kValue.GetType() == typeof(Dictionary<string, object>))
            {
                foreach (KeyValuePair<string, object> item in kValue as Dictionary<string, object>)
                {
                    XmlElement element = node.OwnerDocument.CreateElement(item.Key);
                    KeyValue2Xml(element, item);
                    node.AppendChild(element);
                }
            }
            else if (kValue.GetType() == typeof(object[]))
            {
                object[] o = kValue as object[];
                for (int i = 0; i < o.Length; i++)
                {
                    XmlElement xitem = node.OwnerDocument.CreateElement("Item");
                    KeyValuePair<string, object> item = new KeyValuePair<string, object>("Item", o[i]);
                    KeyValue2Xml(xitem, item);
                    node.AppendChild(xitem);
                }
            }
            else
            {
                XmlText text = node.OwnerDocument.CreateTextNode(kValue.ToString());
                node.AppendChild(text);
            }
        }
        /// <summary>
        /// 对象转换成XML字符串
        /// </summary>
        /// <returns></returns>
        public static string ToXmlString<T>(this T obj) where T : new()
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }
        /// <summary>
        /// 对象转换成XML字符串
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this XmlNode obj)
        {
            return JsonConvert.SerializeXmlNode(obj);
        }
    }

    /// <summary>
    /// description：字符对象扩展
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// 去掉字符串中的数字
        /// </summary>
        /// <returns></returns>
        public static string RemoveNumber(this string str)
        {
            return Regex.Replace(str, @"\d", string.Empty);
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <returns></returns>
        public static bool IsNumber(this string str)
        {
            return new Regex(@"^([0-9])[0-9]*(\\.\\w*)?$").IsMatch(str);
        }
        /// <summary>
        /// 是否是手机号码
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile(this string str)
        {
            return new Regex(@"/^[1][0-9]{10}$/").IsMatch(str);
        }
        /// <summary>
        /// 是否邮箱
        /// </summary>
        /// <returns></returns>
        public static bool IsEmail(this string str)
        {
            return new Regex(@"[\w-\.]+@(?:[A-Za-z0-9-]+\.)+[a-z]+").IsMatch(str);
        }
        /// <summary>
        /// 去掉html标记
        /// </summary>
        public static string RemoveHtml(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new Regex("<.*?>", RegexOptions.Compiled).Replace(str, string.Empty);
        }
    }
    /// <summary>
    /// description：字典对象扩展
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// dict.TryAdd(2, "Banana");
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false) dict.Add(key, value);
            return dict;
        }
        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// dict.AddOrReplace(3, "Orange");
        /// </summary>
        public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
            return dict;
        }
        /// <summary>
        /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
        /// dict.GetValue(2); 
        /// dict.GetValue(2, "abc"); 
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }
        /// <summary>
        /// 向字典中批量添加键值对
        /// </summary>
        /// <param name="replaceExisted">如果已存在，是否替换</param>
        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> values, bool replaceExisted)
        {
            foreach (var item in values)
            {
                if (dict.ContainsKey(item.Key) == false || replaceExisted)
                    dict[item.Key] = item.Value;
            }
            return dict;
        }
    }
    /// <summary>
    /// description：汉字转拼音类
    /// </summary>
    public static class PinYinHelper
    {
        #region 变量
        private static int[] pyValue = new int[]
               {
                -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
                -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
                -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
                -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
                -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
                -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
                -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
                -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
                -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
                -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
                -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
                -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
                -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
                -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
                -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
                -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
                -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
                -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
                -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
                -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
                -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
                -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
                -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
                -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
                -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
                -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
                -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
                -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
                -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
                -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
                -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
                -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
                -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
               };

        private static string[] pyName = new string[]
                {
                "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
                "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
                "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
                "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
                "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
                "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
                "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
                "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
                "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
                "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
                "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
                "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
                "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
                "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
                "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
                "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
                "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
                "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
                "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
                "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
                "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
                "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
                "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
                "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
                "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
                "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
                "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
                "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
                "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
                "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
                "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
                "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
                "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
                };
        #endregion
        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="chinese">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string ToPinYin(this string chinese)
        {
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
            byte[] array = new byte[2];
            string pyString = "";
            int chrAsc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] noWChar = chinese.ToCharArray();

            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    array = System.Text.Encoding.Default.GetBytes(noWChar[j].ToString());
                    i1 = (short)(array[0]);
                    i2 = (short)(array[1]);
                    chrAsc = i1 * 256 + i2 - 65536;
                    if (chrAsc > 0 && chrAsc < 160)
                    {
                        pyString += noWChar[j];
                    }
                    else
                    {
                        // 修正部分文字
                        if (chrAsc == -9254)  // 修正“圳”字
                            pyString += "Zhen";
                        else
                        {
                            for (int i = (pyValue.Length - 1); i >= 0; i--)
                            {
                                if (pyValue[i] <= chrAsc)
                                {
                                    pyString += pyName[i];
                                    break;
                                }
                            }
                        }
                    }
                }
                // 非中文字符
                else
                {
                    pyString += noWChar[j].ToString();
                }
            }
            return pyString;
        }

        ///   <summary> 
        ///   得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        ///   </summary> 
        ///   <param   name="chinese">单个汉字</param> 
        ///   <returns>单个大写字母</returns> 
        public static string FirstSpellCode(this string chinese)
        {
            long iCnChar;

            byte[] ZW = Encoding.Default.GetBytes(chinese);

            //如果是字母，则直接返回 
            if (ZW.Length == 1)
            {
                return chinese.ToUpper();
            }
            else
            {
                //   get   the     array   of   byte   from   the   single   char    
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }
            #region table   of   the   constant   list
            //expresstion 
            //table   of   the   constant   list 
            // 'A';           //45217..45252 
            // 'B';           //45253..45760 
            // 'C';           //45761..46317 
            // 'D';           //46318..46825 
            // 'E';           //46826..47009 
            // 'F';           //47010..47296 
            // 'G';           //47297..47613 

            // 'H';           //47614..48118 
            // 'J';           //48119..49061 
            // 'K';           //49062..49323 
            // 'L';           //49324..49895 
            // 'M';           //49896..50370 
            // 'N';           //50371..50613 
            // 'O';           //50614..50621 
            // 'P';           //50622..50905 
            // 'Q';           //50906..51386 

            // 'R';           //51387..51445 
            // 'S';           //51446..52217 
            // 'T';           //52218..52697 
            //没有U,V 
            // 'W';           //52698..52979 
            // 'X';           //52980..53640 
            // 'Y';           //53689..54480 
            // 'Z';           //54481..55289 
            #endregion
            //   iCnChar match     the   constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }

            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= .51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else return ("?");
        }
    }
    /// <summary>
    /// description：辅助对象扩展
    /// </summary>
    public static class UtilityExtend
    {
        /// <summary>
        /// 字符串截取
        /// </summary>
        /// <param name="strSub">原始字符</param>
        /// <param name="length">保留长度</param>
        /// <param name="byASCIIEncoding">true 英文和汉字都是一个字符 false 一个汉字两个字符，英文一个字符 </param>
        /// <returns></returns>
        public static string StrSubString(this string strSub, int length, bool byASCIIEncoding = false)
        {
            if (string.IsNullOrEmpty(strSub)) return string.Empty;
            if (byASCIIEncoding)
            {
                return strSub.Substring(0, length) + (length >= strSub.Length ? "" : "...");
            }

            var ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = string.Empty;
            byte[] s = ascii.GetBytes(strSub);
            bool isFinish = true;
            for (int i = 0; i < s.Length; i++)
            {
                //63 = 汉字
                tempLen += s[i] == 63 ? 2 : 1;
                try
                {
                    tempString += strSub.Substring(i, 1);
                }
                catch
                {
                    isFinish = false;
                    break;
                }
                if (tempLen >= length)
                {
                    isFinish = false;
                    break;
                }
            }
            return tempString + (isFinish ? "" : "...");
        }
        /// <summary>
        /// 把字符转换为int类型，如果转换的字符无效，则返回默认值
        /// </summary>
        /// <param name="str">需要转换的字符</param>
        /// <param name="defaultValue">异常返回的默认数据，如果为null，则返回0</param>
        /// <returns></returns>
        public static int ToInt(this string str, int? defaultValue = null)
        {
            int v;
            if (int.TryParse(str, out v))
            {
                return v;
            }
            return defaultValue ?? 0;
        }
        /// <summary>
        /// 把字符转换为float类型，如果转换的字符无效，则返回默认值
        /// </summary>
        /// <param name="str">需要转换的字符</param>
        /// <param name="defaultValue">异常返回的默认数据，如果为null，则返回0</param>
        /// <returns></returns>
        public static float ToFloat(this string str, float? defaultValue = null)
        {
            float v;
            if (float.TryParse(str, out v))
            {
                return v;
            }
            return defaultValue ?? 0f;
        }
        /// <summary>
        /// 把字符转换为float类型，如果转换的字符无效，则返回默认值
        /// </summary>
        /// <param name="str">需要转换的字符</param>
        /// <param name="defaultValue">异常返回的默认数据，如果为null，则返回0</param>
        /// <returns></returns>
        public static double ToDouble(this string str, double? defaultValue = null)
        {
            double v;
            if (double.TryParse(str, out v))
            {
                return v;
            }
            return defaultValue ?? 0d;
        }
        /// <summary>
        /// 把字符转换为decimal类型，如果转换的字符无效，则返回默认值
        /// </summary>
        /// <param name="str">需要转换的字符</param>
        /// <param name="defaultValue">异常返回的默认数据，如果为null，则返回0</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, decimal? defaultValue = null)
        {
            decimal v;
            if (decimal.TryParse(str, out v))
            {
                return v;
            }
            return defaultValue ?? 0m;
        }
        /// <summary>
        /// 把字符转换为DateTime类型，如果转换的字符无效，则返回默认值
        /// </summary>
        /// <param name="str">需要转换的字符</param>
        /// <param name="defaultValue">异常返回的默认数据</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string str, DateTime? defaultValue = null)
        {
            DateTime v;
            if (DateTime.TryParse(str, out v))
            {
                return v;
            }
            return defaultValue;
        }
        /// <summary>
        /// 截断指定位数(不做四舍五入操作)
        /// </summary>
        /// <param name="value">需要截断的金额</param>
        /// <param name="len">保留的位数</param>
        /// <returns></returns>
        public static decimal CutWithN(this decimal value, int len)
        {
            string strDecimal = value.ToString(CultureInfo.InvariantCulture);
            int index = strDecimal.IndexOf(".", StringComparison.Ordinal);
            if (index == -1 || strDecimal.Length < index + len + 1)
            {
                strDecimal = string.Format("{0:F" + len + "}", value);
            }
            else
            {
                int length = index;
                if (len != 0)
                {
                    length = index + len + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return decimal.Parse(strDecimal);
        }
        /// <summary>
        /// 判断List对象中是否有数据
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this List<T> dataList)
        {
            return dataList == null || dataList.Count <= 0;
        }
        /// <summary>
        /// 判断IEnumerable是否有数据
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> dataList)
        {
            return dataList == null || !dataList.Any();
        }
    }
}