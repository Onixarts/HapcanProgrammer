using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;

namespace HapcanProgrammer.ViewModels
{
    [Export(typeof(DeviceListViewModel))]
    public class DeviceListViewModel
    {

        [Import]
        public BindableCollection<DeviceBase> Devices { get; set; }

        //[ImportMany]
        public BindableCollection<MenuItem> DeviceMenuOptions { get; set; }

        public DeviceListViewModel()
        {
            if( Execute.InDesignMode)
            {
                Devices = new BindableCollection<DeviceBase>()
                {
                    new DeviceBase() { Description="dev1", GroupNumber=1, ModuleNumber=2,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x02, ApplicationVersion=0,  },
                    new DeviceBase() { Description="device2", GroupNumber=1, ModuleNumber=3,SerialNumber=123234,HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x01, ApplicationVersion=0 },
                    new DeviceBase() { Description="device3", GroupNumber=1, ModuleNumber=4,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x02, ApplicationVersion=0 },
                    new DeviceBase() { Description="device444", GroupNumber=1, ModuleNumber=5,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x03, ApplicationVersion=0 },
                    new DeviceBase() { Description="buttons5", GroupNumber=2, ModuleNumber=1,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x02, ApplicationVersion=0 },
                    new DeviceBase() { Description="device6", GroupNumber=2, ModuleNumber=2,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x03, ApplicationVersion=0 },
                    new DeviceBase() { Description="testdevice", GroupNumber=3, ModuleNumber=3,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3,ApplicationType=0x04, ApplicationVersion=0 },
                    new DeviceBase() { Description="device8", GroupNumber=3, ModuleNumber=5,SerialNumber=123234, HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x02, ApplicationVersion=0 },
                    new DeviceBase() { Description="device9", GroupNumber=4, ModuleNumber=4,SerialNumber=123234,HardwareType=0x3000, HardwareVersion=3, ApplicationType=0x05, ApplicationVersion=0 },
                };

                DeviceMenuOptions = new BindableCollection<MenuItem>()
                {
                    new MenuItem() {DisplayName = "opcja dynamiczna 1", Action = Test1 },
                    new MenuItem() {DisplayName = "opcja dynamiczna 2",  Action = ()=> { System.Windows.MessageBox.Show("test2"); } },
                    new MenuItem() {DisplayName = "opcja dynamiczna 3",  Action = ()=> { System.Windows.MessageBox.Show("test3"); } },
                };

            }


        }

        public void Test1()
        {
            System.Windows.MessageBox.Show("test1");
        }


        public DeviceBase SelectedDevice { get { return null; } set { } }

    }
}
