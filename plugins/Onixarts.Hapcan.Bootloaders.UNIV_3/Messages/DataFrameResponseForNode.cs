using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{
    public class DataFrameResponseForNode : ResponseMessage
    {
        public DataFrameResponseForNode() : base((short)Module.FrameType.DataFrame)
        {
        }

        public DataFrameResponseForNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Data frame ACK from ({Frame.ModuleNumber}, {Frame.GroupNumber})";
        }
    }
}
