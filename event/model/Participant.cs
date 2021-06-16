using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoProtocol.EventMVVM
{
    [Serializable]
    class Participant : INotifyPropertyChanged
    {
        private static readonly int ParticipantNameMaxLength = 128;
        private int _id;

        public int Id
        {
            get => _id; 
            set {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (value.Length > ParticipantNameMaxLength) return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public List<long> Times { get; } = new List<long>();

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
