using System;

namespace Unioty
{
    [Serializable]
    public class DeviceControl
    {
        public byte DeviceID;
        public byte ControlID;

        byte[] message;
        
        public DeviceControl(byte DeviceID, byte ControlID)
        {
            this.DeviceID = DeviceID;
            this.ControlID = ControlID;
        }

        public byte[] GetRequestMessage()
        {
            if (message == null)
            {
                message = new byte[] { DeviceID, ControlID };
            }
            return message;
        }
    }
}
