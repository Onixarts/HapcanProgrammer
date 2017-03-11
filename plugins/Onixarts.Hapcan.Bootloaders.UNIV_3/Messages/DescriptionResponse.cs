using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class DescriptionResponse : ResponseMessage
    {
        public DescriptionResponse() : base((short)Module.FrameType.DescriptionMessageToGroup)
        {
        }

        public DescriptionResponse(Frame frame) : base(frame)
        {
        }

        public string Text
        {
            get
            {
                return Encoding.ASCII.GetString(Frame.RawData, 5, 8).Trim(new[] { '\0' });
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return string.Format("Description response: {0}",
                Text
            );
        }

    }
}
