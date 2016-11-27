namespace Unioty
{
    /// <summary>
    /// Represents a control in a device. Must be unique per Unioty server
    /// </summary>
    public class DeviceControl
    {
        /// <summary>
        /// The event will be raised during a game tick, after data from this DeviceControl has been received. 
        /// Must only be raised during a game update, to ensure thread-safety
        /// </summary>
        public event DataReceivedEventHandler DataReceived;

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

        public DeviceControl(byte DeviceID, byte ControlID)
        {
            this.DeviceID = DeviceID;
            this.ControlID = ControlID;
        }

        public DeviceControl(int controlcode)
            :this((byte)(controlcode >> 8), (byte)controlcode)
        { }

        #region Public methods
        public void RaiseDataReceivedEvent(DataReceivedEventArgs e)
        {
            DataReceived(this, e);
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
