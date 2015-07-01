using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket;
using NetworkSocket.Fast;
namespace LuceneServer
{
    /// <summary>
    /// Lucene.Net服务器
    /// </summary>
    internal class LnServer : FastTcpServer
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
    }
}
