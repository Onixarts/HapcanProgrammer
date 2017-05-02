using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class SetDefaultNodeAndGroupRequestToNode : RequestMessage
    {
        public SetDefaultNodeAndGroupRequestToNode() : base((short)Module.FrameType.SetDefaultNodeAndGroupMessageToNode)
        {
        }

        public SetDefaultNodeAndGroupRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Set default node and group request to ({RequestedModuleNumber}, {RequestedGroupNumber})";
        }
    }
}
