using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class HardwareTypeRequestToGroup : RequestMessage
    {
        public HardwareTypeRequestToGroup() : base((short)Module.FrameType.HardwareTypeMessageToGroup)
        {
            RequestedModuleNumber = 0;
        }

        public HardwareTypeRequestToGroup(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return string.Format("Hardware type request to group {0}",
                RequestedGroupNumber.ToString()
            );
        }

    }
}
