using System;

namespace Unioty
{
    //[Serializable]
    public class DeviceControl
    {
        public event InputReceivedEventHandler InputReceived;

        public byte DeviceID
        {
            get;
            private set;
        }

        public byte ControlID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message required by the server to query the status 
        /// of this control.
        /// </summary>
        public byte[] QueryMessage
        {
            get;
            private set;
        }
        
        public DeviceControl(byte DeviceID, byte ControlID)
        {
            this.DeviceID = DeviceID;
            this.ControlID = ControlID;
            QueryMessage = new byte[] { DeviceID, ControlID };
        }

        public DeviceControl(int controlcode)
            :this((byte)(controlcode >> 8), (byte)controlcode)
        { }

        #region Public methods
        public void RaiseInputReceivedEvent(InputReceivedEventArgs e)
        {
            InputReceived(this, e);
        }

        #endregion

        #region Static methods
        public static int GetControlCode(byte deviceID, byte controlID)
        {
            // ControlID is lower 8 bits, DeviceID is upper 8 bits.
            // The rest 16 bits are zero
            return deviceID << 8 | controlID;
        }
        #endregion

        #region Object method overrides
        public override int GetHashCode()
        {
            // Hash code is control code
            // Upper 16 bits are 0, devID (8 bits), ctrlID (8 bits)
            return GetControlCode(DeviceID, ControlID);
        }

        public override bool Equals(object obj)
        {
            return (obj.GetType() == typeof(DeviceControl) && obj.GetHashCode() == GetHashCode());
        }
        #endregion
    }
}
