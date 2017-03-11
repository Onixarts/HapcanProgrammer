using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class HardwareTypeResponse : ResponseMessage
    {
        public HardwareTypeResponse() : base((short)Module.FrameType.HardwareType)
        {
        }

        public HardwareTypeResponse(Frame frame) : base(frame)
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
        public int SerialNumber
        {
            get
            {
                return (Frame.GetData(4) << 24) + (Frame.GetData(5) << 16) + (Frame.GetData(6) << 8) + Frame.GetData(7);
            }
            set
            {
                Frame.SetData(4, (byte)(value >> 24));
                Frame.SetData(5, (byte)((value & 0x00FF0000) >> 16));
                Frame.SetData(6, (byte)((value & 0x0000FF00) >> 8));
                Frame.SetData(7, (byte)((value & 0x000000FF)));
            }
        }



        //TODO: HardwareVersion wziąć z pluginów
        public override string ToString()
        {
            //HapcanManager.HapcanDevicePlugins.FirstOrDefault
            return string.Format("Hardware type response: Type: 0x{0:X4}, HVers: {1}, ID: 0x{2:X8}",
                HardwareType,
                HardwareVersion,
                SerialNumber
            );
        }

    }
}
