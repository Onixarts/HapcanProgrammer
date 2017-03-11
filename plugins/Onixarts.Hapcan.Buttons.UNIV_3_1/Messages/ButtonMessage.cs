using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Buttons.UNIV_3_1.Messages
{
    public enum ButtonState
    {
        Open = 0x00,
        Closed = 0xFF,
        ClosedHeld400ms = 0xFE,
        ClosedHeld4s = 0xFD,
        ClosedOpenIn400ms = 0xFC,
        ClosedOpen400ms_4s = 0xFB,
        ClosedOpenAfter4s = 0xFA
    }

    public enum LEDState : byte
    {
        Off = 0x00,
        On = 0xFF
    }

    public class ButtonMessage : Message
    {
        public ButtonMessage() : base((short) Module.FrameType.ButtonMessage)
        {
        }

        public ButtonMessage(Frame frame) : base(frame)
        {
        }

        public byte Channel
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
        public byte Button
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
        public byte LED
        {
            get
            {
                return Frame.GetData(4);
            }
            set
            {
                Frame.SetData(4, value);
            }
        }

        public string StateString
        {
            get
            {
                switch ((ButtonState)Button)
                {
                    case ButtonState.Open:
                        return "released";
                    case ButtonState.Closed:
                        return "pressed";
                    case ButtonState.ClosedHeld400ms:
                        return "held for 400ms";
                    case ButtonState.ClosedHeld4s:
                        return "held for 4s";
                    case ButtonState.ClosedOpenIn400ms:
                        return "released before 400ms";
                    case ButtonState.ClosedOpen400ms_4s:
                        return "released between 400ms and 4s";
                    case ButtonState.ClosedOpenAfter4s:
                        return "released after 4s";
                    default:
                        return "Unknown message";
                }
            }
            private set { }
        }


        public override string ToString()
        {
            return string.Format("Button {0} {1}",
                Channel,
                StateString
                //LED == (byte) LEDState.On ? "on" : "off"
                );
        }

    }
}
