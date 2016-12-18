using System.Collections;
using System.Collections.Generic;
using Unioty.Controls;

namespace Unioty
{
    /// <summary>
    /// Wrapper of UniotyServer that uses thread-safe event-based callbacks 
    /// </summary>
    public class UniotyServerUpdater : IUniotyServer, IUpdate
    {
        const int MAX_EVENTS_PER_UPDATE = 20;

        UniotyServer server;
        Dictionary<int, DeviceControl> controlMap = new Dictionary<int, DeviceControl>();

        public int Port
        {
            get { return server.Port; }
        }

        public UniotyServerUpdater(int port, WriteLogDelegate writeLogFunc)
        {
            server = new UniotyServer(port, writeLogFunc, OnDataReceived);
        }

        public void Start()
        {
            server.Start();
        }

        public void Stop()
        {
            server.Stop();
        }

        public void Update()
        {
            foreach (var control in controlMap.Values)
            {
                control.Update();
            }
            server.UpdateVirtualControls();
        }

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

        void OnDataReceived(byte devID, byte ctrlID, Payload payload)
        {
            // Find the control in the map
            var control = GetDeviceControl(devID, ctrlID);
            control.Payload = payload;
        }

        public DeviceControl GetVirtualControl(byte controlID)
        {
            return server.GetVirtualControl(controlID);
        }
    }
}
