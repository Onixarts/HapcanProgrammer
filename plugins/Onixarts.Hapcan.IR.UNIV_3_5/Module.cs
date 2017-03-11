using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.IR.UNIV_3_5
{
    [Export(typeof(IHapcanDevice))]
    public class Module : IHapcanDevice
    {
        public string HardwareVersionName { get { return "UNIV 3"; } }
        public short HardwareType { get { return 0x3000; } }
        public byte HardwareVersion { get { return 0x03; } }


        public enum FrameType
        {
            Empty = 0,
            IRReceiverMessage = 0x303,
        }


        public Message GetMessage(Frame frame)
        {
            switch ((FrameType)frame.Type)
            {
                case FrameType.Empty: break;
                case FrameType.IRReceiverMessage: return new Messages.IRReceiverMessage(frame);
            }
            return null;
        }

        public bool HandleMessage(Message msg)
        {
            return false;
        }

    }
}
