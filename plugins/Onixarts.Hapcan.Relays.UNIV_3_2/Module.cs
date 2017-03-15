using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Relays.UNIV_3_2
{
    [Export(typeof(IDevicePlugin))]
    public class Module : IDevicePlugin
    {
        public string HardwareVersionName { get { return "UNIV 3"; } }
        public short HardwareType { get { return 0x3000; } }
        public byte HardwareVersion { get { return 0x03; } }
        public byte ApplicationType { get { return 0x02; } }

        public enum FrameType
        {
            Empty = 0,
            RelayMessage = 0x302,
        }


        public Message GetMessage(Frame frame)
        {
            switch ((FrameType)frame.Type)
            {
                case FrameType.Empty: break;
                case FrameType.RelayMessage: return new Messages.RelayMessage(frame);
            }
            return null;
        }

        public bool HandleMessage(Message msg)
        {
            return false;
        }

    }
}
