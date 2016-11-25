using System;

namespace Unioty
{
    public class DataReceivedEventArgs : EventArgs
    {
        public byte DeviceID { get; private set; }
        public byte ControlID { get; private set; }
        public PayloadType PayloadType { get; private set; }
        public object Payload { get; private set; }

        public int ControlCode
        {
            get { return DeviceControl.GetControlCode(DeviceID, ControlID); }
        }

        public DataReceivedEventArgs(byte devId, byte ctrlId, PayloadType payloadType, object payload)
            :base()
        {
            DeviceID = devId;
            ControlID = ctrlId;
            PayloadType = payloadType;
            Payload = payload;
        }
    }
}
