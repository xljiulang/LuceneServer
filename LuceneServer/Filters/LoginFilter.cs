using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket.Fast;
using System.Configuration;

namespace LuceneServer.Filters
{
    /// <summary>
    /// 表示登录状态检测过滤器
    /// </summary>
    internal class LoginFilter : FastFilterAttribute
    {
        /// <summary>
        /// 登录状态检测过滤器
        /// </summary>
        public LoginFilter()
        {
            this.Order = -1;
        }

        /// <summary>
        /// 检查是否已登录
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnExecuting(ActionContext filterContext)
        {
            var logined = filterContext.Session.TagData.TryGet<bool>("Logined");
            if (logined == false && bool.Parse(ConfigurationManager.AppSettings["NeedLogin"]) == true)
            {
                throw new Exception("请登录系统后再操作 ..");
            }
        }
    }
}
