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

    public class Actions
    {
        public HapcanManager HapcanManager { get; set; }

        internal Flows.ProgrammingFlow ProgrammingFlow { get; set; }

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
            ProgrammingFlow = new Flows.ProgrammingFlow();

            var programmingData = new Queue<MemoryBlock>();
            var memoryBlock = new MemoryBlock() { Address = 0xF00020 };
            memoryBlock.Data[6] = moduleNumber;
            memoryBlock.Data[7] = groupNumber; 

            programmingData.Enqueue(memoryBlock);

            await ProgrammingFlow.StartAsync(device, programmingData, moduleNumber, groupNumber);
        }

        public async void RestoreDefaultIdAsync(DeviceBase device)
        {
            //TODO: AAAA osobne flowy porobić i usunąc klase Actions. Zobaczyć czy da się RebotAction jakoś przenieść
            var msg = new Messages.SetDefaultNodeAndGroupRequestToNode();
            msg.RequestedModuleNumber = device.ModuleNumber;
            msg.RequestedGroupNumber = device.GroupNumber;
            HapcanManager.Connector.Send(msg);

            await new Flows.ProgrammingFlow().PingDeviceAsync(device, (byte)(device.SerialNumber >> 8), (byte)device.SerialNumber);
        }

    }
}
