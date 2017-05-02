using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Devices
{
    public class DeviceBase : PropertyChangedBase
    {
        private byte moduleNumber;
        private byte groupNumber;
        private int serialNumber;
        private short hardwareType;
        private byte hardwareVersion;
        private byte firmwareVersion;
        //private Version moduleVersion = new Version();
        private Version bootloaderVersion = new Version();

        private string description1;
        private string description2;

        public DeviceBase()
        {
            description1 = "<unknown";
            description2 = ">";
        }

        public byte ModuleNumber
        {
            get { return moduleNumber; }
            set
            {
                moduleNumber = value;
                NotifyOfPropertyChange(() => ModuleNumber);
            }
        }
        public byte GroupNumber
        {
            get { return groupNumber; }
            set
            {
                groupNumber = value;
                NotifyOfPropertyChange(() => GroupNumber);
            }
        }
        public int SerialNumber
        {
            get { return serialNumber; }
            set
            {
                serialNumber = value;
                NotifyOfPropertyChange(() => SerialNumber);
            }
        }

        public short HardwareType
        {
            get { return hardwareType; }
            set
            {
                hardwareType = value;
                NotifyOfPropertyChange(() => HardwareType);
            }
        }

        public byte HardwareVersion
        {
            get { return hardwareVersion; }
            set
            {
                hardwareVersion = value;
                NotifyOfPropertyChange(() => HardwareVersion);
            }
        }


        private byte applicationType;
        public byte ApplicationType
        {
            get { return applicationType; }
            set
            {
                applicationType = value;
                NotifyOfPropertyChange(() => ApplicationType);
            }
        }


        private byte applicationVersion;
        public byte ApplicationVersion
        {
            get { return applicationVersion; }
            set
            {
                applicationVersion = value;
                NotifyOfPropertyChange(() => ApplicationVersion);
            }
        }


        public byte FirmwareVersion
        {
            get { return firmwareVersion; }
            set
            {
                firmwareVersion = value;
                NotifyOfPropertyChange(() => FirmwareVersion);
            }
        }


        public Version BootloaderVersion
        {
            get { return bootloaderVersion; }
            set
            {
                bootloaderVersion = value;
                NotifyOfPropertyChange(() => BootloaderVersion);
            }
        }
        public string Description
        {
            get { return description1 + description2; }
            set
            {
                if (description1 == null)
                    description1 = value;
                else if (description2 == null)
                    description2 = value;
                else
                {
                    description1 = value;
                    description2 = null;
                }
                NotifyOfPropertyChange(() => Description);
            }
        }


        private IDevicePlugin devicePlugin;
        public IDevicePlugin DevicePlugin
        {
            get { return devicePlugin; }
            set
            {
                devicePlugin = value;
                NotifyOfPropertyChange(() => DevicePlugin);
            }
        }

        private bool isInProgrammingMode;
        public bool IsInProgrammingMode
        {
            get { return isInProgrammingMode; }
            set
            {
                isInProgrammingMode = value;
                NotifyOfPropertyChange(() => IsInProgrammingMode);
            }
        }
    }
}
