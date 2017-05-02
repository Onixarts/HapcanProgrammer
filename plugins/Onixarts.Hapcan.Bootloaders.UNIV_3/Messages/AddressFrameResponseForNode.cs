using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class AddressFrameResponseForNode : ResponseMessage
    {
        public AddressFrameResponseForNode() : base((short)Module.FrameType.AddressFrame)
        {
        }

        public AddressFrameResponseForNode(Frame frame) : base(frame)
        {
        }

        public int Address
        {
            get
            {
                return ADRU << 16 | ADRH << 8 | ADRL;
            }
            set
            {
                ADRU = (byte)(value >> 16);
                ADRH = (byte)(value >> 8);
                ADRL = (byte)value;
            }
        }

        public byte ADRU
        {
            get
            {
                return Frame.GetData(0);
            }
            set
            {
                Frame.SetData(0, value);
            }
        }

        public byte ADRH
        {
            get
            {
                return Frame.GetData(1);
            }
            set
            {
                Frame.SetData(1, value);
            }
        }

        public byte ADRL
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

        public ProgrammingCommand Command
        {
            get
            {
                return (ProgrammingCommand)Frame.GetData(5);
            }
            set
            {
                Frame.SetData(5, (byte)value);
            }
        }

        public override string ToString()
        {
            return $"Address frame ACK from ({Frame.ModuleNumber}, {Frame.GroupNumber}) - {Command}";
        }

    }
}
