using log4net;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

//指定log4net使用的config文件来读取配置信息
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]
namespace Easy.Library.Utility
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    internal class Log4Net : ILogHelper
    {
        private static readonly ConcurrentDictionary<Type, ILog> Loggers = new ConcurrentDictionary<Type, ILog>();

        /// <summary>
        /// 获取记录器
        /// </summary>
        /// <returns></returns>
        private ILog GetLogger()
        {
            StackTrace trace = new StackTrace();
            var source = trace.GetFrame(2).GetMethod().DeclaringType;
            if (Loggers.ContainsKey(source))
            {
                return Loggers[source];
            }
            else
            {
                ILog logger = LogManager.GetLogger(source);
                Loggers.TryAdd(source, logger);
                return logger;
            }
        }
        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="strMsg">信息内容</param>
        public void Info(string strMsg)
        {
            ILog logger = GetLogger();
            if (logger.IsInfoEnabled)
                logger.Info(strMsg);
        }
        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="formatStr">信息内容</param>
        /// <param name="args">格式化参数</param>
        public void InfoFormat(string formatStr, params object[] args)
        {
            ILog logger = GetLogger();
            if (logger.IsInfoEnabled)
                logger.InfoFormat(formatStr, args);
        }
        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="strMsg">信息内容</param>
        public void Warn(string strMsg)
        {
            ILog logger = GetLogger();
            if (logger.IsWarnEnabled)
                logger.Warn(strMsg);
        }
        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        public void WarnFormat(string formatStr, params object[] args)
        {
            ILog logger = GetLogger();
            if (logger.IsWarnEnabled)
                logger.WarnFormat(formatStr, args);
        }
        /// <summary>
        /// 一般错误信息 
        /// </summary>>
        /// <param name="strMsg">错误信息</param>
        /// <param name="exception">异常信息</param>
        public void Error(string strMsg, Exception exception)
        {
            ILog logger = GetLogger();
            if (logger.IsErrorEnabled)
                logger.Error(strMsg, exception);
        }
        /// <summary>
        /// 一般错误信息 
        /// </summary>>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        public void ErrorFormat(string formatStr, params object[] args)
        {
            ILog logger = GetLogger();
            if (logger.IsErrorEnabled)
                logger.ErrorFormat(formatStr, args);
        }
        /// <summary>
        /// 失败信息
        /// </summary>
        /// <param name="strMsg">失败信息</param>
        /// <param name="exception"></param>
        public void Fatal(string strMsg, Exception exception)
        {
            ILog logger = GetLogger();
            if (logger.IsFatalEnabled)
                logger.Fatal(strMsg, exception);
        }
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        public void FatalFormat(string formatStr, params object[] args)
        {
            ILog logger = GetLogger();
            if (logger.IsFatalEnabled)
                logger.FatalFormat(formatStr, args);
        }
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="strMsg">调试信息</param>
        public void Debug(string strMsg)
        {
            ILog logger = GetLogger();
            if (logger.IsDebugEnabled)
                logger.Debug(strMsg);
        }
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        public void DebugFormat(string formatStr, params object[] args)
        {
            ILog logger = GetLogger();
            if (logger.IsDebugEnabled)
                logger.DebugFormat(formatStr, args);
        }
    }
}