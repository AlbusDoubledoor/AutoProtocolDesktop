using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.ObjectModel;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;

namespace AutoProtocol.EventMVVM
{
    class EventViewModel : INotifyPropertyChanged, IErrorActionConsumer, IFileExportProvidable, IFileImportProvidable, IConfirmProvidable
    {
        private Event _event;
        private Participant _selectedParticipant;
        private CheckPoint _selectedCheckPoint;
        private HashSet<CheckPoint> _checkPoints = new HashSet<CheckPoint>();
        private HashSet<Participant> _participants = new HashSet<Participant>();
        public ObservableCollection<Participant> Participants { get; set; }
        public ObservableCollection<CheckPoint> CheckPoints { get; set; }

        public Action<string> ErrorAction { get; set; }
        public void DoErrorAction(string errorMessage)
        {
            if (ErrorAction != null)
            {
                ErrorAction.Invoke(errorMessage);
            }
        }

        public Func<string, string> ProvideFileExport { get; set; }
        public string RequestFileExport(string filter)
        {
            if (ProvideFileExport != null)
            {
                return ProvideFileExport.Invoke(filter);
            }
            return "";
        }

        public Func<string, string> ProvideFileImport { get; set; }
        public string RequestFileImport(string filter)
        {
            if (ProvideFileImport != null)
            {
                return ProvideFileImport.Invoke(filter);
            }
            return "";
        }

        public Func<string, bool> ProvideConfirm { get; set; }
        public bool RequestConfirm(string confirmation)
        {
            if (ProvideConfirm != null)
            {
                return ProvideConfirm.Invoke(confirmation);
            }
            return false;
        }

        public Participant SelectedParticipant
        {
            get => _selectedParticipant;
            set
            {
                _selectedParticipant = value;
                OnPropertyChanged();
            }
        }

        public CheckPoint SelectedCheckPoint
        {
            get => _selectedCheckPoint;
            set
            {
                _selectedCheckPoint = value;
                OnPropertyChanged();
            }
        }

        public Event Event
        {
            get { return _event; }
            set
            {
                _event = value;
                Participants = new ObservableCollection<Participant>(_event.Participants);
                Participants.CollectionChanged += (sender, e) =>
                {
                    _event.Participants.Clear();
                    foreach (Participant participant in Participants)
                    {
                        participant.Id = Participants.IndexOf(participant) + 1;
                        _event.Participants.Add(participant);
                    }
                };
                if (Participants.Count > 0)
                {
                    SelectedParticipant = Participants[0];
                }

                CheckPoints = new ObservableCollection<CheckPoint>(_event.CheckPoints);
                CheckPoints.CollectionChanged += (sender, e) =>
                {
                    _event.CheckPoints.Clear();
                    foreach (CheckPoint checkPoint in CheckPoints)
                    {
                        checkPoint.Id = CheckPoints.IndexOf(checkPoint) + 1;
                        _event.CheckPoints.Add(checkPoint);
                    }
                };
                if (CheckPoints.Count > 0)
                {
                    SelectedCheckPoint = CheckPoints[0];
                }

                OnPropertyChanged(String.Empty);
            }
        }

        public string Name
        {
            get => _event.Name;
            set
            {
                _event.Name = value;
                OnPropertyChanged();
            }
        }

        public int AutoSyncDelay
        {
            get => _event.AutoSyncDelay;
            set
            {
                _event.AutoSyncDelay = value;
                OnPropertyChanged();
            }
        }

        public int ManualSyncDelay
        {
            get => _event.ManualSyncDelay;
            set
            {
                _event.ManualSyncDelay = value;
                OnPropertyChanged();
            }
        }

        public int LapsCount
        {
            get => _event.LapsCount;
            set
            {
                _event.LapsCount = value;
                OnPropertyChanged();
            }
        }

        public EventViewModel()
        {
            this.Event = new Event { Name = "New Event" };
            AddParticipantCommand.Execute("New Participant");
            AddCheckPointCommand.Execute("New CheckPoint");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private RelayCommand _addParticipantCommand;
        public RelayCommand AddParticipantCommand
        {
            get
            {
                return _addParticipantCommand ??
                  (_addParticipantCommand = new RelayCommand(obj =>
                  {
                      var newParticipant = new Participant { Name = obj?.ToString() ?? "" };
                      Participants.Add(newParticipant);
                      SelectedParticipant = newParticipant;
                  }));
            }
        }

        private RelayCommand _addCheckPointCommand;
        public RelayCommand AddCheckPointCommand
        {
            get
            {
                return _addCheckPointCommand ??
                  (_addCheckPointCommand = new RelayCommand(obj =>
                  {
                      var newCheckPoint = new CheckPoint { Name = obj?.ToString() ?? "" };
                      CheckPoints.Add(newCheckPoint);
                      SelectedCheckPoint = newCheckPoint;
                  }));
            }
        }

        private RelayCommand _deleteParticipantCommand;
        public RelayCommand DeleteParticipantCommand
        {
            get
            {
                return _deleteParticipantCommand ??
                    (_deleteParticipantCommand = new RelayCommand(obj =>
                    {
                        if (SelectedParticipant != null)
                        {
                            if (Participants.Count == 1)
                            {
                                string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__LAST_PARTICIPANT_DELETION).ToString();
                                DoErrorAction(localizedErrorMsg);
                                return;
                            }
                            int removeIndex = Participants.IndexOf(SelectedParticipant);
                            Participants.Remove(SelectedParticipant);

                            SelectedParticipant = removeIndex > (Participants.Count - 1) ? Participants[Participants.Count - 1] : Participants[removeIndex];
                        }
                    }));
            }
        }

