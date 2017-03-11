using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class FirmwareTypeRequestToGroup : RequestMessage
    {
        public FirmwareTypeRequestToGroup() : base((short)Module.FrameType.FirmwareTypeMessageToGroup)
        {
            RequestedModuleNumber = 0;
        }

        public FirmwareTypeRequestToGroup(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return string.Format("Firmware type request to group {0}",
                RequestedGroupNumber.ToString()
            );
        }

    }
}
