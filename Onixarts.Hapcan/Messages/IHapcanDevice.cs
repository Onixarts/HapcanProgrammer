using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: przenieść do innego namespaca
namespace Onixarts.Hapcan.Messages
{
    public interface IHapcanDevice
    {
        Message GetMessage(Frame frame);
        string HardwareVersionName { get; }
        short HardwareType { get; }
        byte HardwareVersion { get; }
        bool HandleMessage(Message msg);
    }
}
