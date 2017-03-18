using Onixarts.Hapcan.Devices;
using Onixarts.Hapcan.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Bootloaders.UNIV_3
{
    public class Actions
    {
        public HapcanManager HapcanManager { get; set; }

        public void RebootAction(MenuItem menuItem, object context)
        {
            var device = context as DeviceBase;
            if (device == null)
                return;

            var msg = new Messages.RebootRequestToNode();
            msg.RequestedGroupNumber = device.GroupNumber;
            msg.RequestedModuleNumber = device.ModuleNumber;
            HapcanManager.Connector.Send(msg);
        }

    }
}
