using Onixarts.Hapcan.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.UI
{
    public class MenuItem
    {
        public string DisplayName { get; set; }
        public Action<MenuItem, object> Action { get; set; }
        public void CallAction(MenuItem menuItem, object context)
        {
            Action(menuItem, context);
        }
    }
}
