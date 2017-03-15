using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan.Devices
{
    public interface IDevicePlugin
    {
        Message GetMessage(Frame frame);
        string HardwareVersionName { get; }
        short HardwareType { get; }
        byte HardwareVersion { get; }
        byte ApplicationType { get; }
        bool HandleMessage(Message msg);
    }
}
