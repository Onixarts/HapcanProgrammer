using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Messages
{

    public class DataFrameRequestToNode : RequestMessage
    {
        public DataFrameRequestToNode() : base((short)Module.FrameType.DataFrame)
        {
        }

        public DataFrameRequestToNode(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return $"Data frame for ({Frame.ModuleNumber}, {Frame.GroupNumber})";
        }
    }
}
