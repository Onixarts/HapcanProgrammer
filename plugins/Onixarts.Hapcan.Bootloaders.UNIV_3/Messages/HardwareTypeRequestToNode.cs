using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class HardwareTypeRequestToNode : RequestMessage
    {
        public HardwareTypeRequestToNode() : base((short)Module.FrameType.HardwareTypeMessageToNode)
        {
        }

        public HardwareTypeRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Hardware type request to node ({RequestedModuleNumber}, {RequestedGroupNumber})";
        }

    }
}
