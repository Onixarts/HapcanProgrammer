using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Messages
{
    public class RequestMessage : Message
    {
        public RequestMessage(short type) : base(type)
        {
        }

        public RequestMessage(Frame frame) : base(frame)
        {
        }

        public byte CompID1
        {
            get
            {
                return Frame.ModuleNumber;
            }
            set
            {
                Frame.ModuleNumber = value;
            }
        }
        public byte CompID2
        {
            get
            {
                return Frame.GroupNumber;
            }
            set
            {
                Frame.GroupNumber = value;
            }
        }
        public byte RequestedModuleNumber
        {
            get
            {
                return Frame.GetData(2);
            }
            set
            {
                Frame.SetData(2, value);
            }
        }
        public byte RequestedGroupNumber
        {
            get
            {
                return Frame.GetData(3);
            }
            set
            {
                Frame.SetData(3, value);
            }
        }

        public override string ToString()
        {
            return string.Format("Unknown request: from {0}:{1} to {2}:{3}",
                CompID1,
                CompID2,
                RequestedModuleNumber,
                RequestedGroupNumber
                );
        }
    }
}
