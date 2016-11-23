using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Unioty
{
    public class UniotyMaster : MonoBehaviour
    {
        public string Host = "localhost";
        public int Port = 25556;

        TcpClient tcpClient;
        NetworkStream networkStream;
        byte[] readBuffer = new byte[3];

        Dictionary<int, DeviceControl> controlMap = new Dictionary<int, DeviceControl>();
        Queue inputQueue = new Queue();
        Thread socketThread;
        EventWaitHandle updateWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        #region Public methods
        /// <summary>
        /// Gets the device control in the control map. If it's not in the map already, it'll be created.
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="ctrlID"></param>
        /// <returns></returns>
        public DeviceControl GetDeviceControl(byte devID, byte ctrlID)
        {
            // Check if control is already in the hash map
            var controlCode = DeviceControl.GetControlCode(devID, ctrlID);
            if (!controlMap.ContainsKey(controlCode))
            {
                controlMap.Add(controlCode, new DeviceControl(controlCode));
            }
            return controlMap[controlCode];
        }
        #endregion

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
                var e = Queue.Synchronized(inputQueue).Dequeue();
                if (e != null)
                {
                    // Raise the received event for this control
                    var args = (InputReceivedEventArgs)e;
                    controlMap[args.ControlCode].RaiseInputReceivedEvent(args);
                }
            }
        }

        void OnDestroy()
        {
            socketThread.Abort();
        }

        void NetworkUpdate()
        {
            updateWaitHandle.WaitOne();
            // Attempt to reconnect if disconnected
            MaintainConnection();
            foreach (var devBtn in controlMap.Values)
            {
                WriteSocket(devBtn.QueryMessage);
            }
            // Read the state
            ReadSocket();
        }

        void SetupSocket()
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

        void WriteSocket(byte[] data)
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

        void ReadSocket()
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
                    var args = new InputReceivedEventArgs(readBuffer[0], readBuffer[1], readBuffer[2]);
                    // Enqueue the input. We process it later during Update
                    Queue.Synchronized(inputQueue).Enqueue(args);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Socket read error: " + e);
            }
        }

        void CloseSocket()
        {
            if (!tcpClient.Connected) return;
            tcpClient.Close();
        }

        void MaintainConnection()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                SetupSocket();
            }
        }
    }
}