using System;

namespace Easy.Library.Utility
{
    public interface ILogHelper
    {
        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="strMsg">信息内容</param>
        void Info(string strMsg);
        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="formatStr">信息内容</param>
        /// <param name="args">格式化参数</param>
        void InfoFormat(string formatStr, params object[] args);
        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="strMsg">信息内容</param>
        void Warn(string strMsg);
        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="formatStr">信息内容</param>
        /// <param name="args">格式化参数</param>
        void WarnFormat(string formatStr, params object[] args);
        /// <summary>
        /// 一般错误信息 
        /// </summary>>
        /// <param name="strMsg">错误信息</param>
        /// <param name="exception">异常信息</param>
        void Error(string strMsg, Exception exception);
        /// <summary>
        /// 一般错误信息 
        /// </summary>>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        void ErrorFormat(string formatStr, params object[] args);
        /// <summary>
        /// 失败信息
        /// </summary>
        /// <param name="strMsg">失败信息</param>
        /// <param name="exception">异常信息</param>
        void Fatal(string strMsg, Exception exception);
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        void FatalFormat(string formatStr, params object[] args);
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="strMsg">调试信息</param>
        void Debug(string strMsg);
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="formatStr">错误信息</param>
        /// <param name="args">格式化参数</param>
        void DebugFormat(string formatStr, params object[] args);
    }
}
