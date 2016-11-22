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
        public DeviceControl[] DeviceControls;

        TcpClient tcpClient;
        NetworkStream networkStream;
        byte[] readBuffer = new byte[3];

        Queue inputQueue = new Queue();
        Thread socketThread;
        EventWaitHandle updateWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        void Start()
        {
            socketThread = new Thread(() => 
            {
                while (true)
                {
                    NetworkUpdate();
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

        public void NetworkUpdate()
        {
            updateWaitHandle.WaitOne();
            // Attempt to reconnect if disconnected
            MaintainConnection();
            foreach (var devBtn in DeviceControls)
            {
                WriteSocket(devBtn.GetRequestMessage());
            }
            // Read the state
            ReadSocket();
        }

        public void SetupSocket()
        {
            try
            {
                tcpClient = new TcpClient(Host, Port);
                networkStream = tcpClient.GetStream();
                // Tell server that we are a listener
                WriteSocket(new byte[] { 0x00 });
            }
            catch (Exception e)
            {
                Debug.Log("Socket setup error: " + e);
            }
        }

        public void WriteSocket(byte[] data)
        {
            if (!tcpClient.Connected) return;
            try
            {
                networkStream.Write(data, 0, data.Length);
                networkStream.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Socket write error: " + e);
            }
        }

        public void ReadSocket()
        {
            if (!tcpClient.Connected) return;
            try
            {
                if (networkStream.DataAvailable)
                {
                    // Read 3 bytes, [dev id, ctrl id, ctrl state]
                    int readLen = networkStream.Read(readBuffer, 0, 3);
                    if (readLen != 3)
                    {
                        // Error: unexpected read length
                        throw new Exception("Unexpected read length!");
                    }
                    var args = new InputReceiveEventArgs(readBuffer[0], readBuffer[1], readBuffer[2]);
                    // Enqueue the input. We process it later during Update
                    Queue.Synchronized(inputQueue).Enqueue(args);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Socket read error: " + e);
            }
        }

        public void CloseSocket()
        {
            if (!tcpClient.Connected) return;
            tcpClient.Close();
        }

        public void MaintainConnection()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                SetupSocket();
            }
        }
    }
}