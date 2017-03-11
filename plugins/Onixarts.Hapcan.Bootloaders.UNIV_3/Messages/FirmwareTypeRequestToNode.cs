using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class FirmwareTypeRequestToNode : RequestMessage
    {
        public FirmwareTypeRequestToNode() : base((short)Module.FrameType.FirmwareTypeMessageToNode)
        {
        }

        public FirmwareTypeRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return string.Format("Firmware type request to node {0} in group {1}",
                RequestedModuleNumber.ToString(),
                RequestedGroupNumber.ToString()
            );
        }

    }
}
