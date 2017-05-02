using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class SetDefaultNodeAndGroupResponse : RequestMessage
    {
        public SetDefaultNodeAndGroupResponse() : base((short)Module.FrameType.SetDefaultNodeAndGroupMessageToNode)
        {
        }

        public SetDefaultNodeAndGroupResponse(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Default node and group restored to ({Frame.ModuleNumber}, {Frame.GroupNumber})";
        }

    }
}
