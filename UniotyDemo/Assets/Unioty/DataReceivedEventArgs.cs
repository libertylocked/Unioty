using System;

namespace Unioty
{
    public class DataReceivedEventArgs : EventArgs
    {
        public byte DeviceID { get; private set; }
        public byte ControlID { get; private set; }
        public Payload Payload { get; private set; }

        public int ControlCode
        {
            get { return DeviceControl.GetControlCode(DeviceID, ControlID); }
        }

        public DataReceivedEventArgs(byte devId, byte ctrlId, Payload payload)
            :base()
        {
            DeviceID = devId;
            ControlID = ctrlId;
            Payload = payload;
        }
    }
}
