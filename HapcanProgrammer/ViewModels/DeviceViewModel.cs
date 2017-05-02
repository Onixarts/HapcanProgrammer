using Caliburn.Micro;
using Onixarts.Hapcan;
using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HapcanProgrammer.ViewModels
{
    [Export(typeof(DeviceViewModel))]
    public class DeviceViewModel : Screen
        , IHandle<DeviceBase>
    {
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public DeviceViewModel(IEventAggregator events)
        {
            this.events = events;
            events.Subscribe(this);
        }        

        [Import]
        HapcanManager HapcanManager { get; set; }

        public DeviceBase CurrentDevice { get; set; }

        // TODO: change to event class?
        public void Handle(DeviceBase device)
        {
            CurrentDevice = device;

            // TODO: try find overwritten view for specific plugin instead default bootloaderPlugin
            var bootloaderPlugin = HapcanManager.FindBootloaderPlugin(device.HardwareType, device.HardwareVersion);
            if (bootloaderPlugin == null)
                return;

            SettingsTabViewModel = bootloaderPlugin.SettingsTabViewModel;
            SettingsTabViewModel.Device = device;
            SettingsTabViewModel.BootloaderPlugin = bootloaderPlugin;
            
        }


        private DeviceTabViewModel settingsTabViewModel;
        public DeviceTabViewModel SettingsTabViewModel
        {
            get { return settingsTabViewModel; }
            set
            {
                settingsTabViewModel = value;
                NotifyOfPropertyChange(() => SettingsTabViewModel);
            }
        }

    }
}
