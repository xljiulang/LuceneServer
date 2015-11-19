using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket.Fast;
using System.Configuration;
using NetworkSocket.Core;

namespace LuceneServer.Services
{
    /// <summary>
    /// 系统安全登录服务
    /// </summary>
    internal class SystemService : FastApiService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [Api]
        public bool Login(string account, string password)
        {
            var state = account == ConfigurationManager.AppSettings["Account"] && password == ConfigurationManager.AppSettings["Password"];
            this.CurrentContext.Session.TagData.Set("Logined", state);
            return state;
        }
    }
}
