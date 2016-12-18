using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Unioty.Controls;

namespace Unioty.TcpHandlers
{
    class DevicePushHandler : TcpDeviceHandler
    {
        public DevicePushHandler(UniotyServer server, TcpClient client)
            : base(server, client)
        {
        }

        public override void HandleConnection()
        { 
            var stream = client.GetStream();
            int readLen = 0;
            var buffer = new byte[1024];
            byte devID;

            // First read in the device ID
            readLen = stream.Read(buffer, 0, 1);
            if (readLen != 1)
            {
                throw new IOException("Unexpected read in device ID");
            }
            devID = buffer[0];

            server.Log("TCP: Device {0} connected from {1}", devID, client.Client.RemoteEndPoint);

            while (client.Connected)
            {
                // A message over TCP should follow this format
                // [1 byte control ID, 1 byte payload type, 1 byte payload length, n byte payload]
                // Read the first 3 bytes, [1 byte control ID, 1 byte payload type, 1 byte payload length]
                readLen = stream.Read(buffer, 0, 3);
                if (readLen == 0)
                {
                    break;
                }
                var controlID = buffer[0];
                var payloadType = (PayloadType)buffer[1];
                var payloadLen = buffer[2];
                // Then read payloadLen bytes for the payload
                readLen = stream.Read(buffer, 0, payloadLen);
                if (readLen == 0)
                {
                    break;
                }
                var payload = buffer.Take(readLen).ToArray();
                server.ProcessData(devID, controlID, payloadType, payload);
            }

        }

        public override void Close()
        {
            // Nothing to clean up here
        }
    }
}