        private RelayCommand _deleteCheckPointCommand;
        public RelayCommand DeleteCheckPointCommand
        {
            get
            {
                return _deleteCheckPointCommand ??
                    (_deleteCheckPointCommand = new RelayCommand(obj =>
                    {
                        if (SelectedCheckPoint != null)
                        {
                            if (CheckPoints.Count == 1)
                            {
                                string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__LAST_CHECK_POINT_DELETION).ToString();
                                DoErrorAction(localizedErrorMsg);
                                return;
                            }
                            int removeIndex = CheckPoints.IndexOf(SelectedCheckPoint);
                            CheckPoints.Remove(SelectedCheckPoint);

                            SelectedCheckPoint = removeIndex > (CheckPoints.Count - 1) ? CheckPoints[CheckPoints.Count - 1] : CheckPoints[removeIndex];
                        }
                    }));
            }
        }

        private RelayCommand _exportConfigurationCommand;
        public RelayCommand ExportConfigurationCommand
        {
            get
            {
                return _exportConfigurationCommand ??
                    (_exportConfigurationCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            string fileExtension = EventConfiguration.FILE_EXTENSION;
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__EVENT).ToString();
                            string filter = $"{localizedFilter} (*{fileExtension})|*{fileExtension}";
                            string fileName = RequestFileExport(filter);

                            if (fileName.Length == 0) return;

                            using (var eventConfiguration = new EventConfiguration(_event))
                            {
                                eventConfiguration.Write(fileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__EXPORT_CONFIGURATION).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }
                    ));
            }
        }

        private RelayCommand _importConfigurationCommand;
        public RelayCommand ImportConfigurationCommand
        {
            get
            {
                return _importConfigurationCommand ??
                    (_importConfigurationCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            string fileExtension = EventConfiguration.FILE_EXTENSION;
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__EVENT).ToString();
                            string filter = $"{localizedFilter} (*{fileExtension})|*{fileExtension}";
                            string fileName = RequestFileImport(filter);

                            if (fileName.Length == 0) return;

                            using (var eventConfiguration = new EventConfiguration(fileName))
                            {
                                this.Event = eventConfiguration.Event;
                            }
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__EXPORT_CONFIGURATION).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }
                    ));
            }
        }

        private bool _isConfigurationApplied = false;
        public bool IsConfigurationApplied
        {
            get => _isConfigurationApplied;
            set
            {
                _isConfigurationApplied = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FinalDataViewModel> FinalData { get; set; } = new ObservableCollection<FinalDataViewModel>();
        private FinalDataViewModel _selectedLap;
        public FinalDataViewModel SelectedLap
        {
            get => _selectedLap;
            set
            {
                _selectedLap = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _applyConfigurationCommand;
        public RelayCommand ApplyConfigurationCommand
        {
            get
            {
                return _applyConfigurationCommand ??
                    (_applyConfigurationCommand = new RelayCommand(obj =>
                    {
                    IsConfigurationApplied = RequestConfirm("После применения, конфигурацию нельзя будет изменить. Продолжить?");
                        foreach (Participant participant in Participants)
                        {
                            foreach (CheckPoint checkPoint in CheckPoints)
                            {

                                ParticipantTime newParticipantTime = new ParticipantTime();
                                newParticipantTime.CheckPoint = checkPoint;
                                for (int i = 0; i < LapsCount; ++i)
                                {
                                    newParticipantTime.Times.Add(new Time());
                                }
                                participant.ParticipantTimes.Add(newParticipantTime);

                            }
                        }
                        for (int i = 1; i <= LapsCount; ++i)
                        {
                            FinalData.Add(new FinalDataViewModel(i, "Lap {0}", CheckPoints, Participants));
                            SelectedLap = FinalData[0];
                        }
                    }));
            }
        }

        private RelayCommand _loadDataCommand;
        public RelayCommand LoadDataCommand
        {
            get
            {
                return _loadDataCommand ??
                    (_loadDataCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            string fileExtension = ".apd";
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__EVENT).ToString();
                            string filter = $"{localizedFilter} (*{fileExtension})|*{fileExtension}";
                            string fileName = RequestFileImport(filter);

                            if (fileName.Length == 0) return;

                            new EventDataLoader(this.Event, fileName);
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__EXPORT_CONFIGURATION).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        private RelayCommand _exportProtocolCommand;
        public RelayCommand ExportProtocolCommand
        {
            get
            {
                return _exportProtocolCommand ??
                    (_exportProtocolCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

                            WebKitConverterSettings settings = new WebKitConverterSettings();

                            //Set WebKit path
                            settings.WebKitPath = "qtbin/";

                            //Assign WebKit settings to HTML converter
                            htmlConverter.ConverterSettings = settings;

                            //Convert URL to PDF
                            PdfDocument document = htmlConverter.Convert(HTMLTemplate,"my.x");

                            //Save and close the PDF document 
                            string fileName = RequestFileExport("Protocol file (*.pdf)|*.pdf");
                            document.Save(File.Create(fileName));

                            document.Close(true);

                            
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__EXPORT_CONFIGURATION).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

        }


        private string _htmlTemplate;
        public string HTMLTemplate
        {
            get => _htmlTemplate;
            set
            {
                _htmlTemplate = value;
                OnPropertyChanged();
            }
        }

        public bool Save(String savePath)
        {
            try
            {
                var fileStream = File.Create(savePath);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(fileStream, Event);
                fileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                DoErrorAction(ex.Message);
                return false;
            }
        }

        public bool Load(String loadPath)
        {
            try
            {
                var openFileStream = File.OpenRead(loadPath);
                BinaryFormatter deserializer = new BinaryFormatter();
                this.Event = (Event)deserializer.Deserialize(openFileStream);
                openFileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                DoErrorAction(ex.Message);
                return false;
            }
        }
    }
}
