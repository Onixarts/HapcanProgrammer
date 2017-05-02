using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class EnterProgrammingModeRequestToNode : RequestMessage
    {
        public EnterProgrammingModeRequestToNode() : base((short)Module.FrameType.EnterProgrammingMode)
        {
        }

        public EnterProgrammingModeRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Enter programming mode request to ({RequestedModuleNumber}, {RequestedGroupNumber})";
        }

    }
}
