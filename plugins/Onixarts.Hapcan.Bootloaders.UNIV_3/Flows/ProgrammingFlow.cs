using Caliburn.Micro;
using Onixarts.Hapcan.Devices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Flows
{
    public class ProgrammingFlow
    {
        public HapcanManager HapcanManager
        {
            get
            {
                return IoC.Get<HapcanManager>();
            }
        }

        public ProgrammingFlow()
        {
        }

        private Queue<MemoryBlock> ProgrammingData { get; set; }
        private MemoryBlock CurrentMemoryBlock { get; set; }
        byte ExtraModuleNumber { get; set; }
        byte ExtraGroupNumber { get; set; }

        DeviceBase CurrentDevice { get; set; }
        bool DeviceEnteredProgrammingMode { get; set; }
        bool ReceivedAddressFrameACKMessage { get; set; }
        bool ReceivedDataFrameACKMessage { get; set; }
        bool ReceivedHardwareTypeResponseMessage { get; set; }

        Hapcan.Messages.Message SentMessage { get; set; }

        public void MessageReceive(Hapcan.Messages.Message receivedMessage)
        {
            // check if this message is for current device (or updated ID device)
            if ( (receivedMessage.Frame.GroupNumber != CurrentDevice.GroupNumber || receivedMessage.Frame.ModuleNumber != CurrentDevice.ModuleNumber )
                && (ExtraGroupNumber != receivedMessage.Frame.GroupNumber || ExtraModuleNumber != receivedMessage.Frame.ModuleNumber) )
                return;

            if (receivedMessage is Messages.EnterProgrammingModeResponse)
                DeviceEnteredProgrammingMode = true;
            else if ( SentMessage is Messages.AddressFrameRequestToNode && receivedMessage is Messages.AddressFrameResponseForNode)
            {
                var messageACK = receivedMessage as Messages.AddressFrameResponseForNode;
                var sentMessage = SentMessage as Messages.AddressFrameRequestToNode;

                if (messageACK.Command != sentMessage.Command)
                    return;

                if (messageACK.Address != sentMessage.Address)
                    return;

                ReceivedAddressFrameACKMessage = true;
            }
            else if (SentMessage is Messages.DataFrameRequestToNode && receivedMessage is Messages.DataFrameResponseForNode)
            {
                byte index = 0;
                foreach (var byteData in CurrentMemoryBlock.Data)
                {
                    if (receivedMessage.Frame.GetData(index) != byteData)
                        return;
                    index++;
                }
                ReceivedDataFrameACKMessage = true;
            }
            else if (SentMessage is Messages.HardwareTypeRequestToNode && receivedMessage is Messages.HardwareTypeResponseForNode)
            {
                ReceivedHardwareTypeResponseMessage = true;
            }
        }

        public async Task StartAsync(DeviceBase device, Queue<MemoryBlock> programmingData, byte extraModuleNumber = 0, byte extraGroupNumber = 0)
        {
            await Task.Run(() =>
            {
                try
                {
                    CurrentDevice = device;
                    ProgrammingData = programmingData;
                    ExtraModuleNumber = extraModuleNumber;
                    ExtraGroupNumber = extraGroupNumber;

                    // TODO: repeat sending 3 times
                    EnterProgrammingMode();
                    WaitForEnterToProgrammingMode();

                    //TODO: check bootloader/firmware

                    // sendind data loop
                    while (ProgrammingData.Count > 0)
                    {
                        ReceivedAddressFrameACKMessage = false;
                        CurrentMemoryBlock = ProgrammingData.Dequeue();
                        int retryCounter = 3;

                        do
                        {
                            SendAddressFrame();
                            retryCounter--;
                        } while (!AddressFrameACKReceived() && retryCounter > 0);

                        if (retryCounter == 0)
                            throw new Exception("Device not responding to address frame or bad answers");

                        ReceivedDataFrameACKMessage = false;
                        retryCounter = 3;

                        do
                        {
                            SendDataFrame();
                            retryCounter--;
                        } while (!DataFrameACKReceived() && retryCounter > 0);

                        if (retryCounter == 0)
                            throw new Exception("Device not responding to data frame or bad answers");
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                }

                bool moduleRestared = false;

                ExitProgrammingMode(CurrentDevice.ModuleNumber, CurrentDevice.GroupNumber);
                Thread.Sleep(1000);

                // in case module has changed ID, ping this new module ID
                if (ExtraModuleNumber != 0 && ExtraGroupNumber != 0)
                {
                    moduleRestared = PingDevice(ExtraModuleNumber, ExtraGroupNumber);
                }

                if( !moduleRestared )
                    moduleRestared = PingDevice(CurrentDevice.ModuleNumber, CurrentDevice.GroupNumber);
            });
        }

        void EnterProgrammingMode()
        {
            var msg = new Messages.EnterProgrammingModeRequestToNode();
            msg.RequestedModuleNumber = CurrentDevice.ModuleNumber;
            msg.RequestedGroupNumber = CurrentDevice.GroupNumber;

            HapcanManager.Connector.Send(msg);
        }

        void WaitForEnterToProgrammingMode()
        {
            Task t = Task.Run(() => { while (!DeviceEnteredProgrammingMode) { Thread.Sleep(1); } });
            if (!t.Wait(5000))
                throw new TimeoutException("Device not entered into programming mode in 5s");
        }

        bool AddressFrameACKReceived()
        {
            //TODO: add task cancelation 
            Task t = Task.Run(() => { while (!ReceivedAddressFrameACKMessage) { Thread.Sleep(1); } ReceivedAddressFrameACKMessage = false; return true; });
            if (!t.Wait(1000))
                return false;

            return true;
        }

        bool DataFrameACKReceived()
        {
            //TODO: add task cancelation 
            Task t = Task.Run(() => { while (!ReceivedDataFrameACKMessage) { Thread.Sleep(1); } ReceivedDataFrameACKMessage = false; return true; });
            if (!t.Wait(1000))
                return false;

            return true;
        }

        bool HardwareTypeResponseReceived()
        {
            //TODO: add task cancelation 
            Task t = Task.Run(() => { while (!ReceivedHardwareTypeResponseMessage) { Thread.Sleep(1); } return true; });
            if (!t.Wait(1000))
                return false;

            return true;
        }

        void SendAddressFrame()
        {
            if (CurrentMemoryBlock == null)
                throw new ArgumentException("Missing data block");
            if (CurrentDevice == null)
                throw new ArgumentException("Programming device is null");

            var msg = new Messages.AddressFrameRequestToNode();
            msg.Frame.ModuleNumber = CurrentDevice.ModuleNumber;
            msg.Frame.GroupNumber = CurrentDevice.GroupNumber;
            msg.Address = CurrentMemoryBlock.Address;
            msg.Command = Messages.ProgrammingCommand.Write;

            SentMessage = msg;
            HapcanManager.Connector.Send(msg);
        }

        void SendDataFrame()
        {
            if (CurrentMemoryBlock == null)
                throw new ArgumentException("Missing data block");
            if (CurrentDevice == null)
                throw new ArgumentException("Programming device is null");


            var msg = new Messages.DataFrameRequestToNode();
            msg.Frame.ModuleNumber = CurrentDevice.ModuleNumber;
            msg.Frame.GroupNumber = CurrentDevice.GroupNumber;
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

        bool PingDevice(byte moduleNumber, byte groupNumber)
        {
            int retryCounter = 5;
            ReceivedHardwareTypeResponseMessage = false;

            do
            {
                SendHardwareTypeRequestToNode(moduleNumber, groupNumber);
                retryCounter--;
            } while (!HardwareTypeResponseReceived() && retryCounter > 0);

            if (retryCounter == 0)
                return false;

            return true;
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
