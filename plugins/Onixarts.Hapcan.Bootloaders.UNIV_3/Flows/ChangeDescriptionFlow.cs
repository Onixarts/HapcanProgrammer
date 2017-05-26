using System;
using Onixarts.Hapcan.Messages;
using Onixarts.Hapcan.Devices;
using System.Text;
using System.Collections.Generic;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.Flows
{
    class ChangeDescriptionFlow : ProgrammingFlow
    {
        public ChangeDescriptionFlow(DeviceBase device, string description)
            : base(device)
        {
            if (description.Length > 16)
                description = description.Substring(0, 16);

            var descriptionWin1250 = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(1250), Encoding.Unicode.GetBytes(description));

            ProgrammingData = new Queue<MemoryBlock>();
            var memoryBlock = new MemoryBlock() { Address = 0xF00030 };

            for (byte i = 0; i < 8; i++)
            {
                if (i < description.Length)
                    memoryBlock.Data[i] = descriptionWin1250[i];
                else
                    memoryBlock.Data[i] = 0;
            }
            ProgrammingData.Enqueue(memoryBlock);

            memoryBlock = ProgrammingData.AddNextBlock();
            for (byte i = 0; i < 8; i++)
            {
                if (i + 8 < description.Length)
                    memoryBlock.Data[i] = descriptionWin1250[i + 8];
                else
                    memoryBlock.Data[i] = 0;
            }
        }
    }
}
