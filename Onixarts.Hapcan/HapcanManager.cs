using Caliburn.Micro;
using Hapcan = Onixarts.Hapcan;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;
using Onixarts.Hapcan.Devices;

namespace Onixarts.Hapcan
{
    [Export(typeof(HapcanManager))]
    public class HapcanManager 
        : PropertyChangedBase
        , IHandle<Communication.ReceivedEvent>
        , IHandle<Communication.SentEvent>
        
    {
        private readonly IEventAggregator events;

        [Import]
        public Communication.EthernetConnector Connector { get; set; }

        [Export(typeof(BindableCollection<Hapcan.Devices.DeviceBase>))]
        public BindableCollection<Hapcan.Devices.DeviceBase> Devices { get; private set; }

        [Export(typeof(BindableCollection<Hapcan.Messages.Message>))]
        public BindableCollection<Hapcan.Messages.Message> Messages { get; private set; }

        [ImportMany]
        public IEnumerable<IDevicePlugin> DevicePlugins { get; set; }


        public IDevicePlugin FindBootloaderPlugin(short hardwareType, byte hardwareVersion)
        {
            return DevicePlugins.Where(p => p.HardwareType == hardwareType
                   && p.HardwareVersion == hardwareVersion
                   && p.ApplicationType == 0).FirstOrDefault();
            
        }

        //private DeviceBase currentDevice;
        //public DeviceBase CurrentDevice
        //{
        //    get { return currentDevice; }
        //    set
        //    {
        //        currentDevice = value;
        //        NotifyOfPropertyChange(() => CurrentDevice);
        //    }
        //}


        [ImportingConstructor]
        public HapcanManager(IEventAggregator events)
        {
            this.events = events;
            events.Subscribe(this);

            Devices = new BindableCollection<Onixarts.Hapcan.Devices.DeviceBase>();
            Messages = new BindableCollection<Onixarts.Hapcan.Messages.Message>();
        }

        // Connect to the hapcan ethernet module
        public void Connect()
        {
            if( Connector!= null)
            {
                Connector.Connect();
            }
        }

        public void Handle(Communication.ReceivedEvent e)
        {
            HandleMessage(e.Frame);
        }

        public void Handle(Communication.SentEvent e)
        {
            HandleMessage(e.Frame);
        }

        private void HandleMessage(Frame frame)
        {
            Messages.Message msg = null;

            var sourceDevice = Devices.Where(d => d.GroupNumber == frame.GroupNumber && d.ModuleNumber == frame.ModuleNumber).FirstOrDefault();

            IDevicePlugin devicePlugin = sourceDevice?.DevicePlugin;
            msg = devicePlugin?.GetMessage(frame);

            // device plugin don't recognize message? maybe other plugin would
            if (msg == null)
            {
                foreach (var plugin in DevicePlugins)
                {
                    msg = plugin.GetMessage(frame);
                    if (msg != null)
                    {
                        devicePlugin = plugin;
                        break;
                    }
                }
            }

            // still no luck? just create default message
            if (msg == null)
                msg = new Messages.Message(frame);

            Messages.Insert(0, msg);

            if (devicePlugin != null)
                devicePlugin.HandleMessage(msg);
        }

        // TODO: move to plugin?
        [Import] 
        public Func<int,Task> ScanBusForDevices { get; set; }
    }
}
