using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;
using Onixarts.Hapcan;

namespace HapcanProgrammer.ViewModels
{
    [Export(typeof(DeviceListViewModel))]
    public class DeviceListViewModel : Screen
    {

        private readonly IEventAggregator events;

        [Import]
        public BindableCollection<DeviceBase> Devices { get; set; }

        DeviceBase selectedDevice;
        public DeviceBase SelectedDevice
        {
            get
            {
                return selectedDevice;
            }
            set
            {
                if (selectedDevice != value)
                {
                    selectedDevice = value;
                    CreateContextMenuForSelectedItem();
                    events.PublishOnUIThread(selectedDevice);
                }
                NotifyOfPropertyChange(() => SelectedDevice);
            }
        }

        public BindableCollection<MenuItem> DeviceContextMenuItems{ get; set; }

        [Import]
        HapcanManager HapcanManager { get; set; }

        [ImportingConstructor]
        public DeviceListViewModel(IEventAggregator events)
        {
            this.events = events;

            DeviceContextMenuItems = new BindableCollection<MenuItem>();
        }

        public DeviceListViewModel()
        {
            if (Execute.InDesignMode)
            {
                Devices = new BindableCollection<DeviceBase>()
                {
                    new DeviceBase() { Description="dev1", GroupNumber=1, ModuleNumber=2,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x02, ApplicationVersion=0,  },
                    new DeviceBase() { Description="device2", GroupNumber=1, ModuleNumber=3,SerialNumber=123234,HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x01, ApplicationVersion=0 },
                    new DeviceBase() { Description="device3", GroupNumber=1, ModuleNumber=4,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x02, ApplicationVersion=0 },
                    new DeviceBase() { Description="device444", GroupNumber=1, ModuleNumber=5,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x03, ApplicationVersion=0, IsInProgrammingMode=true },
                    new DeviceBase() { Description="buttons5", GroupNumber=2, ModuleNumber=1,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x02, ApplicationVersion=0 },
                    new DeviceBase() { Description="device6", GroupNumber=2, ModuleNumber=2,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x03, ApplicationVersion=0 },
                    new DeviceBase() { Description="testdevice", GroupNumber=3, ModuleNumber=3,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x04, ApplicationVersion=0 },
                    new DeviceBase() { Description="device8", GroupNumber=3, ModuleNumber=5,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x02, ApplicationVersion=0 },
                    new DeviceBase() { Description="device9", GroupNumber=4, ModuleNumber=4,SerialNumber=123234,HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x05, ApplicationVersion=0 },
                };
            }
        }

        private void CreateContextMenuForSelectedItem()
        {
            if (selectedDevice != null)
            {
                DeviceContextMenuItems.Clear();
                var bootloaderPlugin = HapcanManager.FindBootloaderPlugin(selectedDevice.HardwareType, selectedDevice.HardwareVersion);
                DeviceContextMenuItems.AddRange(bootloaderPlugin?.DevicesListContextMenuItems);
            }

        }

    }
}
