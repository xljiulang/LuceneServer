using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket.Fast;
using NetworkSocket.Core;

namespace LuceneServer.Filters
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    internal class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
        }
    }
}
