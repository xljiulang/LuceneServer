using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket.Fast;

namespace LuceneServer.Filters
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    internal class ExceptionFilter : FastFilterAttribute
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
        }
    }
}
