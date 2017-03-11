using Caliburn.Micro;
using Hapcan = Onixarts.Hapcan;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Messages;

namespace Onixarts.Hapcan
{
    [Export(typeof(HapcanManager))]
    public class HapcanManager 
        : IHandle<Communication.ReceivedEvent>
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
        public IEnumerable<IHapcanDevice> HapcanDevicePlugins { get; set; }

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
            Messages.Message msg = null;

            // TODO: jeśli znane jest urządzenie z którego pochodzi ramka to zwracamy się do niego czy potrafi ją wyświetlić
            // jesli nie to pytamy czy ktokolwiek potrafi

            IHapcanDevice hapcanDevicePlugin = null;

            foreach ( var devicePlugin in HapcanDevicePlugins)
            {
                msg = devicePlugin.GetMessage(e.Frame);
                if (msg != null)
                {
                    hapcanDevicePlugin = devicePlugin;
                    break;
                }
            }

            if (msg == null)
                msg = new Messages.Message(e.Frame);

            // TODO: jeśli ten plugin nie moze obsłużyć to sprawdzić czy któryś inny umie
            if (hapcanDevicePlugin != null)
                hapcanDevicePlugin.HandleMessage(msg);


            //TODO usuwanie starych ramek
            Messages.Insert(0, msg);
        }

        public void Handle(Communication.SentEvent e)
        {
            Messages.Message msg = null;

            // parsowanie ramki za pomocą pluginów
            foreach (var devicePlugin in HapcanDevicePlugins)
            {
                msg = devicePlugin.GetMessage(e.Frame);
                if (msg != null)
                    break;
            }

            if (msg == null)
                msg = new Messages.Message(e.Frame);
            //TODO usuwanie starych ramek
            Messages.Insert(0, msg);
        }

        // TODO: move to plugin?
        [Import] 
        public Func<int,Task> ScanBusForDevices { get; set; }
    }
}
