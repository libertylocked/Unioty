using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Unioty
{
    public class ControllerHubListener : MonoBehaviour
    {
        public string Host = "localhost";
        public int Port = 25556;
        public event InputReceiveEventHandler InputReceived;
        public DeviceButton[] DeviceButtons;

        TcpClient tcpClient;
        NetworkStream networkStream;
        byte[] buf = new byte[3];

        Queue inputQueue = new Queue();
        Thread socketThread;
        EventWaitHandle updateWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public bool SocketReady
        {
            get;
            private set;
        }

        void Start()
        {
            SetupSocket();
            // Tell the server that we are a listener
            WriteSocket(new byte[] { 0x00 });
            socketThread = new Thread(() => 
            {
                while (true)
                {
                    updateWaitHandle.WaitOne();
                    foreach (var devBtn in DeviceButtons)
                    {
                        WriteSocket(devBtn.GetRequestMessage());
                    }
                    // Read the state
                    ReadSocket();
                }
            });
            socketThread.Start();
        }

        void Update()
        {
            updateWaitHandle.Set();
            if (Queue.Synchronized(inputQueue).Count > 0)
            {
                var args = Queue.Synchronized(inputQueue).Dequeue();
                if (args != null && InputReceived != null)
                {
                    InputReceived(this, (InputReceiveEventArgs)args);
                }
            }
        }

        void OnDestroy()
        {
            socketThread.Abort();
        }

        public void SetupSocket()
        {
            try
            {
                tcpClient = new TcpClient(Host, Port);
                networkStream = tcpClient.GetStream();
                SocketReady = true;
            }
            catch (Exception e)
            {
                Debug.Log("Socket error:" + e);
            }
        }

        public void WriteSocket(byte[] data)
        {
            if (!SocketReady)
                return;
            networkStream.Write(data, 0, data.Length);
            networkStream.Flush();
        }

        public void ReadSocket()
        {
            if (!SocketReady)
            {
                return;
            }
            if (networkStream.DataAvailable)
            {
                // Read 3 bytes, [dev id, ctrl id, ctrl state]
                int readLen = networkStream.Read(buf, 0, 3);
                if (readLen != 3)
                {
                    // Error: unexpected read length
                }
                var args = new InputReceiveEventArgs(buf[0], buf[1], buf[2]);
                // Enqueue the input. We process it later during Update
                Queue.Synchronized(inputQueue).Enqueue(args);
            }
        }

        public void CloseSocket()
        {
            if (!SocketReady)
                return;
            tcpClient.Close();
            SocketReady = false;
        }

        public void MaintainConnection()
        {
            if (!networkStream.CanRead)
            {
                SetupSocket();
            }
        }
    }
}