using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AutoProtocol.EventMVVM
{
    /*
     * Модель представления финальных данных для отображения
     * Каждый экземпляр представляет собой один круг
     */
    class FinalDataViewModel : INotifyPropertyChanged
    {
        private int _index;
        private string _format = Application.Current.FindResource(R.LAP_INDEX_FORMAT).ToString();


        public string LapIndex
        {
            get => String.Format(_format, _index);
        }

        public ObservableCollection<CheckPoint> CheckPoints { get; set; } = new ObservableCollection<CheckPoint>();
        public ObservableCollection<Participant> Participants { get; set; } = new ObservableCollection<Participant>();
        public ObservableCollection<ObservableCollection<Time>> ParticipantTimes { get; set; } = new ObservableCollection<ObservableCollection<Time>>();

        public FinalDataViewModel(int index, ObservableCollection<CheckPoint> checkpoints, ObservableCollection<Participant> participants)
        {
            _index = index;
            CheckPoints = checkpoints;
            Participants = participants;
            foreach (Participant participant in Participants)
            {
                var time = new ObservableCollection<Time>();
                foreach (ParticipantTime participantTime in participant.ParticipantTimes)
                {
                    time.Add(participantTime.Times[_index - 1]);
                }
                ParticipantTimes.Add(time);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
