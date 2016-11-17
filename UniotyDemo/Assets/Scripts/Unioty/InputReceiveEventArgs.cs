using System;

namespace Unioty
{
    public class InputReceiveEventArgs : EventArgs
    {
        public byte DeviceID { get; private set; }
        public byte ControlID { get; private set; }
        public byte State { get; private set; }

        public InputReceiveEventArgs(byte devId, byte ctrlId, byte state)
            :base()
        {
            DeviceID = devId;
            ControlID = ctrlId;
            State = state;
        }
    }
}
