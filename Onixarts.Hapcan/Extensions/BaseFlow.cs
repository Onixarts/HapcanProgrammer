using Caliburn.Micro;
using Onixarts.Hapcan.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Extensions
{
    public abstract class BaseFlow
    {

        public BaseFlow(DeviceBase device)
        {
            Device = device;
            Timeout = 1000;
            RetryCounter = 3;
        }

        protected DeviceBase Device { get; set; }

        public HapcanManager HapcanManager
        {
            get
            {
                return IoC.Get<HapcanManager>();
            }
        }

        public abstract void Run();

        public virtual async Task RunAsync()
        {
            await Task.Run(()=> { Run(); });
        }

        protected Messages.Message SentMessage { get; set; }

        public abstract bool HandleMessage(Messages.Message receivedMessage);

        protected int RetryCounter { get; set; }
        protected int Timeout { get; set; }

        protected bool IsAnswerMessageReceived<T>(Expression<Func<T>> conditionProperty)
        {
            var propertyInfo = ((MemberExpression)conditionProperty.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'conditionProperty' should point to a valid Property");
            }

            //TODO: add task cancelation 
            Task t = Task.Run(() => 
            {
                //var value;// (bool)propertyInfo.GetValue(this, null);
                while (!((bool)propertyInfo.GetValue(this, null)))
                {
                    Thread.Sleep(1);
                }
                propertyInfo.SetValue(this, false, null);
                //value = false; return true; 
            });
            if (!t.Wait(1000))
                return false;

            return true;
        }
    }
}
