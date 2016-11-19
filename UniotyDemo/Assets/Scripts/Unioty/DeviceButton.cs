using System;

namespace Unioty
{
    [Serializable]
    public class DeviceButton
    {
        public byte DeviceID;
        public byte ControlID;

        byte[] message;
        
        public DeviceButton(byte DeviceID, byte ControlID)
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
