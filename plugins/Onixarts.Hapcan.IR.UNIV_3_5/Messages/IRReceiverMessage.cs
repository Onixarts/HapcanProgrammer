using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.IR.UNIV_3_5.Messages
{

    public enum Codes
    {
        SIRC12 = 0x03,
        SIRC15 = 0x04,
        SIRC20 = 0x05,
        RC5 = 0x06,
        NEC16 = 0x07,
        NEC24 = 0x08,
    }

    public class IRReceiverMessage : Message
    {
        public IRReceiverMessage() : base((short) Module.FrameType.IRReceiverMessage)
        {
        }

        public IRReceiverMessage(Frame frame) : base(frame)
        {
        }

        public byte CodeType
        {
            get
            {
                return (byte)(Frame.GetData(2) & 0x7F); // najstarszy bit jest ustawiany na 1 jeśli jest to zakończenie kodu, więc trzeba go wyrzucić
            }
            set
            {
                Frame.SetData(2, value);
            }
        }
        public byte Code1
        {
            get
            {
                return Frame.GetData(3);
            }
            set
            {
                Frame.SetData(3, value);
            }
        }
        public byte Code2
        {
            get
            {
                return Frame.GetData(4);
            }
            set
            {
                Frame.SetData(4, value);
            }
        }
        public byte Code3
        {
            get
            {
                return Frame.GetData(5);
            }
            set
            {
                Frame.SetData(6, value);
            }
        }
        public int Address
        {
            get
            {
                switch ((Codes)CodeType)
                {
                    case Codes.SIRC12:
                    case Codes.SIRC15:
                    case Codes.RC5:
                    case Codes.NEC16:
                        return Code1;
                    case Codes.SIRC20:
                    case Codes.NEC24:
                        return Code1 << 8 + Code2;
                    default:
                        return 0;
                }
            }
            private set { }
        }

        public int Command
        {
            get
            {
                switch ((Codes)CodeType)
                {
                    case Codes.SIRC12:
                    case Codes.SIRC15:
                    case Codes.RC5:
                    case Codes.NEC16:
                        return Code2;
                    case Codes.SIRC20:
                    case Codes.NEC24:
                        return Code3;
                    default:
                        return 0;
                }
            }
            private set { }
        }

        public bool EOT
        {
            get { return (Frame.GetData(2) & 0x80) == 0x80; } // jesli w typie kodu nastarszy bit jest = 1 to oznacza koniec nadawania
            private set { }
        }

        public override string ToString()
        {
            return string.Format("IR receiver code: {0}, adr: {1}, cmd: {2} {3}",
                Enum.GetName(typeof(Codes), CodeType),
                Address,
                Command,
                EOT ? ", EOT" : ""
                );
        }

    }
}
