using System;
using System.Web;
using System.Web.Security;

namespace Easy.Library.Utility
{
    /// <summary>
    /// 当前上下文的操作方法
    /// </summary>
    public class CurrentHelper
    {
        /// <summary>
        /// 获取当前窗体认证下，保存的用户数据
        /// </summary>
        /// <returns></returns>
        public static string GetFormsAuthenticationUserData()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null) return null;
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            return ticket?.UserData;
        }
        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {
            try
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }

                //CDN加速后取到的IP simone 090805
                var customerIp = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(customerIp))
                {
                    return customerIp;
                }

                customerIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(customerIp))
                {
                    return customerIp;
                }

                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    customerIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    customerIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(customerIp, "unknown", StringComparison.OrdinalIgnoreCase) == 0 || string.IsNullOrEmpty(customerIp))
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
                return customerIp;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
