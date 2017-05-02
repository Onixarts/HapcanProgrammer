using Caliburn.Micro;
using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3.ViewModels
{
    public class DeviceSettingsTabViewModel : DeviceTabViewModel
    {

        public DeviceSettingsTabViewModel()
        {
        }

        DeviceBase device;
        public override DeviceBase Device
        {
            get
            {
                return device;
            }

            set
            {
                device = value;
                ModuleNumber = Device.ModuleNumber;
                GroupNumber = Device.GroupNumber;
                Description = device.Description;
                NotifyOfPropertyChange(() => Device);
            }
        }

        private byte moduleNumber;
        public byte ModuleNumber
        {
            get { return moduleNumber; }
            set
            {
                moduleNumber = value;
                NotifyOfPropertyChange(() => ModuleNumber);
            }
        }

        private byte groupNumber;
        public byte GroupNumber
        {
            get { return groupNumber; }
            set
            {
                groupNumber = value;
                NotifyOfPropertyChange(() => GroupNumber);
            }
        }


        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public void UpdateId()
        {
            var module = BootloaderPlugin as Module;
            module.Actions.UpdateIDActionAsync(Device, ModuleNumber, GroupNumber);
        }
    }
}
