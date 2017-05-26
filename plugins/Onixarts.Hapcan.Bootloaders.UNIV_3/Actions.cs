using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;
using System;
using System.Collections.Generic;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3
{

    public class MemoryBlock
    {
        int address;
        public int Address
        {
            get { return address; }
            set
            {
                if (value % 8 != 0)
                    throw new Exception($"Incorrect address value. Memory block Address should be multiple of 8, {value} given.");

                address = value;
            }
        }
        public byte[] Data { get; } = new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
    }

    public static class MemoryBlockExtensions
    {
        public static MemoryBlock AddNextBlock(this Queue<MemoryBlock> programmingData)
        {
            var memoryBlock = new MemoryBlock();
            if (programmingData.Count == 0)
            {
                throw new Exception("Programming data is empty, can't genereta next element");
            }
            else
            {
                memoryBlock.Address = programmingData.Peek().Address + programmingData.Count * 8;
            }

            programmingData.Enqueue(memoryBlock);
            return memoryBlock;
        }
    }

    public class Actions
    {
        public HapcanManager HapcanManager { get; set; }

        internal Flows.ProgrammingFlow ProgrammingFlow { get; set; }
        internal Flows.RestoreDefaultIDFlow RestoreDefaultIDFlow { get; set; }
        internal Flows.ChangeDescriptionFlow ChangeDescriptionFlow { get; set; }

        public void RebootAction(MenuItem menuItem, object context)
        {
            var device = context as DeviceBase;
            if (device == null)
                return;

            var msg = new Messages.RebootRequestToNode();
            msg.RequestedGroupNumber = device.GroupNumber;
            msg.RequestedModuleNumber = device.ModuleNumber;
            HapcanManager.Connector.Send(msg);
        }

        public async void UpdateIDActionAsync(DeviceBase device, byte moduleNumber, byte groupNumber)
        {
            if (moduleNumber == 0)
                throw new ArgumentException("Module number must be greater than 0");

            if (groupNumber == 0)
                throw new ArgumentException("Group number must be greater than 0");

            var programmingData = new Queue<MemoryBlock>();
            var memoryBlock = new MemoryBlock() { Address = 0xF00020 };
            memoryBlock.Data[6] = moduleNumber;
            memoryBlock.Data[7] = groupNumber; 

            programmingData.Enqueue(memoryBlock);

            ProgrammingFlow = new Flows.ProgrammingFlow(device, programmingData, moduleNumber, groupNumber);

            await ProgrammingFlow.RunAsync();
        }

        public async void UpdateDescriptionAsync(DeviceBase device, string description)
        {
            ChangeDescriptionFlow = new Flows.ChangeDescriptionFlow(device, description);
            await ChangeDescriptionFlow.RunAsync().ContinueWith((task) =>
            {
                device.Description = description.Length <= 8 ? description : description.Substring(0, 8);
                device.Description = description.Length <= 8 ? "" : description.Substring(8, description.Length - 8);

                ChangeDescriptionFlow = null;
            });
        }

        public async void RestoreDefaultIdAsync(DeviceBase device, byte moduleNumber, byte groupNumber)
        {
            RestoreDefaultIDFlow = new Flows.RestoreDefaultIDFlow(device, moduleNumber, groupNumber);
            await RestoreDefaultIDFlow.RunAsync().ContinueWith((task) =>
            {
                RestoreDefaultIDFlow = null;
            });
        }

    }
}
