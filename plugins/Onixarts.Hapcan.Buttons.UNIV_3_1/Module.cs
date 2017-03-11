﻿using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Buttons.UNIV_3_1
{
    [Export(typeof(IHapcanDevice))]
    public class Module : IHapcanDevice
    {
        public string HardwareVersionName { get { return "UNIV 3"; } }
        public short HardwareType { get { return 0x3000; } }
        public byte HardwareVersion { get { return 0x03; } }


        public enum FrameType
        {
            Empty = 0,
            ButtonMessage = 0x301,
            TemperatureMessage = 0x304,
        }

        public enum FrameDataType
        {
            TemperatureFrame = 0x11,
            ThermostatFrame = 0x12,
            TemperatureControllerFrame = 0x13,
            SensorErrorFrame = 0xF0,
        }

        public Message GetMessage(Frame frame)
        {
            switch ((FrameType)frame.Type)
            {
                case FrameType.Empty: break;
                case FrameType.ButtonMessage: return new Messages.TemperatureMessage(frame);
                case FrameType.TemperatureMessage:
                    {
                        switch((FrameDataType)frame.GetData(2))
                        {
                            case FrameDataType.TemperatureFrame: return new Messages.TemperatureMessage(frame);
                            case FrameDataType.ThermostatFrame: return null;
                            case FrameDataType.TemperatureControllerFrame: return null;
                            case FrameDataType.SensorErrorFrame: return null;
                        }
                    }
                    break;
            }
            return null;
        }

        public bool HandleMessage(Message msg)
        {
            return false;
        }

    }
}