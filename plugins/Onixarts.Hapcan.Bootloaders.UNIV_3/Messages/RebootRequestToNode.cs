using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class RebootRequestToNode : RequestMessage
    {
        public RebootRequestToNode() : base((short)Module.FrameType.RebootMessageToNode)
        {
        }

        public RebootRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return string.Format("Reboot request to node {0} in group {1}",
                RequestedModuleNumber.ToString(),
                RequestedGroupNumber.ToString()
            );
        }

    }
}
