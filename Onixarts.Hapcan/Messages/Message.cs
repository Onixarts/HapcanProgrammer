using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Messages
{
    public class Message
    {

        public Message(short type)
        {
            Frame = new Frame(new byte[15]);
            Frame.Start = Frame.ControlByte.StartFrame;
            Frame.Stop = Frame.ControlByte.EndFrame;
            for (byte i = 0; i < 8; i++)
                Frame.SetData(i, 0xFF);
            Frame.Type = type;
            Received = false;
        }

        public Message(Frame frame)
        {
            Frame = frame;
            Received = true;
            Time = DateTime.Now;
        }

        public Frame Frame { get; private set; }
        public bool Received { get; private set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return string.Format("Unknown message type (0x{0:X2})", Frame.Type );
        }
    }
}
