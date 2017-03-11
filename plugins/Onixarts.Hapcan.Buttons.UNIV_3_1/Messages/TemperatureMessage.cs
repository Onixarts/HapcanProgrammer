using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Buttons.UNIV_3_1.Messages
{
    public class TemperatureMessage : Message
    {
        public TemperatureMessage() : base((short)Module.FrameType.TemperatureMessage)
        {
            Frame.SetData(2, (byte)Module.FrameDataType.TemperatureFrame);
        }

        public TemperatureMessage(Frame frame) : base(frame)
        {
        }

        public float Temperature
        {
            get
            {
                return (float)((Frame.GetData(3) * 256 + Frame.GetData(4)) * 0.0625);
            }
            set
            {
                //TODO
                Frame.SetData(3, 0);
                Frame.SetData(4, 0);
            }
        }
       
        public override string ToString()
        {
            return string.Format("Temperature sensor {0}°C",
                   Temperature
                );
        }

    }
}
