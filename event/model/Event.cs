using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace AutoProtocol.EventMVVM
{
    [Serializable]
    class Event : INotifyPropertyChanged
    {
        private string _name;
        private int _autoSyncDelay = 1;
        private int _manualSyncDelay = 10;
        private static readonly int EventNameMaxLength = 128;

        public HashSet<Participant> Participants { get; } = new HashSet<Participant>();
        public HashSet<CheckPoint> CheckPoints { get; } = new HashSet<CheckPoint>();
        
        public int AutoSyncDelay
        {
            get => _autoSyncDelay;
            set
            {
                _autoSyncDelay = value;
                OnPropertyChanged();
            }
        }

        public int ManualSyncDelay
        {
            get => _manualSyncDelay;
            set
            {
                _manualSyncDelay = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value.Length > EventNameMaxLength) return;

                _name = value;
                OnPropertyChanged();
            }
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
