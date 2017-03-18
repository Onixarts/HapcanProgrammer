using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.Messages;
using Onixarts.Hapcan.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Gates.UNIV_3_102
{
    [Export(typeof(IDevicePlugin))]
    public class Module : IDevicePlugin
    {
        public string HardwareVersionName { get { return "UNIV 3"; } }
        public short HardwareType { get { return 0x3000; } }
        public byte HardwareVersion { get { return 0x03; } }
        public byte ApplicationType { get { return 102; } }


        public enum FrameType
        {
            Empty = 0,
            RTCMessage = 0x300,
        }


        public Message GetMessage(Frame frame)
        {
            switch ((FrameType)frame.Type)
            {
                case FrameType.Empty: break;
                case FrameType.RTCMessage: return new Messages.RTCMessage(frame);
            }
            return null;
        }

        public bool HandleMessage(Message msg)
        {
            return false;
        }

        public IEnumerable<MenuItem> DevicesListContextMenuItems
        {
            get
            {
                return null;// new[] { new MenuItem() { DisplayName = "Reset Device" } };
            }
        }
    }
}
