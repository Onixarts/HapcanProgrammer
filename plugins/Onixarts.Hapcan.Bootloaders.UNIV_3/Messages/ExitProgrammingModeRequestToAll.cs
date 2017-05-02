using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class ExitProgrammingModeRequestToAll : RequestMessage
    {
        public ExitProgrammingModeRequestToAll() : base((short)Module.FrameType.ExitAllFromBootloaderProgrammingMode)
        {
        }

        public ExitProgrammingModeRequestToAll(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Exit all nodes from programming mode";
        }

    }
}
