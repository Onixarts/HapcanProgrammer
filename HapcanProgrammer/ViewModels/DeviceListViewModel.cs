using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onixarts.Hapcan.Devices;

namespace HapcanProgrammer.ViewModels
{
    [Export(typeof(DeviceListViewModel))]
    public class DeviceListViewModel
    {

        [Import]
        public BindableCollection<DeviceBase> Devices { get; set; }

        public DeviceListViewModel()
        {

        }

        public DeviceBase SelectedDevice { get { return null; } set { } }

    }
}
