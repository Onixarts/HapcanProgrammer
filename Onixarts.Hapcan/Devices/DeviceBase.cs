using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Devices
{
    //public enum DeviceType : byte
    //{
    //    Unknown = 0x00,
    //    Button = 0x01,
    //    Relay = 0x02,
    //    IRReceiver = 0x03,
    //    TemperatureSensor = 0x04,
    //    IRTransmitter = 0x05,
    //    Dimmer = 0x06,
    //    BlindController = 0x07,
    //    LEDController = 0x08,
    //    Last
    //}


    public class DeviceBase : PropertyChangedBase
    {
        //private DeviceType m_type;
        private byte _moduleNumber;
        private byte _groupNumber;
        private int m_serialNumber;
        //private Int16 _hardwareVersion;
        private Version _moduleVersion = new Version();
        private Version _bootloaderVersion = new Version();

        private string description1;
        private string description2;
        //private int _descriptionFrameIndex = 0;
        //private Int16 _firmwareVersion;


        //public DeviceType Type
        //{
        //    get { return m_type; }
        //    set
        //    {
        //        if (value == DeviceType.Unknown || value >= DeviceType.Last)
        //            throw new ArgumentOutOfRangeException("Type", "Incorrect device type");
        //        m_type = value;
        //        //RaisePropertyChanged(() => Type);
        //    }
        //}

        public byte ModuleNumber
        {
            get { return _moduleNumber; }
            set
            {
                _moduleNumber = value;
                NotifyOfPropertyChange(() => ModuleNumber);
            }
        }
        public byte GroupNumber
        {
            get { return _groupNumber; }
            set
            {
                _groupNumber = value;
                NotifyOfPropertyChange(() => GroupNumber);
            }
        }
        public int SerialNumber
        {
            get { return m_serialNumber; }
            set
            {
                m_serialNumber = value;
                NotifyOfPropertyChange(() => SerialNumber);
            }
        }
        //public Int16 HardwareVersion
        //{
        //    get { return _hardwareVersion; }
        //    set
        //    {
        //        _hardwareVersion = value;
        //        RaisePropertyChanged(() => HardwareVersion);
        //    }
        //}


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


        //{
        //    get { return _moduleVersion; }
        //    set
        //    {
        //        _moduleVersion = value;
        //        RaisePropertyChanged(() => ModuleVersion);
        //    }
        //}
        public Version BootloaderVersion
        {
            get { return _bootloaderVersion; }
            set
            {
                _bootloaderVersion = value;
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

        //public void SerialNumberFromBytes(byte id0, byte id1, byte id2, byte id3)
        //{
        //    SerialNumber = (id0 << 24) + (id1 << 16) + (id2 << 8) + id3;
        //}


        //public void UpdateHardwareInfo(HardwareTypeResponseMessage message)
        //{
        //    SerialNumber = message.SerialNumber;
        //    ModuleNumber = message.Frame.ModuleNumber;
        //    GroupNumber = message.Frame.GroupNumber;
        //    HardwareVersion = message.HardwareVersion;
        //}


        //public void UpdateFirmwareInfo(FirmwareTypeNodeResponseMessage message)
        //{
        //    ModuleVersion = new Version((int)message.HVersion, (int)message.Type, (int)message.ApplicationVersion, (int)message.FirmwareVersion);
        //    BootloaderVersion = new Version((int)message.BootloaderVersion1, (int)message.BootloaderVersion2);
        //    Type = message.Type;
        //    RaisePropertyChanged(() => ModuleFirmwareVersion);
        //}

        //public void UpdateDescription(string text)
        //{
        //    switch (_descriptionFrameIndex)
        //    {
        //        case 0: _description = text; _descriptionFrameIndex++; break;
        //        case 1: _description += text; _descriptionFrameIndex = 0; RaisePropertyChanged(() => Description); break;
        //    }
        //}

        //public string ModuleFirmwareVersion
        //{
        //    get
        //    {
        //        return String.Format("{0} {1}",
        //            ModuleVersion,
        //            Type
        //        );
        //    }
        //}

    }
}
