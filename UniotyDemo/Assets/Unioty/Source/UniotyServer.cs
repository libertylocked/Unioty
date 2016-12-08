using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Unioty
{
    public class UniotyServer : IUniotyServer
    {
        bool started = false;
        WriteLogDelegate WriteLog;
        DataReceivedCallback DataRecv;

        TcpListener tcpListener;
        UdpClient udpListener;
        Thread udpNetworkThread, tcpNetworkThread;

        public int Port
        {
            get;
            private set;
        }

        public UniotyServer(int port, WriteLogDelegate writeLogFunc, DataReceivedCallback dataReceiveFunc)
        {
            Port = port;
            WriteLog = writeLogFunc;
            DataRecv = dataReceiveFunc;
        }

        public void Start()
        {
            if (started) return;
            started = true;
            SetupNetworkThreads();
            tcpNetworkThread.Start();
            udpNetworkThread.Start();
        }

        public void Stop()
        {
            if (!started) return;
            started = false;
            tcpNetworkThread.Abort();
            udpNetworkThread.Abort();
            // XXX: May cause exceptions if threads haven't exited
            tcpListener.Stop();
            udpListener.Close();
        }

        void SetupNetworkThreads()
        {
            // Thread that runs the UDP server
            udpNetworkThread = new Thread(() =>
            {
                udpListener = new UdpClient(Port);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Port);
                if (WriteLog != null) WriteLog.Invoke("Listening UDP {0}", Port);

                while (true)
                {
                    try
                    {
                        byte[] dataRead = udpListener.Receive(ref groupEP);
                        // Parse data
                        var deviceID = dataRead[0];
                        var controlID = dataRead[1];
                        var payloadType = (PayloadType)dataRead[2];
                        var payload = dataRead.Skip(3).ToArray();
                        ProcessData(deviceID, controlID, payloadType, payload);
                    }
                    catch (Exception exc)
                    {
                        if (WriteLog != null) WriteLog.Invoke(exc.ToString());
                    }
                }
            });

            // Thread that runs the TCP server
            tcpNetworkThread = new Thread(() =>
            {
                tcpListener = new TcpListener(IPAddress.Any, Port);
                tcpListener.Start();
                if (WriteLog != null) WriteLog.Invoke("Listening TCP {0}", Port);
                StartAcceptTCP();
            });
        }

        void StartAcceptTCP()
        {
            tcpListener.BeginAcceptTcpClient(HandleAsyncTCPConnection, tcpListener);
        }

        void HandleAsyncTCPConnection(IAsyncResult ar)
        {
            StartAcceptTCP();
            TcpClient client = tcpListener.EndAcceptTcpClient(ar);
            NetworkStream stream = client.GetStream();
            int readLen = 0;
            byte[] buffer = new byte[1];
            byte devID = 0;

            try
            {
                // Read the device ID
                // When devID is 0x00, the client is a poller
                readLen = stream.Read(buffer, 0, 1);
                if (readLen != 1)
                {
                    throw new IOException("Unexpected read in device ID");
                }
                devID = buffer[0];
                if (devID == 0x00) HandleTCPPollerConnection(client);
                else HandleTCPPusherConnection(client, devID);
            }
            catch (Exception exc)
            {
                if (WriteLog != null) WriteLog.Invoke(exc.ToString());
            }
            finally
            {
                if (WriteLog != null) WriteLog.Invoke("TCP: Device {0} disconnected", devID);
                client.Close();
            }
        }

        void HandleTCPPusherConnection(TcpClient client, byte devID)
        {
            NetworkStream stream = client.GetStream();
            int readLen = 0;
            byte[] buffer = new byte[1024];
            
            if (WriteLog != null) WriteLog.Invoke("TCP: Device {0} connected from {1}", devID, client.Client.RemoteEndPoint);

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
                ProcessData(devID, controlID, payloadType, payload);
            }
        }

        void HandleTCPPollerConnection(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            if (WriteLog != null) WriteLog.Invoke("TCP: Poller connected from {0}", client.Client.RemoteEndPoint);


        }

        void ProcessData(byte devID, byte ctrlID, PayloadType payloadType, byte[] payloadRaw)
        {
            Payload payload = new Payload(payloadType, payloadRaw);

            // Invoke the data recv callback function
            if (DataRecv != null)
            {
                DataRecv.Invoke(devID, ctrlID, payload);
            }
        }
    }
}
