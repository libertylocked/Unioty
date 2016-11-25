using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Unioty
{
    public class UniotyMasterScript : MonoBehaviour
    {
        public int Port = 25556;

        UniotyServer server;
        Dictionary<int, DeviceControl> controlMap = new Dictionary<int, DeviceControl>();
        Queue deviceDataQueue = new Queue(); // Queue used to read data from device

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
            server = new UniotyServer(Port, Debug.LogFormat, OnDataReceived);
            server.Start();
        }

        void Update()
        {
            if (Queue.Synchronized(deviceDataQueue).Count > 0)
            {
                var e = Queue.Synchronized(deviceDataQueue).Dequeue();
                if (e != null)
                {
                    // Raise the received event for this control
                    var args = (DataReceivedEventArgs)e;
                    if (controlMap.ContainsKey(args.ControlCode))
                    {
                        controlMap[args.ControlCode].RaiseDataReceivedEvent(args);
                    }
                }
            }
        }

        void OnDestroy()
        {
            server.Stop();
        }

        private void OnDataReceived(byte devID, byte ctrlID, PayloadType payloadType, object payload)
        {
            var args = new DataReceivedEventArgs(devID, ctrlID, payloadType, payload);
            Queue.Synchronized(deviceDataQueue).Enqueue(args);
        }
    }
}