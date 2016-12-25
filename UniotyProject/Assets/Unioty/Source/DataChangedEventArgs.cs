using System;
using Unioty.Controls;

namespace Unioty
{
    public class DataChangedEventArgs : EventArgs
    {
        public DeviceControl Control
        {
            get;
            private set;
        }

        public byte DeviceID
        {
            get
            {
                return Control.DeviceID;
            }
        }

        public byte ControlID
        {
            get
            {
                return Control.ControlID;
            }
        }

        public Payload Payload
        {
            get
            {
                return Control.Payload;
            }
        }

        public int ControlCode
        {
            get
            {
                return Control.ControlCode;
            }
        }

        public DataChangedEventArgs(DeviceControl control)
            :base()
        {
            this.Control = control;
        }
    }
}
