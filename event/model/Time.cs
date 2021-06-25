using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoProtocol.EventMVVM
{
    /*
     * Модель метки времени
     */
    [Serializable]
    class Time : INotifyPropertyChanged
    {
        private int _pointCount = 1;
        public static readonly string TIME_PATTERN = "HH:mm:ss.fff";

        private long _rawValue = 0;
        public long RawValue
        {
            get => _rawValue;
            set
            {
                _rawValue = (_rawValue + value) / _pointCount++;
                StringValue = new DateTime(value*10000L).ToString(TIME_PATTERN);
                OnPropertyChanged();
            }
        }

        private string _stringValue = "";
        public string StringValue
        {
            get => _stringValue;
            set
            {
                try
                {
                    _stringValue = value;
                    RawValue = DateTime.Parse(TIME_PATTERN).Ticks;
                }
                catch
                {
                    // Ignore
                }
                OnPropertyChanged();
            }
        }

        public Time()
        {
            RawValue = 0;
            _pointCount = 1;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
