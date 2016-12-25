using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Unioty.Controls;

namespace Unioty.TcpHandlers
{
    class DeviceListenHandler : TcpDeviceHandler
    {
        // XXX: Consider using a map to prevent client from subscribing to the same control more than once
        List<DeviceControl> subscribedControls = new List<DeviceControl>();

        public DeviceListenHandler(UniotyServer server, TcpClient client)
            : base(server, client)
        {
        }

        public override void HandleConnection()
        {
            var stream = client.GetStream();
            int readLen = 0;
            var buffer = new byte[1024];
            byte ctrlID;

            server.Log("TCP: Listener connected from {0}", client.Client.RemoteEndPoint);

            while (client.Connected)
            {
                // First read 1 byte host control ID
                readLen = stream.Read(buffer, 0, 1);
                if (readLen != 1)
                {
                    throw new IOException("Unexpected read in control ID");
                }
                ctrlID = buffer[0];

                var virtualControl = server.GetVirtualControl(ctrlID);
                subscribedControls.Add(virtualControl);
                virtualControl.DataChanged += VirtualControl_DataChanged;
                server.Log("TCP: Listener is listening control {0}", virtualControl.ControlID);
            }
        }

        public override void Close()
        {
            // Unsub the virtual controls
            foreach (var c in subscribedControls)
            {
                c.DataChanged -= VirtualControl_DataChanged;
            }
        }

        private void VirtualControl_DataChanged(object sender, DataChangedEventArgs args)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    // Write the new data to client
                    var stream = client.GetStream();
                    var metadata = new byte[]
                    {
                    args.ControlID,
                    (byte)args.Payload.PayloadType,
                    (byte)args.Payload.Raw.Length,
                    };
                    // Format is [1 byte control ID, 1 byte payload type, 1 byte payload length, n byte payload]
                    var message = metadata.Concat(args.Payload.Raw).ToArray();
                    stream.Write(message, 0, message.Length);
                }
            }
            catch (Exception exc)
            {
                server.Log("TCP: Error writing changed data to client {0}", exc);
            }
        }
    }
}
