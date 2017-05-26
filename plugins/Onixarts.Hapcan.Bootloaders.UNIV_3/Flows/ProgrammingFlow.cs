using Caliburn.Micro;
using Onixarts.Hapcan.Devices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Flows
{
    public class ProgrammingFlow : Extensions.BaseFlow
    {

        public ProgrammingFlow(DeviceBase device)
            : base(device)
        {
        }

        protected Queue<MemoryBlock> ProgrammingData { get; set; }
        private MemoryBlock CurrentMemoryBlock { get; set; }
        protected byte ExtraModuleNumber { get; set; }
        protected byte ExtraGroupNumber { get; set; }

        
        bool DeviceEnteredProgrammingModeACK { get; set; }
        bool ReceivedAddressFrameMessageACK { get; set; }
        bool ReceivedDataFrameMessageACK { get; set; }
        bool ReceivedHardwareTypeResponseMessageACK { get; set; }

        public override bool HandleMessage(Hapcan.Messages.Message receivedMessage)
        {
            // check if this message is for current device (or updated ID device)
            if ( (receivedMessage.Frame.GroupNumber != Device.GroupNumber || receivedMessage.Frame.ModuleNumber != Device.ModuleNumber )
                && (ExtraGroupNumber != receivedMessage.Frame.GroupNumber || ExtraModuleNumber != receivedMessage.Frame.ModuleNumber) )
                return false;

            if (receivedMessage is Messages.EnterProgrammingModeResponse)
            {
                DeviceEnteredProgrammingModeACK = true;
                Device.IsInProgrammingMode = true;
                return true;
            }
            else if (SentMessage is Messages.AddressFrameRequestToNode && receivedMessage is Messages.AddressFrameResponseForNode)
            {
                var messageACK = receivedMessage as Messages.AddressFrameResponseForNode;
                var sentMessage = SentMessage as Messages.AddressFrameRequestToNode;

                if (messageACK.Command != sentMessage.Command)
                    return false;

                if (messageACK.Address != sentMessage.Address)
                    return false;

                ReceivedAddressFrameMessageACK = true;
                return true;
            }
            else if (SentMessage is Messages.DataFrameRequestToNode && receivedMessage is Messages.DataFrameResponseForNode)
            {
                byte index = 0;
                foreach (var byteData in CurrentMemoryBlock.Data)
                {
                    if (receivedMessage.Frame.GetData(index) != byteData)
                        return false;
                    index++;
                }
                ReceivedDataFrameMessageACK = true;
                return true;
            }
            else if (SentMessage is Messages.HardwareTypeRequestToNode && receivedMessage is Messages.HardwareTypeResponseForNode)
            {
                ReceivedHardwareTypeResponseMessageACK = true;
                Device.IsInProgrammingMode = false;
                Device.ModuleNumber = receivedMessage.Frame.ModuleNumber;
                Device.GroupNumber = receivedMessage.Frame.GroupNumber;
                return true;
            }

            return false;
        }

        public override void Run()
        {
            try
            {
                RetryCounter = 3;
                do
                {
                    EnterProgrammingMode();
                }
                while (!IsAnswerMessageReceived(() => DeviceEnteredProgrammingModeACK) && --RetryCounter > 0);

                if (RetryCounter == 0)
                    throw new TimeoutException("Device not entered in programming mode");


                //TODO: check bootloader/firmware

                // sendind data loop
                while (ProgrammingData.Count > 0)
                {
                    ReceivedAddressFrameMessageACK = false;
                    CurrentMemoryBlock = ProgrammingData.Dequeue();

                    RetryCounter = 3;
                    do
                    {
                        SendAddressFrame();
                    }
                    while (!IsAnswerMessageReceived(() => ReceivedAddressFrameMessageACK) && --RetryCounter > 0);

                    if (RetryCounter == 0)
                        throw new Exception("Device not responding to address frame or incorrect answers received");


                    ReceivedDataFrameMessageACK = false;
                    RetryCounter = 3;
                    do
                    {
                        SendDataFrame();
                    }
                    while (!IsAnswerMessageReceived(() => ReceivedDataFrameMessageACK) && --RetryCounter > 0);

                    if (RetryCounter == 0)
                        throw new Exception("Device not responding to data frame or incorrect answers received");
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

            bool moduleRestared = false;

            ExitProgrammingMode(Device.ModuleNumber, Device.GroupNumber);
            Thread.Sleep(500);

            // in case module has changed ID, ping this new module ID
            if (ExtraModuleNumber != 0 && ExtraGroupNumber != 0)
            {
                RetryCounter = 5;
                do
                {
                    SendHardwareTypeRequestToNode(ExtraModuleNumber, ExtraGroupNumber);
                }
                while (!IsAnswerMessageReceived(() => ReceivedHardwareTypeResponseMessageACK) && --RetryCounter > 0);

                if (RetryCounter > 0)
                    moduleRestared = true;
            }

            if( !moduleRestared )
            {
                RetryCounter = 5;
                do
                {
                    SendHardwareTypeRequestToNode(Device.ModuleNumber, Device.GroupNumber);
                }
                while (!IsAnswerMessageReceived(() => ReceivedHardwareTypeResponseMessageACK) && --RetryCounter > 0);

                if (RetryCounter > 0)
                    moduleRestared = true;
            }

            if(!moduleRestared)
                System.Windows.MessageBox.Show("Restarted module not responding");
        }

        void EnterProgrammingMode()
        {
            HapcanManager.Connector.Send(SentMessage = new Messages.EnterProgrammingModeRequestToNode()
            {
                RequestedModuleNumber = Device.ModuleNumber,
                RequestedGroupNumber = Device.GroupNumber
            });
        }

        void SendAddressFrame()
        {
            if (CurrentMemoryBlock == null)
                throw new ArgumentException("Missing data block");
            if (Device == null)
                throw new ArgumentException("Programming device is null");

            var msg = new Messages.AddressFrameRequestToNode();
            msg.Frame.ModuleNumber = Device.ModuleNumber;
            msg.Frame.GroupNumber = Device.GroupNumber;
            msg.Address = CurrentMemoryBlock.Address;
            msg.Command = Messages.ProgrammingCommand.Write;

            SentMessage = msg;
            HapcanManager.Connector.Send(msg);
        }

        void SendDataFrame()
        {
            if (CurrentMemoryBlock == null)
                throw new ArgumentException("Missing data block");
            if (Device == null)
                throw new ArgumentException("Programming device is null");


            var msg = new Messages.DataFrameRequestToNode();
            msg.Frame.ModuleNumber = Device.ModuleNumber;
            msg.Frame.GroupNumber = Device.GroupNumber;
            byte index = 0;
            foreach (var byteData in CurrentMemoryBlock.Data)
            {
                msg.Frame.SetData(index, byteData);
                index++;
            }

            SentMessage = msg;
            HapcanManager.Connector.Send(msg);
        }

        public void ExitProgrammingMode(byte moduleNumber, byte groupNumber)
        {
            var msg = new Messages.ExitProgrammingModeRequestToNode();
            msg.Frame.ModuleNumber = moduleNumber;
            msg.Frame.GroupNumber = groupNumber;

            HapcanManager.Connector.Send(msg);
            HapcanManager.Connector.Send(msg);
        }

        void SendHardwareTypeRequestToNode(byte moduleNumber, byte groupNumber)
        {
            var msg = new Messages.HardwareTypeRequestToNode();
            msg.RequestedModuleNumber = moduleNumber;
            msg.RequestedGroupNumber = groupNumber;

            SentMessage = msg;
            HapcanManager.Connector.Send(msg);
        }
    }
}
