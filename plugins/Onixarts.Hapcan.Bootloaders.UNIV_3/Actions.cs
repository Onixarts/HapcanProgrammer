using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            if (description.Length > 16)
                description = description.Substring(0, 16);

            var descriptionWin1250 = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(1250), Encoding.Unicode.GetBytes(description));

            var programmingData = new Queue<MemoryBlock>();
            var memoryBlock = new MemoryBlock() { Address = 0xF00030 };

            for (byte i = 0; i < 8; i++)
            {
                if (i < description.Length)
                    memoryBlock.Data[i] = descriptionWin1250[i];
                else
                    memoryBlock.Data[i] = 0;
            }
            programmingData.Enqueue(memoryBlock);

            memoryBlock = programmingData.AddNextBlock();
            for (byte i = 0; i < 8; i++)
            {
                if (i + 8 < description.Length)
                    memoryBlock.Data[i] = descriptionWin1250[i + 8];
                else
                    memoryBlock.Data[i] = 0;
            }

            ProgrammingFlow = new Flows.ProgrammingFlow(device, programmingData);
            await ProgrammingFlow.RunAsync().ContinueWith((task) =>
            {
                // TODO: make this on flow?
                device.Description = description.Length <=8 ? description : description.Substring(0,8);
                device.Description = description.Length <=8 ? "" : description.Substring(8, description.Length-8);
            });
        }

        public async void RestoreDefaultIdAsync(DeviceBase device, byte moduleNumber, byte groupNumber)
        {
            //TODO: AAAA osobne flowy porobić i usunąc klase Actions. Zobaczyć czy da się RebotAction jakoś przenieść
            //var msg = new Messages.SetDefaultNodeAndGroupRequestToNode();
            //msg.RequestedModuleNumber = device.ModuleNumber;
            //msg.RequestedGroupNumber = device.GroupNumber;
            //HapcanManager.Connector.Send(msg);
            RestoreDefaultIDFlow = new Flows.RestoreDefaultIDFlow(device, moduleNumber, groupNumber);
            await RestoreDefaultIDFlow.RunAsync();

        //    await new Flows.ProgrammingFlow().PingDeviceAsync(device, (byte)(device.SerialNumber >> 8), (byte)device.SerialNumber);
        }

    }
}
