using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Messages
{
    public class Frame
    {
        byte[] m_bytes;
        public Frame(byte[] bytes)
        {
            if (bytes.Length != 15)
                throw new ArgumentOutOfRangeException("bytes", "Invalid CAN message length (expected 15 bytes)");

            m_bytes = bytes.ToArray();
        }

        public byte[] RawData { get { return m_bytes; } }


        public enum ControlByte : byte
        {
            StartFrame = 0xAA,
            EndFrame = 0xA5
        }

        public ControlByte Start
        {
            get
            {
                return (ControlByte)m_bytes[0];
            }
            set
            {
                m_bytes[0] = (byte)value;
            }
        }

        public Int16 Type
        {
            get
            {
                return (Int16)(((m_bytes[1]) << 4) + (((Int16)m_bytes[2] & (Int16)0xF0) >> 4));
            }
            set
            {
                m_bytes[1] = (byte)(value >> 4);
                m_bytes[2] &= 0x0F;
                m_bytes[2] |= (byte)(value << 4);
            }
        }

        public byte Flags
        {
            get
            {
                return (byte)(m_bytes[2] & 0x0F);
            }
            set
            {
                m_bytes[2] &= 0xF0;
                m_bytes[2] |= (byte)(value & 0x0F);
            }
        }

        public enum Flag : byte
        {
            NormalMessage = 0x0,
            ResponseMessage = 0x1
        }

        public bool IsResponse
        {
            get { return Flags == (byte)Flag.ResponseMessage; }
        }

        public byte ModuleNumber
        {
            get
            {
                return m_bytes[3];
            }
            set
            {
                m_bytes[3] = value;
            }
        }

        public byte GroupNumber
        {
            get
            {
                return m_bytes[4];
            }
            set
            {
                m_bytes[4] = value;
            }
        }

        public byte GetData(byte index)
        {
            if (index < 0 || index > 7)
                throw new ArgumentException("Invalid Data byte index [0-7]");
            return m_bytes[5 + index];
        }

        public void SetData(byte index, byte value)
        {
            if (index < 0 || index > 7)
                throw new ArgumentException("Invalid Data byte index [0-7]");

            m_bytes[5 + index] = value;
        }

        public byte ControlSum
        {
            get
            {
                return m_bytes[13];
            }
            set
            {
                m_bytes[13] = value;
            }
        }

        public Frame.ControlByte Stop
        {
            get
            {
                return (ControlByte)m_bytes[14];
            }
            set
            {
                m_bytes[14] = (byte)value;
            }
        }

        public void CalculateControlSum()
        {
            byte sum = 0;
            for (byte i = 1; i <= 12; i++)
                sum += m_bytes[i];

            ControlSum = sum;
        }

        public override string ToString()
        {
            string rxLine = "";
            int index = 0;
            foreach (byte dataByte in m_bytes)
            {
                rxLine += string.Format("{0:X2}{1}", dataByte, (index == 2 || index == 4 || index == 12) ? "    " : " ");
                index++;
            }
            return rxLine;
        }
    }
}
