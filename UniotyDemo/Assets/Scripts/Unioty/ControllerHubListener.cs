using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Unioty
{
    public class ControllerHubListener : MonoBehaviour
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        string Host = "localhost";
        int Port = 25556;
        byte[] buf = new byte[3];

        Queue inputQueue = new Queue();
        Thread socketThread;
        EventWaitHandle updateWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public event InputReceiveEventHandler InputReceived;

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
                    // Tell the server that we want the state of 0x01 dev's 0x01 ctrl
                    WriteSocket(new byte[] { 0x01, 0x01 });
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