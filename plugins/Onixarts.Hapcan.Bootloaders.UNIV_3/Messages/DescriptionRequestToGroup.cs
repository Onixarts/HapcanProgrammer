using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class DescriptionRequestToGroup : RequestMessage
    {
        public DescriptionRequestToGroup() : base((short)Module.FrameType.DescriptionMessageToGroup)
        {
            RequestedModuleNumber = 0;
        }

        public DescriptionRequestToGroup(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return string.Format("Description request to group {0}",
                RequestedGroupNumber.ToString()
            );
        }

    }
}
