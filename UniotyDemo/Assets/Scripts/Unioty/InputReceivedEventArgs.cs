using System;

namespace Unioty
{
    public class InputReceivedEventArgs : EventArgs
    {
        public byte DeviceID { get; private set; }
        public byte ControlID { get; private set; }
        public byte State { get; private set; }
        public int ControlCode
        {
            get { return DeviceControl.GetControlCode(DeviceID, ControlID); }
        }

        public InputReceivedEventArgs(byte devId, byte ctrlId, byte state)
            :base()
        {
            DeviceID = devId;
            ControlID = ctrlId;
            State = state;
        }
    }
}
