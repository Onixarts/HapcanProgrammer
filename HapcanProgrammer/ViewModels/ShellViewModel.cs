using Caliburn.Micro;
using Hapcan = Onixarts.Hapcan;
using System.ComponentModel.Composition;
using System.Windows;

namespace HapcanProgrammer.ViewModels
{
    internal enum ExpandedView
    {
        Devices = 1,
        Module,
        Messages
    }

    public enum MessageListViewType
    {
        Compact,
        Grid
    }

    public enum DeviceListViewType
    {
        Compact
    }

    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
        private readonly IWindowManager windowManager;

        [Import]
        Onixarts.Hapcan.HapcanManager HapcanManager { get; set; }

        [Import]
        public MessageListViewModel MessageListViewModel { get; set; }
        
        [Import]
        public DeviceListViewModel DeviceListViewModel { get; set; }

        public ShellViewModel()
        {
            if (!Execute.InDesignMode)
            {
            }
            FillTestData();

            SetupLayout();
        }

        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            //FillTestData();

            SetupLayout();
        }



        public void FillTestData()
        {
            //Devices = new BindableCollection<Hapcan.Devices.DeviceBase>{
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1111, ApplicationType = 1 , ApplicationVersion = 0, GroupNumber = 1, ModuleNumber = 1},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 2222, ApplicationType = 2 , ApplicationVersion = 0, GroupNumber = 1, ModuleNumber = 2},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1333, ApplicationType = 3 , ApplicationVersion = 3, GroupNumber = 2, ModuleNumber = 1},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1444, ApplicationType = 4 , ApplicationVersion = 0, GroupNumber = 2, ModuleNumber = 2},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1555, ApplicationType = 1 , ApplicationVersion = 2, GroupNumber = 3, ModuleNumber = 1},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1666, ApplicationType = 1 , ApplicationVersion = 0, GroupNumber = 3, ModuleNumber = 2},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1777, ApplicationType = 2 , ApplicationVersion = 1, GroupNumber = 4, ModuleNumber = 3},
            //        new Hapcan.Devices.DeviceBase { SerialNumber = 1888, ApplicationType = 10, ApplicationVersion = 0, GroupNumber = 5, ModuleNumber = 4 }
            //        };

            //Messages = new BindableCollection<Hapcan.Messages.Message>
            //        {
            //            new Hapcan.Messages.Message(0x02),
            //            new Hapcan.Messages.Message(0x03),
            //            new Hapcan.Messages.Message(0x02)
            //            //new Hapcan.Messages.Message(0x04),
            //            //new Hapcan.Messages.Message(0x02),
            //            //new Hapcan.Messages.Message(0x03),
            //            //new Hapcan.Messages.Message(0x02),
            //            //new Hapcan.Messages.Message(0x04),
            //            //new Hapcan.Messages.Message(0x02),
            //            //new Hapcan.Messages.Message(0x03),
            //            //new Hapcan.Messages.Message(0x02),
            //            //new Hapcan.Messages.Message(0x04),
            //            //new Hapcan.Messages.Message(0x03)
            //        };
        }
        
        public void buttonMouseEnter()
        {
           // System.Windows.MessageBox.Show("test");
        }

        public void About()
        {
            this.windowManager.ShowDialog(new AboutViewModel());
        }

        protected override void OnActivate()
        {

        }

        protected override void OnInitialize()
        {
            // TODO: sprawdziæ czy parametry po³¹czenia s¹ uzupe³nione, jeœli nie to wyœwietliæ okno parametrów

            HapcanManager.Connect();
        }

        public async void ScanBusForDevices()
        {
            await HapcanManager.ScanBusForDevices(2);
        }


        private MessageListViewType messageViewType;
        public MessageListViewType MessageListViewType
        {
            get { return messageViewType; }
            set
            {
                messageViewType = value;
                NotifyOfPropertyChange(() => MessageListViewType);
            }
        }

        public void ChangeMessageView(string viewName)
        {
            switch(viewName)
            {
                case "Compact": MessageListViewType = MessageListViewType.Compact; break;
                case "Grid": MessageListViewType = MessageListViewType.Grid; break;
            }
        }


        private DeviceListViewType deviceListViewType;
        public DeviceListViewType DeviceListViewType
        {
            get { return deviceListViewType; }
            set
            {
                deviceListViewType = value;
                NotifyOfPropertyChange(() => DeviceListViewType);
            }
        }

        #region ViewSizing
        private GridLength devicesColumnWidth;
        public GridLength DevicesColumnWidth
        {
            get { return devicesColumnWidth; }
            set
            {
                devicesColumnWidth = value;
                NotifyOfPropertyChange(() => DevicesColumnWidth);
            }
        }

        private GridLength messagesColumnWidth;
        public GridLength MessagesColumnWidth
        {
            get { return messagesColumnWidth; }
            set
            {
                messagesColumnWidth = value;
                NotifyOfPropertyChange(() => MessagesColumnWidth);
            }
        }

        private GridLength moduleColumnWidth;
        public GridLength ModuleColumnWidth
        {
            get { return moduleColumnWidth; }
            set
            {
                moduleColumnWidth = value;
                NotifyOfPropertyChange(() => ModuleColumnWidth);
            }
        }

        private ExpandedView expandedView = ExpandedView.Messages;

        public void ExpandDevicesView()
        {
            expandedView = ExpandedView.Devices;
            SetupLayout();
        }

        public void ExpandModuleView()
        {
            expandedView = ExpandedView.Module;
            SetupLayout();
        }

        public void ExpandMessagesView()
        {
            expandedView = ExpandedView.Messages;
            SetupLayout();
        }

        private void SetupLayout()
        {
            switch (expandedView)
            {
                case ExpandedView.Devices:
                    DevicesColumnWidth = new GridLength(9, GridUnitType.Star);
                    ModuleColumnWidth = new GridLength(1, GridUnitType.Star);
                    MessagesColumnWidth = new GridLength(2, GridUnitType.Star);
                    break;
                case ExpandedView.Module:
                    DevicesColumnWidth = new GridLength(2, GridUnitType.Star);
                    ModuleColumnWidth = new GridLength(9, GridUnitType.Star);
                    MessagesColumnWidth = new GridLength(2, GridUnitType.Star);
                    break;
                case ExpandedView.Messages:
                    DevicesColumnWidth = new GridLength(2, GridUnitType.Star);
                    ModuleColumnWidth = new GridLength(1, GridUnitType.Star);
                    MessagesColumnWidth = new GridLength(9, GridUnitType.Star);
                    break;
            }
            MessageListViewType = MessageListViewType.Compact;
            DeviceListViewType = DeviceListViewType.Compact;
        }
        #endregion

    }
}