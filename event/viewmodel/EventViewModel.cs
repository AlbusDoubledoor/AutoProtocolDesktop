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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AutoProtocol.EventMVVM
{
    class EventViewModel : INotifyPropertyChanged, IErrorActionConsumer, IFileExportProvidable, IFileImportProvidable, IConfirmProvidable
    {
        private Event _event;
        private Participant _selectedParticipant;
        private CheckPoint _selectedCheckPoint;
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

                FinalData.Clear();
                IsConfigurationApplied = _event.IsStaged;
                if (IsConfigurationApplied)
                {
                    FillFinalData();
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
            string defaultEventName = Application.Current.FindResource(R.DEFAULT_VALUE__EVENT_NAME).ToString();
            string defaultParticipantName = Application.Current.FindResource(R.DEFAULT_VALUE__EVENT_NAME).ToString();
            string defaultCheckpointName = Application.Current.FindResource(R.DEFAULT_VALUE__EVENT_NAME).ToString();
            this.Event = new Event { Name = defaultEventName };
            AddParticipantCommand.Execute(defaultParticipantName);
            AddCheckPointCommand.Execute(defaultCheckpointName);
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
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__EVENT_CONFIGURATION).ToString();
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
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__EVENT_CONFIGURATION).ToString();
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
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__IMPORT_CONFIGURATION).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }
                    ));
            }
        }

        public bool IsConfigurationApplied
        {
            get => _event.IsStaged;
            set
            {
                _event.IsStaged = value;
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

        private void FillFinalData()
        {
            for (int i = 1; i <= LapsCount; ++i)
            {
                FinalData.Add(new FinalDataViewModel(i, CheckPoints, Participants));
                SelectedLap = FinalData[0];
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
                        string localizedMsg = Application.Current.FindResource(R.DLG_MSG__CONFIRM_APPLY_CONFIG).ToString();
                        IsConfigurationApplied = RequestConfirm(localizedMsg);
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

                        FinalData.Clear();
                        FillFinalData();
                    }));
            }
        }

        private RelayCommand _resetConfigurationCommand;
        public RelayCommand ResetConfigurationCommand
        {
            get
            {
                return _resetConfigurationCommand ??
                    (_resetConfigurationCommand = new RelayCommand(obj =>
                    {
                        string localizedMsg = Application.Current.FindResource(R.DLG_MSG__CONFIRM_RESET_CONFIG).ToString();
                        if (RequestConfirm(localizedMsg))
                        {
                            IsConfigurationApplied = false;
                            FinalData.Clear();
                            foreach (Participant participant in Participants)
                            {
                                participant.ParticipantTimes.Clear();
                            }
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
                            string fileExtension = EventDataLoader.FILE_EXTENSION;
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__EVENT_DATA).ToString();
                            string filter = $"{localizedFilter} (*{fileExtension})|*{fileExtension}";
                            string fileName = RequestFileImport(filter);

                            if (fileName.Length == 0) return;

                            EventDataLoader.Load(this.Event, fileName);
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__IMPORT_DATA).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        private HtmlToPdfConverter htmlConverter;
        private const string WEBKIT_PATH = "qtbin/";
        private const string BASIC_DOC_URL = "autoprotocol.doc";

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
                            if (htmlConverter == null)
                            {
                                htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
                                WebKitConverterSettings settings = new WebKitConverterSettings();
                                settings.WebKitPath = WEBKIT_PATH;
                                htmlConverter.ConverterSettings = settings;
                            }


                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__PROTOCOL).ToString();
                            string fileName = RequestFileExport($"{localizedFilter} (*.pdf)|*.pdf");

                            if (fileName.Length == 0) return;

                            StringBuilder overAllBuilder = new StringBuilder(HTMLTemplate);

                            overAllBuilder.Replace("{timestamp}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                            overAllBuilder.Replace("{event_name}", Event.Name);
                            overAllBuilder.Replace("{laps_count}", Event.LapsCount.ToString());
                            overAllBuilder.Replace("{check_points_count}", Event.MaxCheckPoint.ToString());
                            overAllBuilder.Replace("{participants_count}", Event.MaxParticipant.ToString());


                            StringBuilder templatedCollectionBuilder = new StringBuilder();
                            Regex template = new Regex(@"\{ParticipantTemplate\}(?<inner>(.|\n)+?)\{/ParticipantTemplate\}");
                            MatchCollection templateCollections = template.Matches(overAllBuilder.ToString());
                            string inner = "";
                            string bind = "";
                            foreach (Match match in templateCollections)
                            {
                                inner = match.Groups["inner"].Value;
                                foreach (Participant participant in Participants)
                                {
                                    templatedCollectionBuilder
                                        .Append(inner
                                            .Replace("{pname}", participant.Name)
                                            .Replace("{pid}", participant.Id.ToString()));
                                }
                                overAllBuilder.Replace(match.Value, templatedCollectionBuilder.ToString());
                                templatedCollectionBuilder.Clear();
                            }

                            template = new Regex(@"\{CheckPointTemplate\}(?<inner>(.|\n)+?)\{/CheckPointTemplate\}");
                            templateCollections = template.Matches(overAllBuilder.ToString());
                            foreach (Match match in templateCollections)
                            {
                                inner = match.Groups["inner"].Value;
                                foreach (CheckPoint checkPoint in CheckPoints)
                                {
                                    templatedCollectionBuilder
                                        .Append(inner
                                            .Replace("{cname}", checkPoint.Name)
                                            .Replace("{cid}", checkPoint.Id.ToString()));
                                }
                                overAllBuilder.Replace(match.Value, templatedCollectionBuilder.ToString());
                                templatedCollectionBuilder.Clear();
                            }

                            template = new Regex(@"\{LapTemplate\}(?<inner>(.|\n)+?)\{/LapTemplate\}");
                            templateCollections = template.Matches(overAllBuilder.ToString());
                            foreach (Match match in templateCollections)
                            {
                                inner = match.Groups["inner"].Value;
                                for (int i = 1; i <= LapsCount; ++i)
                                {
                                    templatedCollectionBuilder
                                        .Append(inner.Replace("{lid}", i.ToString()));
                                }
                                overAllBuilder.Replace(match.Value, templatedCollectionBuilder.ToString());
                                templatedCollectionBuilder.Clear();
                            }

                            template = new Regex(@"\{TimeTemplate Bind=(?<bind>(.|\n)+?)\}(?<inner>(.|\n)+?)\{/TimeTemplate\}");
                            templateCollections = template.Matches(overAllBuilder.ToString());
                            Regex bindRegex = new Regex(@"(\[Lap=(?<lap>(.|\n)+?)\])?(\[Participant=(?<participant>(.|\n)+?)\])?(\[CheckPoint=(?<checkpoint>(.|\n)+?)\])?");
                            string bindParticipant = "";
                            string bindCheckpoint = "";
                            string bindLap = "";
                            foreach (Match match in templateCollections)
                            {
                                bind = match.Groups["bind"].Value;
                                if (!bind.Equals("Any"))
                                {
                                    bindParticipant = "";
                                    bindCheckpoint = "";
                                    bindLap = "";
                                    MatchCollection bindMatches = bindRegex.Matches(bind);
                                    foreach (Match bindMatch in bindMatches)
                                    {
                                        bindParticipant = bindParticipant.Length == 0 ? bindMatch.Groups["participant"].Value : bindParticipant;
                                        bindCheckpoint = bindCheckpoint.Length == 0 ? bindMatch.Groups["checkpoint"].Value : bindCheckpoint;
                                        bindLap = bindLap.Length == 0 ? bindMatch.Groups["lap"].Value : bindLap;
                                    }
                                }
                                inner = match.Groups["inner"].Value;
                                foreach (Participant participant in Participants)
                                {
                                    if (bindParticipant.Length > 0 && !participant.Id.ToString().Equals(bindParticipant)) continue;
                                    foreach (ParticipantTime participantTime in participant.ParticipantTimes)
                                    {
                                        if (bindCheckpoint.Length > 0 && !participantTime.CheckPoint.Id.ToString().Equals(bindCheckpoint)) continue;
                                        for (int i = 1; i <= participantTime.Times.Count; ++i)
                                        {
                                            if (bindLap.Length > 0 && !(i.ToString().Equals(bindLap))) continue;
                                            Regex valueRegex = new Regex(@"\{value\}");
                                            templatedCollectionBuilder
                                                    .Append(valueRegex.Replace(inner, participantTime.Times[i - 1].StringValue, 1));
                                        }
                                    }
                                }
                                overAllBuilder.Replace(match.Value, templatedCollectionBuilder.ToString());
                                templatedCollectionBuilder.Clear();
                            }

                            overAllBuilder.Replace("{signature}", $"<u>{"".PadRight(15, '\0').Replace("\0", "&nbsp;")}</u>");

                            PdfDocument document = htmlConverter.Convert(overAllBuilder.ToString(), BASIC_DOC_URL);

                            using (var fileStream = File.Create(fileName))
                            {
                                document.Save(fileStream);
                                document.Close(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__EXPORT_PROTOCOL).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        private const string STANDARD_TEMPLATE_FILE_NAME = "standard_template.html";

        private RelayCommand _importStandardTemplateCommand;
        public RelayCommand ImportStandardTemplateCommand
        {
            get
            {
                return _importStandardTemplateCommand ??
                    (_importStandardTemplateCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            using (var reader = new StreamReader(File.OpenRead(STANDARD_TEMPLATE_FILE_NAME)))
                            {
                                HTMLTemplate = reader.ReadToEnd();
                            }
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__IMPORT_TEMPLATE).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        private RelayCommand _importTemplateCommand;
        public RelayCommand ImportTemplateCommand
        {
            get
            {
                return _importTemplateCommand ??
                    (_importTemplateCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            string fileExtension = ".html";
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__HTML_TEMPLATE).ToString();
                            string filter = $"{localizedFilter} (*{fileExtension})|*{fileExtension}";
                            string fileName = RequestFileImport(filter);

                            if (fileName.Length == 0) return;

                            using (var reader = new StreamReader(File.OpenRead(fileName)))
                            {
                                HTMLTemplate = reader.ReadToEnd();
                            }
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__IMPORT_TEMPLATE).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        private RelayCommand _exportTemplateCommand;
        public RelayCommand ExportTemplateCommand
        {
            get
            {
                return _exportTemplateCommand ??
                    (_exportTemplateCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            string fileExtension = ".html";
                            string localizedFilter = Application.Current.FindResource(R.FILE_FILTER__HTML_TEMPLATE).ToString();
                            string filter = $"{localizedFilter} (*{fileExtension})|*{fileExtension}";
                            string fileName = RequestFileExport(filter);

                            if (fileName.Length == 0) return;

                            using (var writer = new StreamWriter(File.Create(fileName)))
                            {
                                writer.Write(HTMLTemplate);
                            }
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__EXPORT_TEMPLATE).ToString();
                            DoErrorAction(String.Format(localizedErrorMsg, ex.Message));
                        }
                    }));
            }
        }

        private const string TEMPLATE_HELP_FILE = "template_help.pdf";

        private RelayCommand _templateHelpCommand;
        public RelayCommand TemplateHelpCommand
        {
            get
            {
                return _templateHelpCommand ??
                    (_templateHelpCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            Process.Start(TEMPLATE_HELP_FILE);
                        }
                        catch (Exception ex)
                        {
                            string localizedErrorMsg = Application.Current.FindResource(R.ERRORS__TEMPLATE_HELP).ToString();
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


        public string HTMLTemplate
        {
            get => _event.HTMLTemplate;
            set
            {
                _event.HTMLTemplate = value;
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
