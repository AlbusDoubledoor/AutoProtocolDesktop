using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace AutoProtocol.EventMVVM
{
    /*
     * Модель события
     */
    [Serializable]
    class Event : INotifyPropertyChanged
    {
        private string _name;
        private int _autoSyncDelay = 1;
        private int _manualSyncDelay = 10;
        private int _lapsCount = 1;
        private bool _isStaged = false;
        private string _htmlTemplate;
        private static readonly int EventNameMaxLength = 128;

        public HashSet<Participant> Participants { get; } = new HashSet<Participant>();
        public HashSet<CheckPoint> CheckPoints { get; } = new HashSet<CheckPoint>();
        
        public string HTMLTemplate
        {
            get => _htmlTemplate;
            set
            {
                _htmlTemplate = value;
                OnPropertyChanged();
            }
        }

        public bool IsStaged
        {
            get => _isStaged;
            set
            {
                _isStaged = value;
                OnPropertyChanged();
            }
        }

        public int LapsCount
        {
            get => _lapsCount;
            set
            {
                if (value <= 0) return;

                _lapsCount = value;
                OnPropertyChanged();
            }
        }
        public int AutoSyncDelay
        {
            get => _autoSyncDelay;
            set
            {
                if (value <= 0) return;

                _autoSyncDelay = value;
                OnPropertyChanged();
            }
        }

        public int ManualSyncDelay
        {
            get => _manualSyncDelay;
            set
            {
                if (value <= 0) return;

                _manualSyncDelay = value;
                OnPropertyChanged();
            }
        }

        public int MaxParticipant { get => Participants.Count; }
        public int MaxCheckPoint { get => CheckPoints.Count;  }

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
