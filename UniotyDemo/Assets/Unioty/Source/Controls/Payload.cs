using System;
using System.Linq;
using System.Text;

namespace Unioty.Controls
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
            :this(payloadType, payloadRaw, ConvertPayloadRawToObject(payloadType, payloadRaw))
        { }

        Payload(PayloadType payloadType, byte[] payloadRaw, object data)
        {
            PayloadType = payloadType;
            Raw = payloadRaw;
            Data = data;
        }

        public static Payload FromObject(object payloadObject)
        {
            Type type = payloadObject.GetType();
            byte[] payloadRaw;
            PayloadType payloadType;

            if (type == typeof(Single))
            {
                payloadRaw = BitConverter.GetBytes((float)payloadObject);
                payloadType = PayloadType.Float32;
            }
            else if (type == typeof(Int32))
            {
                payloadRaw = BitConverter.GetBytes((int)payloadObject);
                payloadType = PayloadType.Int32;
            }
            else if (type == typeof(Byte))
            {
                payloadRaw = new byte[] { (byte)payloadObject };
                payloadType = PayloadType.Byte;
            }
            else if (type == typeof(String))
            {
                payloadRaw = Encoding.ASCII.GetBytes((string)payloadObject);
                payloadType = PayloadType.String;
            }
            else if (type == typeof(byte[]))
            {
                payloadRaw = (byte[])payloadObject;
                payloadType = PayloadType.Raw;
            }
            else
            {
                throw new ArgumentException("Unknown payload type!");
            }

            // Raw payload is in little edian. Need convertion for numeral values
            if (!BitConverter.IsLittleEndian
                && payloadType != PayloadType.String && payloadType != PayloadType.Raw)
            {
                payloadRaw.Reverse();
            }

            return new Payload(payloadType, payloadRaw, payloadObject);
        }

        static object ConvertPayloadRawToObject(PayloadType payloadType, byte[] payloadRaw)
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
