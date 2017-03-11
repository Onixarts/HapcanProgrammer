namespace Onixarts.Hapcan.Messages
{
    public class ResponseMessage : Message
    {
        public ResponseMessage(short type) : base(type)
        {
            Frame.Flags |= (byte)Frame.Flag.ResponseMessage;
        }

        public ResponseMessage(Frame frame) : base(frame)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}(response)", base.ToString());
        }
    }
}
