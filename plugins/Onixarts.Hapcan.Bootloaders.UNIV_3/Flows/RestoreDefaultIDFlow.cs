using System;
using Onixarts.Hapcan.Messages;
using Onixarts.Hapcan.Devices;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Flows
{
    class RestoreDefaultIDFlow : Extensions.BaseFlow
    {

        public RestoreDefaultIDFlow(DeviceBase device, byte moduleNumber, byte groupNumber)
            : base(device)
        {
            DefaultModuleNumber = moduleNumber;
            DefaultGroupNumber = groupNumber;
        }

        protected byte DefaultModuleNumber { get; set; }
        protected byte DefaultGroupNumber { get; set; }

        public override bool HandleMessage(Message receivedMessage)
        {
            if (receivedMessage.Frame.GroupNumber != DefaultGroupNumber || receivedMessage.Frame.ModuleNumber != DefaultModuleNumber)
                return false;

            if( SentMessage is Messages.SetDefaultNodeAndGroupRequestToNode && receivedMessage is Messages.SetDefaultNodeAndGroupResponse)
            {
                ReceivedSetDefaultNodeAndGroupRequestACK = true;
                Device.ModuleNumber = receivedMessage.Frame.ModuleNumber;
                Device.GroupNumber = receivedMessage.Frame.GroupNumber;
                return true;
            }

            return false;
        }

        bool ReceivedSetDefaultNodeAndGroupRequestACK { get; set; }

        public override void Run()
        {
            try
            {
                RetryCounter = 3;
                do
                {
                    SendDefaultNumberAndGroupToNode();
                    RetryCounter--;
                }
                while (!IsAnswerMessageReceived(()=> ReceivedSetDefaultNodeAndGroupRequestACK) && RetryCounter > 0);

                if (RetryCounter == 0)
                    throw new TimeoutException("Device not responding to restore default ID message");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        void SendDefaultNumberAndGroupToNode()
        {
            // TODO: cache module and group value, so incoming message won't influence request values
            HapcanManager.Connector.Send(SentMessage = new Messages.SetDefaultNodeAndGroupRequestToNode()
            {
                RequestedModuleNumber = Device.ModuleNumber,
                RequestedGroupNumber = Device.GroupNumber
            });
        }
    }
}
