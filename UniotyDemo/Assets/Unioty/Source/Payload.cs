using System;
using System.Linq;
using System.Text;

namespace Unioty
{
    public class Payload
    {
        public PayloadType PayloadType
        {
            get;
            private set;
        }

        public byte[] Raw
        {
            get;
            private set;
        }

        public object Data
        {
            get;
            private set;
        }

        public Payload(PayloadType payloadType, byte[] payloadRaw)
        {
            PayloadType = payloadType;
            Raw = payloadRaw;
            Data = ConvertPayloadRawToObject(payloadType, payloadRaw);
        }

        object ConvertPayloadRawToObject(PayloadType payloadType, byte[] payloadRaw)
        {
            // Converts payload to the corresponding type
            object payloadObject = payloadRaw;

            // Raw payload is in little edian. Need convertion for numeral values
            if (!BitConverter.IsLittleEndian
                && payloadType != PayloadType.String && payloadType != PayloadType.Raw)
            {
                payloadRaw.Reverse();
            }

            if (payloadType == PayloadType.Float32)
            {
                payloadObject = BitConverter.ToSingle(payloadRaw, 0);
            }
            else if (payloadType == PayloadType.Int32)
            {
                payloadObject = BitConverter.ToInt32(payloadRaw, 0);
            }
            else if (payloadType == PayloadType.Byte)
            {
                if (payloadRaw.Length < 1)
                    throw new ArgumentException("Payload array must have at least 1 byte");
                payloadObject = payloadRaw[0];
            }
            else if (payloadType == PayloadType.String)
            {
                payloadObject = Encoding.ASCII.GetString(payloadRaw);
            }
            else if (payloadType == PayloadType.Raw)
            {
                // Convertion not needed
            }
            else
            {
                throw new ArgumentException("Unknown payload type!");
            }

            return payloadObject;
        }
    }
}
