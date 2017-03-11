using Onixarts.Hapcan.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onixarts.Hapcan.Gates.UNIV_3_102.Messages
{
    class RTCMessage : Message
    {
        public RTCMessage() : base((short) Module.FrameType.RTCMessage)
        {
        }

        public RTCMessage(Frame frame) : base(frame)
        {
        }

        protected byte BCD2Value(byte value)
        {
            return (byte)((((value & 0xF0) >> 4) * 10) + (value & 0x0F));
        }

        protected byte Value2BCD(byte value)
        {
            return (byte)(((value / 10) << 4) + (value % 10));
        }

        public byte Year
        {
            get
            {
                return BCD2Value(Frame.GetData(1));
            }
            set
            {
                if (value > 0x99)
                    throw new ArgumentOutOfRangeException("Year", "Invalid year (0-99)");
                Frame.SetData(1, Value2BCD(value));
            }
        }

        public byte Month
        {
            get
            {
                return BCD2Value(Frame.GetData(2));
            }
            set
            {
                if (value > 0x12 || value == 0x00)
                    throw new ArgumentOutOfRangeException("Month", "Invalid month (1-12)");
                Frame.SetData(2, Value2BCD(value));
            }
        }

        public byte Date
        {
            get
            {
                return BCD2Value(Frame.GetData(3));
            }
            set
            {
                if (value > 0x31 || value == 0x00)
                    throw new ArgumentOutOfRangeException("Date", "Invalid date (1-31)");
                Frame.SetData(3, Value2BCD(value));
            }
        }

        public byte Day
        {
            get
            {
                return Frame.GetData(4);
            }
            set
            {
                if (value > 0x07)
                    throw new ArgumentOutOfRangeException("Day", "Invalid day number (0-7)");
                Frame.SetData(4, value);
            }
        }

        public byte Hour
        {
            get
            {
                return BCD2Value(Frame.GetData(5));
            }
            set
            {
                if (value > 0x23)
                    throw new ArgumentOutOfRangeException("Hour", "Invalid hour (0-23)");
                Frame.SetData(5, Value2BCD(value));
            }
        }

        public byte Minute
        {
            get
            {
                return BCD2Value(Frame.GetData(6));
            }
            set
            {
                if (value > 0x59)
                    throw new ArgumentOutOfRangeException("Minute", "Invalid minute (0-59)");
                Frame.SetData(6, Value2BCD(value));
            }
        }

        public byte Second
        {
            get
            {
                return BCD2Value(Frame.GetData(7));
            }
            set
            {
                if (value > 0x59)
                    throw new ArgumentOutOfRangeException("Second", "Invalid second (0-59)");
                Frame.SetData(7, Value2BCD(value));
            }
        }

        public override string ToString()
        {
            var culture = new System.Globalization.CultureInfo("en-US");
            var day = culture.DateTimeFormat.GetDayName((DayOfWeek)(Day % 7));

            return string.Format("RTC: 20{0}-{1:00}-{2:00} {3} {4:00}:{5:00}:{6:00}",
                Year,
                Month,
                Date,
                day,
                Hour,
                Minute,
                Second
                );
        }

    }
}
