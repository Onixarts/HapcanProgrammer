using System;
using Onixarts.Hapcan.Messages;
using Onixarts.Hapcan.Devices;
using System.Text;
using System.Collections.Generic;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Flows
{
    class ChangeIDFlow : ProgrammingFlow
    {
        public ChangeIDFlow(DeviceBase device, byte moduleNumber, byte groupNumber)
            : base(device)
        {
            if (moduleNumber == 0)
                throw new ArgumentException("Module number must be greater than 0");

            if (groupNumber == 0)
                throw new ArgumentException("Group number must be greater than 0");

            ProgrammingData = new Queue<MemoryBlock>();
            var memoryBlock = new MemoryBlock() { Address = 0xF00020 };
            memoryBlock.Data[6] = moduleNumber;
            memoryBlock.Data[7] = groupNumber;

            ProgrammingData.Enqueue(memoryBlock);
            ExtraModuleNumber = moduleNumber;
            ExtraGroupNumber = groupNumber;
        }
    }
}
