namespace Unioty.Controls
{
    /// <summary>
    /// Represents a control in a device. Must be unique per Unioty server
    /// </summary>
    public class DeviceControl : IUpdate
    {
        Payload payload;
        bool payloadChanged = false;

        /// <summary>
        /// The event is raised when Update is called, after data on this DeviceControl has changed. 
        /// Must only be raised during a game update, to ensure thread-safety
        /// </summary>
        public event DataChangedEventHandler DataChanged;

        #region Properties
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

        public Payload Payload
        {
            get
            {
                return payload;
            }
            set
            {
                payload = value;
                payloadChanged = true;
            }
        }

        public int ControlCode
        {
            get
            {
                return GetControlCode(this.DeviceID, this.ControlID);
            }
        }
        #endregion

        public DeviceControl(byte DeviceID, byte ControlID)
        {
            this.DeviceID = DeviceID;
            this.ControlID = ControlID;
        }

        public DeviceControl(int controlcode)
            :this((byte)(controlcode >> 8), (byte)controlcode)
        { }

        #region Public methods
        public void Update()
        {
            // If payload is changed between updates, raise the event.
            // This ensures that the event is only raised once per Update maximum
            if (payloadChanged)
            {
                // Raise the event
                if (DataChanged != null)
                {
                    DataChanged(this, new DataChangedEventArgs(this));
                }
                payloadChanged = false;
            }
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
