using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unioty.Controls;
using Unioty.TcpHandlers;

namespace Unioty
{
    public class UniotyServer : IUniotyServer
    {
        bool started = false;
        WriteLogDelegate WriteLog;
        DataReceivedCallback DataRecv;
        DeviceControl[] virtualControls = new DeviceControl[256];

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
            // Init virtual controls
            for (int i = 0; i < 256; i++)
            {
                virtualControls[i] = new DeviceControl(0x00, (byte)i);
            }
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

        public void ProcessData(byte devID, byte ctrlID, PayloadType payloadType, byte[] payloadRaw)
        {
            Payload payload = new Payload(payloadType, payloadRaw);

            // Invoke the data recv callback function
            if (DataRecv != null)
            {
                DataRecv.Invoke(devID, ctrlID, payload);
            }
        }

        public void Log(string format, params object[] args)
        {
            if (WriteLog != null) WriteLog.Invoke(format, args);
        }

        public DeviceControl GetVirtualControl(byte controlID)
        {
            return virtualControls[controlID];
        }

        public void UpdateVirtualControls()
        {
            // This must be called to raise data changed events
            foreach (var c in virtualControls)
            {
                c.Update();
            }
        }

        void SetupNetworkThreads()
        {
            // Thread that runs the UDP server
            udpNetworkThread = new Thread(() =>
            {
                udpListener = new UdpClient(Port);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, Port);
                Log("Listening UDP {0}", Port);

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
                        Log(exc.ToString());
                    }
                }
            });

            // Thread that runs the TCP server
            tcpNetworkThread = new Thread(() =>
            {
                tcpListener = new TcpListener(IPAddress.Any, Port);
                tcpListener.Start();
                Log("Listening TCP {0}", Port);
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
            byte opcode = 0;
            TcpDeviceHandler handler = null;

            try
            {
                // Read 1 byte opcode
                // 0x00 to read host control, 0x01 to write its control to host
                readLen = stream.Read(buffer, 0, 1);
                if (readLen != 1)
                {
                    throw new IOException("Unexpected read in opcode");
                }
                opcode = buffer[0];

                // Dispatch opcode
                if (opcode == 0x00)
                {
                    // This client wants to listen to a control on the host
                    handler = new DeviceListenHandler(this, client);
                }
                else if (opcode == 0x01)
                {
                    // This client wants to push control data from the device to the game
                    handler = new DevicePushHandler(this, client);
                }
                else
                {
                    throw new NotSupportedException("Opcode not supported: " + opcode);
                }

                // Let TcpHandler handle the connection
                handler.HandleConnection();
            }
            catch (Exception exc)
            {
                Log(exc.ToString());
            }
            finally
            {
                Log("TCP: {0} disconnected", client.Client.RemoteEndPoint);
                // Close the client handler
                if (handler != null) handler.Close();
                // Close the client
                client.Close();
            }
        }
    }
}
