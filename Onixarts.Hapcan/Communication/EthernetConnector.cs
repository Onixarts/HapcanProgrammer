using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hapcan = Onixarts.Hapcan;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Onixarts.Hapcan.Communication
{
    [Export(typeof(EthernetConnector))]
    public class EthernetConnector
    {
        private readonly IEventAggregator events;

        private Socket clientSocket;
        private Thread recivingThread;

        [ImportingConstructor]
        public EthernetConnector(IEventAggregator events)
        {
            this.events = events;

            IP = "192.168.0.100";   //default hapcan ethernet interface address
            Port = 1001;
        }

        public bool Enabled { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public void Connect()
        {
            try
            {
                IPAddress[] IPs = Dns.GetHostAddresses(IP);
                IPEndPoint endpoint = new IPEndPoint(IPs[0], Port);

                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(endpoint);

                events.PublishOnUIThread(new ConnectedEvent());

                recivingThread = new Thread(SocketReceive);
                recivingThread.IsBackground = true;
                Enabled = true;
                recivingThread.Start();
            }
            catch (Exception ex)
            {
                //TODO: info
            }
        }

        public void Disconnect()
        {
            try
            {
                Enabled = false;
                if (clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    events.PublishOnUIThread(new DisconnectedEvent());
                }
            }
            catch (Exception ex)
            {
                //TODO: info
            }
        }

        private void FlushReceivingBuffer()
        {
            byte[] rxBytes = new byte[15];

            try
            {
                while (clientSocket.Available > 0)
                {
                    clientSocket.Receive(rxBytes, 15, SocketFlags.None);
                    Thread.Sleep(1);
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void SocketReceive(object obj)
        {
            Thread.Sleep(1000);
            byte[] rxBytes = new byte[15];

            FlushReceivingBuffer();

            while (Enabled)
            {
                try
                {
                    while (clientSocket.Available > 14)
                    {
                        //System.Diagnostics.Trace.WriteLine("odebrano pakiet");
                        clientSocket.Receive(rxBytes, 15, SocketFlags.None);

                        var frame = new Hapcan.Messages.Frame(rxBytes);

                        if (frame.Start == Hapcan.Messages.Frame.ControlByte.StartFrame)
                            events.PublishOnUIThread(new ReceivedEvent(frame));
                    }
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    //TODO: info
                }

            }
        }

        public void Send(Hapcan.Messages.Message message)
        {
            try
            {
                message.Frame.CalculateControlSum();
                clientSocket.Send(message.Frame.RawData);
                events.PublishOnUIThread(new SentEvent(message.Frame));
            }
            catch (Exception ex)
            {
            }
        }
    }


    public class EthernetConnectorEvent
    {
        public Hapcan.Messages.Frame Frame { get; set; }
        public EthernetConnectorEvent()
        {
        }
        public EthernetConnectorEvent(Hapcan.Messages.Frame frame)
        {
            Frame = frame;
        }
    }

    public class ConnectedEvent : EthernetConnectorEvent
    {
    }

    public class DisconnectedEvent : EthernetConnectorEvent
    {
    }

    public class ReceivedEvent : EthernetConnectorEvent
    {
        public ReceivedEvent(Hapcan.Messages.Frame frame) : base(frame)
        {
        }
    }

    public class SentEvent : EthernetConnectorEvent
    {
        public SentEvent(Hapcan.Messages.Frame frame) : base(frame)
        {
        }
    }
}

