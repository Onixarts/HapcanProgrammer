using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class FirmwareTypeResponse : ResponseMessage
    {
        public FirmwareTypeResponse() : base((short)Module.FrameType.FirmwareTypeMessageToGroup)
        {
        }

        public FirmwareTypeResponse(Frame frame) : base(frame)
        {
        }

        public short HardwareType
        {
            get
            {
                return (short)((Frame.GetData(0) << 8) + Frame.GetData(1));
            }
            set
            {
                Frame.SetData(0, (byte)(value >> 8));
                Frame.SetData(1, (byte)(value & 0x00FF));
            }
        }
        public byte HardwareVersion
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
        public string HardwareVersionText { get; set; }

        public byte ApplicationType
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

        public byte ApplicationVersion
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

        public byte FirmwareVersion
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

        public byte BootloaderVersion1
        {
            get
            {
                return Frame.GetData(6);
            }
            set
            {
                Frame.SetData(6, (byte)(value));
            }
        }

        public byte BootloaderVersion2
        {
            get
            {
                return Frame.GetData(7);
            }
            set
            {
                Frame.SetData(7, (byte)(value));
            }
        }

        // firmware type wziąc z pluginów
        public override string ToString()
        {
            return string.Format("Firmware type response: Hardware: 0x{0:X4}, Firmware: {1}.{2}.{3}.{4}",
                HardwareType,
                HardwareVersion,
                ApplicationType,
                ApplicationVersion,
                FirmwareVersion
            );
        }

    }
}
