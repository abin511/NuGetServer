using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Easy.Library.Utility
{
    /// <summary>
    /// HTTP操作类
    /// </summary>
    public class HttpHelper
    {
        private static readonly Encoding Encoding = Encoding.GetEncoding("utf-8");

        private static HttpClientHandler ClientHandler(string url)
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };
            if (url.Contains("https"))
            {
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            }
            return handler;
        }
        /// <summary>
        /// HttpPost
        /// </summary>
        /// <returns></returns>
        public static T Post<T>(string url, string paramsJson)
        {
            var handler = ClientHandler(url);
            using (var http = new HttpClient(handler))
            {
                HttpContent content = new StringContent(paramsJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = http.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<string> retString = response.Content.ReadAsStringAsync();
                    var result = retString.Result.ToObjByJson<T>();
                    return result;
                }
                return default(T);
            }
        }
        /// <summary>
        /// HttpPost
        /// </summary>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, string paramsJson)
        {
            var handler = ClientHandler(url);
            using (var http = new HttpClient(handler))
            {
                HttpContent content = new StringContent(paramsJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    using (Stream myResponseStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding))
                        {
                            string retString = myStreamReader.ReadToEnd();
                            var result = retString.ToObjByJson<T>();
                            return result;
                        }
                    }
                }
                return default(T);
            }
        }
        /// <summary>
        /// Get请求数据
        /// </summary>
        /// <param name="url">请求的URL地址</param>
        /// <returns></returns>
        public static T Get<T>(string url)
        {
            var handler = ClientHandler(url);
            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage response = http.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<string> tretString = response.Content.ReadAsStringAsync();
                    return tretString.Result.ToObjByJson<T>();
                }
                return default(T);
            }
        }

        /// <summary>
        /// Get请求数据
        /// </summary>
        /// <param name="url">请求的URL地址</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url)
        {
            var handler = ClientHandler(url);
            using (var http = new HttpClient(handler))
            {
                var response = await http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    using (Stream myResponseStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding))
                        {
                            string retString = myStreamReader.ReadToEnd();
                            var result = retString.ToObjByJson<T>();
                            return result;
                        }
                    }
                }
                return default(T);
            }
        }

        /// <summary>
        /// post方法body传值
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="paramsJson">对象序列化</param>
        /// <param name="headers">头部参数</param>
        /// <returns></returns>
        public static T HttpPost<T>(string url, string paramsJson, Dictionary<string, string> headers = null)
        {
            HttpWebRequest webRequest = GetWebRequest(url, "Post", headers);
            byte[] data = Encoding.UTF8.GetBytes(paramsJson);
            webRequest.ContentLength = data.Length;

            using (Stream reqStream = webRequest.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            try
            {
                using (var rsp = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (var responseStream = rsp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            var result = sr.ReadToEnd();
                            return string.IsNullOrEmpty(result) ? default(T) : result.ToObjByJson<T>();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw new HttpException((int)((HttpWebResponse)ex.Response).StatusCode, ex.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static T HttpGet<T>(string url,Dictionary<string,string> headers = null)
        {
            HttpWebRequest webRequest = GetWebRequest(url, "GET", headers);
            try
            {
                using (var rsp = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (var responseStream = rsp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            var result = sr.ReadToEnd();
                            return string.IsNullOrEmpty(result) ? default(T) : result.ToObjByJson<T>();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw new HttpException((int)((HttpWebResponse)ex.Response).StatusCode, null);
            }
        }
        
        public static HttpWebRequest GetWebRequest(string url, string method, Dictionary<string, string> headers = null)
        {
            HttpWebRequest request = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            request.ContentType = "application/json";
            request.ServicePoint.Expect100Continue = false;
            request.Method = method;
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36";
            return request;
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
