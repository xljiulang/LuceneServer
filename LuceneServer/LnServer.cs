using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket;
using NetworkSocket.Fast;
using Topshelf;
using LuceneServer.Services;
using LuceneServer.Filters;
using System.Configuration;
using NetworkSocket.Exceptions;
namespace LuceneServer
{
    /// <summary>
    /// Lucene.Net服务器
    /// </summary>
    internal class LnServer : FastTcpServer, ServiceControl
    {
        /// <summary>
        /// 关闭非法连接的用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="exception"></param>
        protected override void OnException(object sender, Exception exception)
        {
            if (exception is ProtocolException)
            {
                var session = sender as FastSession;
                session.Close();
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(Topshelf.HostControl hostControl)
        {
            this.GlobalFilters.Add(new ExceptionFilter());
            this.BindService<LnService>().BindService<SystemService>();
            this.StartListen(int.Parse(ConfigurationManager.AppSettings["Port"]));
            return true;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(Topshelf.HostControl hostControl)
        {
            this.Dispose();
            return true;
        }
    }
}
