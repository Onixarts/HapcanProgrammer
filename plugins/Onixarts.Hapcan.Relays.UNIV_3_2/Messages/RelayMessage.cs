using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Relays.UNIV_3_2.Messages
{
    public enum RelayStatus
    {
        Off = 0x00,
        On = 0xFF
    }

    public class RelayMessage : Message
    {
        public RelayMessage() : base((short) Module.FrameType.RelayMessage)
        {
        }

        public RelayMessage(Frame frame) : base(frame)
        {
        }

        public byte Channel
        {
            get
            {
                return Frame.GetData(2);
            }
            set
            {
                Frame.SetData(2, value);
            }
        }
        public byte Status
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
        public byte Instr1
        {
            get
            {
                return Frame.GetData(5);
            }
            set
            {
                Frame.SetData(5, value);
            }
        }
        public byte Instr2
        {
            get
            {
                return Frame.GetData(6);
            }
            set
            {
                Frame.SetData(6, value);
            }
        }
        public byte Timer
        {
            get
            {
                return Frame.GetData(7);
            }
            set
            {
                Frame.SetData(7, value);
            }
        }

        public override string ToString()
        {
            return string.Format("Relay{0}: channel {1} turned {2}",
                Frame.IsResponse ? " (response)" : "",
                Channel,
                Status == (byte)RelayStatus.On ? "on" : "off"
                );
        }

    }
}
