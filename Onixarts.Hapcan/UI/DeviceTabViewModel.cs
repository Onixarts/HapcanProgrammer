using Caliburn.Micro;
using Onixarts.Hapcan.Devices;

namespace Onixarts.Hapcan.UI
{
    public abstract class DeviceTabViewModel: Screen
    {
        public abstract DeviceBase Device { get; set; }
        public IDevicePlugin BootloaderPlugin { get; set; }
    }
}
