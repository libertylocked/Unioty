using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Unioty.TcpHandlers
{
    abstract class TcpDeviceHandler
    {
        protected UniotyServer server;
        protected TcpClient client;

        public TcpDeviceHandler(UniotyServer server, TcpClient client)
        {
            this.server = server;
            this.client = client;
        }

        public abstract void HandleConnection();

        public abstract void Close();
    }
}
