using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using System.ComponentModel.Composition;
using System.Threading;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3
{
    [Export(typeof(IHapcanDevice))]
    public class Module : IHapcanDevice
    {
        public string HardwareVersionName { get { return ""; } }
        public short HardwareType { get { return 0; } }
        public byte HardwareVersion { get { return 0; } }


        [Import]
        HapcanManager HapcanManager { get; set; }

        public enum FrameType
        {
            Empty = 0,
            ExitAllFromBootloaderProgrammingMode = 0x010,
            ExitOneNodeFromBootloaderProgrammingMode = 0x020,
            AddressFrame = 0x030,
            DataFrame = 0x040,

            ErrorFrame = 0x0F0,

            EnterProgrammingMode = 0x100,
            RebootRequestToGroup = 0x101,
            RebootRequestToNode = 0x102,
            HardwareType = 0x103,
            HardwareTypeMessageToNode = 0x104,
            FirmwareTypeMessageToGroup = 0x105,
            FirmwareTypeMessageToNode = 0x106,
            SetDefaultNodeAndGroupRequestToNode = 0x107,

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
                case FrameType.ExitAllFromBootloaderProgrammingMode: break;
                case FrameType.ExitOneNodeFromBootloaderProgrammingMode: break;
                case FrameType.AddressFrame: break;
                case FrameType.DataFrame: break;
                
                case FrameType.ErrorFrame: break;
                
                case FrameType.EnterProgrammingMode: break;
                case FrameType.RebootRequestToGroup: break;
                case FrameType.RebootRequestToNode: break;
                case FrameType.HardwareType: if (frame.IsResponse) return new Messages.HardwareTypeResponse(frame); else return new Messages.HardwareTypeRequestToGroup(frame);
                case FrameType.HardwareTypeMessageToNode: break;
                case FrameType.FirmwareTypeMessageToGroup: if (frame.IsResponse) return new Messages.FirmwareTypeResponse(frame); else return new Messages.FirmwareTypeRequestToGroup(frame);
                case FrameType.FirmwareTypeMessageToNode: if (frame.IsResponse) return new Messages.FirmwareTypeResponse(frame); else return new Messages.FirmwareTypeRequestToNode(frame);
                case FrameType.SetDefaultNodeAndGroupRequestToNode: break;
                
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


        public bool HandleMessage(Message msg)
        {
            if(msg is Messages.HardwareTypeResponse)
            {
                var message = msg as Messages.HardwareTypeResponse;
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
                return true;
            }

            if (msg is Messages.FirmwareTypeResponse)
            {
                var message = msg as Messages.FirmwareTypeResponse;
                var device = HapcanManager.Devices.Select(d => d).Where(d => d.GroupNumber == message.Frame.GroupNumber && d.ModuleNumber == message.Frame.ModuleNumber).FirstOrDefault();
                if (device != null)
                {

                    device.ApplicationType = message.ApplicationType;
                    device.ApplicationVersion = message.ApplicationVersion;
                    device.BootloaderVersion = new Version(message.BootloaderVersion1, message.BootloaderVersion2);
                }
                else
                {
                    // nie ma takiego urządzenia na liście
                    // TODO: dodać?
                }
                return true;
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
                return true;
            }

            return false;
        }

        [Export(typeof(Func<int,Task>))]
        public async Task ScanBusForDevices(int asd)
        {
            const int rangeTo = 10;
            
            await Task.Run(() =>
            {
                //TODO: exitAllfromProgrammingMode
                Task.Delay(1000);
            });

            await Task.Run(() =>
            {
                for (int i = 1; i < rangeTo; i++)
                {
                    var msg = new Messages.HardwareTypeRequestToGroup();
                    msg.RequestedGroupNumber = (byte)i;
                    HapcanManager.Connector.Send(msg);
                    //SimpleIoc.Default.GetInstance<EthernetConnector>().Send(msg);
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



            //foreach (var device in DevicesList)
            //{
            //    var msg = new Hapcan.Message.Request.FirmwareTypeNodeRequestMessage();
            //    msg.RequestedGroupNumber = device.GroupNumber;
            //    msg.RequestedModuleNumber = device.ModuleNumber;
            //    SimpleIoc.Default.GetInstance<EthernetConnector>().Send(msg);
            //    Thread.Sleep(100);
            //}

            ////Thread.Sleep(500);
            //foreach (var device in DevicesList)
            //{
            //    var msg = new Hapcan.Message.Request.DescriptionNodeRequestMessage();
            //    msg.RequestedGroupNumber = device.GroupNumber;
            //    msg.RequestedModuleNumber = device.ModuleNumber;
            //    SimpleIoc.Default.GetInstance<EthernetConnector>().Send(msg);
            //    Thread.Sleep(100);
            //}
            //return "asd";
        }
    }
}
