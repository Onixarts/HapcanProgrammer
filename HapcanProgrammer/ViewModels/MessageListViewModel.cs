using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HapcanProgrammer.ViewModels
{
    [Export(typeof(MessageListViewModel))]
    public class MessageListViewModel
    {
        [Import]
        public BindableCollection<Onixarts.Hapcan.Messages.Message> Messages { get; set; }

        public MessageListViewModel()
        {
        }
    }
}
