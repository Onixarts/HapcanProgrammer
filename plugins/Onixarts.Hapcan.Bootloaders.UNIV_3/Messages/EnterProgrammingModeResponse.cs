using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class EnterProgrammingModeResponse : ResponseMessage
    {
        public EnterProgrammingModeResponse() : base((short)Module.FrameType.EnterProgrammingMode)
        {
        }

        public EnterProgrammingModeResponse(Frame frame) : base(frame)
        {
        }

        public byte BootloaderVersion1
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

        public byte BootloaderVersion2
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
            return $"Module ({Frame.ModuleNumber}, {Frame.GroupNumber}) entered programming mode";
        }

    }
}
