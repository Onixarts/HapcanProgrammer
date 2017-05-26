using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;
using System.Threading;
using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3
{
    [Export(typeof(IDevicePlugin))]
    public class Module : IDevicePlugin
    {
        public string HardwareVersionName { get { return "UNIV 3"; } }
        public short HardwareType { get { return 0x3000; } }
        public byte HardwareVersion { get { return 0x03; } }
        public byte ApplicationType { get { return 0; } }   // Bootloader plugin must have application type of 0


        private HapcanManager hapcanManager;
        [Import]
        HapcanManager HapcanManager { get { return hapcanManager; } set { hapcanManager = value; Actions.HapcanManager = value; } }

        internal Actions Actions { get; } = new Actions();

        public Module()
        {
        }

        public enum FrameType
        {
            Empty = 0,
            ExitAllFromBootloaderProgrammingMode = 0x010,
            ExitOneNodeFromBootloaderProgrammingMode = 0x020,
            AddressFrame = 0x030,
            DataFrame = 0x040,

            ErrorFrame = 0x0F0,

            EnterProgrammingMode = 0x100,
            RebootMessageToGroup = 0x101,
            RebootMessageToNode = 0x102,
            HardwareTypeMessageToGroup = 0x103,
            HardwareTypeMessageToNode = 0x104,
            FirmwareTypeMessageToGroup = 0x105,
            FirmwareTypeMessageToNode = 0x106,
            SetDefaultNodeAndGroupMessageToNode = 0x107,

            StatusRequestToGroup = 0x108,
            StatusRequestToNode = 0x109,

            SupplyVoltageRequestToGroup = 0x10B,
            SupplyVoltageRequestToNode = 0x10C,
            DescriptionMessageToGroup = 0x10D,
            DescriptionMessageToNode = 0x10E,
            DeviceIDRequestToGroup = 0x10F,

            DeviceIDRequestToNode = 0x111,
            UptimeRequestToGroup = 0x112,
            UptimeRequestToNode = 0x113,
            HealthCheckRequestToGroup = 0x114,
            HealthCheckRequestToNode = 0x115
        }

        
        public Message GetMessage(Frame frame)
        {
            switch((FrameType)frame.Type)
            {
                case FrameType.Empty: break;
                case FrameType.ExitAllFromBootloaderProgrammingMode: return new Messages.ExitProgrammingModeRequestToAll(frame);
                case FrameType.ExitOneNodeFromBootloaderProgrammingMode: return new Messages.ExitProgrammingModeRequestToNode(frame);
                case FrameType.AddressFrame: if (frame.IsResponse) return new Messages.AddressFrameResponseForNode(frame); else return new Messages.AddressFrameRequestToNode(frame);
                case FrameType.DataFrame: if (frame.IsResponse) return new Messages.DataFrameResponseForNode(frame); else return new Messages.DataFrameRequestToNode(frame);

                case FrameType.ErrorFrame: break;
                
                case FrameType.EnterProgrammingMode: if (frame.IsResponse) return new Messages.EnterProgrammingModeResponse(frame); else return new Messages.EnterProgrammingModeRequestToNode(frame);
                case FrameType.RebootMessageToGroup: break;
                case FrameType.RebootMessageToNode: return new Messages.RebootRequestToNode(frame);
                case FrameType.HardwareTypeMessageToGroup: if (frame.IsResponse) return new Messages.HardwareTypeResponseForGroup(frame); else return new Messages.HardwareTypeRequestToGroup(frame);
                case FrameType.HardwareTypeMessageToNode: if (frame.IsResponse) return new Messages.HardwareTypeResponseForNode(frame); else return new Messages.HardwareTypeRequestToNode(frame);
                case FrameType.FirmwareTypeMessageToGroup: if (frame.IsResponse) return new Messages.FirmwareTypeResponse(frame); else return new Messages.FirmwareTypeRequestToGroup(frame);
                case FrameType.FirmwareTypeMessageToNode: if (frame.IsResponse) return new Messages.FirmwareTypeResponse(frame); else return new Messages.FirmwareTypeRequestToNode(frame);
                case FrameType.SetDefaultNodeAndGroupMessageToNode: if (frame.IsResponse) return new Messages.SetDefaultNodeAndGroupResponse(frame); else return new Messages.SetDefaultNodeAndGroupRequestToNode(frame);

                case FrameType.StatusRequestToGroup: break;
                case FrameType.StatusRequestToNode: break;
                
                case FrameType.SupplyVoltageRequestToGroup: break;
                case FrameType.SupplyVoltageRequestToNode: break;
                case FrameType.DescriptionMessageToGroup: if (frame.IsResponse) return new Messages.DescriptionResponse(frame); else return new Messages.DescriptionRequestToGroup(frame);
                case FrameType.DescriptionMessageToNode: break;
                case FrameType.DeviceIDRequestToGroup: break;
                
                case FrameType.DeviceIDRequestToNode: break;
                case FrameType.UptimeRequestToGroup: break;
                case FrameType.UptimeRequestToNode: break;
                case FrameType.HealthCheckRequestToGroup: break;
                case FrameType.HealthCheckRequestToNode: break;
            }
            return null;
        }

        //var matchedPlugin = HapcanDevicePlugins.Select(d => d).Where(d => d.HardwareType == HardwareType && d.HardwareVersion == HardwareVersion).FirstOrDefault();

        public void HandleMessage(Message msg)
        {
            if(msg is Messages.HardwareTypeResponseForGroup)
            {
                var message = msg as Messages.HardwareTypeResponseForGroup;
                var device = HapcanManager.Devices.Select(d => d).Where(d => d.SerialNumber == message.SerialNumber).FirstOrDefault();
                if (device == null)
                {
                    var newDevice = new Hapcan.Devices.DeviceBase()
                    {
                        SerialNumber = message.SerialNumber,
                        GroupNumber = message.Frame.GroupNumber,
                        ModuleNumber = message.Frame.ModuleNumber,
                        //TODO reszta danych modułu
                    };

                    
                    HapcanManager.Devices.Add(newDevice);
                }
                else
                {
                    device.IsInProgrammingMode = false;
                }
                return;
            }

            if (msg is Messages.FirmwareTypeResponse)
            {
                var message = msg as Messages.FirmwareTypeResponse;
                var device = HapcanManager.Devices.Select(d => d).Where(d => d.GroupNumber == message.Frame.GroupNumber && d.ModuleNumber == message.Frame.ModuleNumber).FirstOrDefault();
                if (device != null)
                {
                    device.HardwareType = message.HardwareType;
                    device.HardwareVersion = message.HardwareVersion;
                    device.ApplicationType = message.ApplicationType;
                    device.ApplicationVersion = message.ApplicationVersion;
                    device.FirmwareVersion = message.FirmwareVersion;
                    device.BootloaderVersion = new Version(message.BootloaderVersion1, message.BootloaderVersion2);
                }
                else
                {
                    // nie ma takiego urządzenia na liście
                    // TODO: dodać?
                }
                return;
            }

            if( msg is Messages.DescriptionResponse)
            {
                var message = msg as Messages.DescriptionResponse;
                var device = HapcanManager.Devices.Where(d => d.GroupNumber == message.Frame.GroupNumber && d.ModuleNumber == message.Frame.ModuleNumber).Select(d => d).FirstOrDefault();
                if (device != null)
                {
                    device.Description = message.Text;
                }
                else
                {
                    // nie ma takiego urządzenia na liście
                    // TODO: dodać?
                }
                return;
            }


            if (Actions.ProgrammingFlow != null ? Actions.ProgrammingFlow.HandleMessage(msg) : false)
                return;

            if (Actions.ChangeDescriptionFlow != null ? Actions.ChangeDescriptionFlow.HandleMessage(msg) : false)
                return;

            if (Actions.ChangeIDFlow != null ? Actions.ChangeIDFlow.HandleMessage(msg) : false)
                return;

            if (Actions.RestoreDefaultIDFlow != null ? Actions.RestoreDefaultIDFlow.HandleMessage(msg) : false)
                return;
        }

        [Export(typeof(Func<int,Task>))]
        public async Task ScanBusForDevices(int asd)
        {
            const int rangeTo = 10;
            
            await Task.Run(() =>
            {
                var msg = new Messages.ExitProgrammingModeRequestToAll();
                HapcanManager.Connector.Send(msg);
                Task.Delay(1000);
            });

            await Task.Run(() =>
            {
                for (int i = 200; i < 210; i++)
                {
                    var msg = new Messages.HardwareTypeRequestToGroup();
                    msg.RequestedGroupNumber = (byte)i;
                    HapcanManager.Connector.Send(msg);
                    Thread.Sleep(20);
                }
            });

            var groups = HapcanManager.Devices.Select(d => d.GroupNumber).Distinct().OrderBy(d => d).ToList();

            await Task.Run(() =>
            {
                foreach (var group in groups)
                {
                    var msg = new Messages.FirmwareTypeRequestToGroup();
                    msg.RequestedGroupNumber = group;
                    HapcanManager.Connector.Send(msg);

                    Thread.Sleep(20);
                }
            });

            await Task.Run(() =>
            {
                foreach (var group in groups)
                {
                    var msg = new Messages.DescriptionRequestToGroup();
                    msg.RequestedGroupNumber = group;
                    HapcanManager.Connector.Send(msg);

                    Thread.Sleep(20);
                }
            });


            // power request 0x10b

            // search for plugin for devices
            await Task.Run(() =>
            {
                //Thread.Sleep(1000);
                foreach(var device in HapcanManager.Devices)
                {
                    var plugin = HapcanManager.DevicePlugins.Where(p => p.HardwareType == device.HardwareType
                                                                   && p.HardwareVersion == device.HardwareVersion
                                                                   && p.ApplicationType == device.ApplicationType).FirstOrDefault();
                    if(plugin != null)
                    {
                        device.DevicePlugin = plugin;
                    }
                }
            });
        }

        public IEnumerable<MenuItem> DevicesListContextMenuItems
        {
            get
            {
                return new[] { new MenuItem() { DisplayName = "Reboot device", Action = Actions.RebootAction }};
            }
        }

        DeviceTabViewModel deviceTabViewModel;
        public DeviceTabViewModel SettingsTabViewModel { get { if (deviceTabViewModel == null) deviceTabViewModel = new ViewModels.DeviceSettingsTabViewModel(); return deviceTabViewModel; } }
    }
}
