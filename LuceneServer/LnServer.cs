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
        protected override void OnConnect(FastSession session)
        {
            base.OnConnect(session);
        }
        protected override void OnDisconnect(FastSession session)
        {
            base.OnDisconnect(session);
        }

        protected override void OnException(Exception exception)
        {
            base.OnException(exception);
        }        
    }
}
