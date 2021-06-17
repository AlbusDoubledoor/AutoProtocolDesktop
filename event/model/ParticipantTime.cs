using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoProtocol.EventMVVM
{
    [Serializable]
    class ParticipantTime : INotifyPropertyChanged
    {
        private CheckPoint _checkPoint;
        public CheckPoint CheckPoint
        {
            get => _checkPoint;
            set
            {
                _checkPoint = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Time> Times { get; } = new ObservableCollection<Time>();

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
