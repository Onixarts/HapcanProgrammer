using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class ExitProgrammingModeRequestToNode : RequestMessage
    {
        public ExitProgrammingModeRequestToNode() : base((short)Module.FrameType.ExitOneNodeFromBootloaderProgrammingMode)
        {
        }

        public ExitProgrammingModeRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Exit from programming mode request to node {Frame.ModuleNumber} in group {Frame.GroupNumber}";
        }

    }
}
