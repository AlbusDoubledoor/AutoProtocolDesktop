using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutoProtocol.EventMVVM
{
    /*
     * Класс для сохранения и загрузки конфигураций событий с помощью файлов
     */
    class EventConfiguration : IDisposable
    {
        private Event _event;
        public static readonly string FILE_EXTENSION = ".apc";

        public Event Event { get => _event; }

        public EventConfiguration(Event eventObject)
        {
            _event = eventObject;
        }

        public void Dispose()
        {
            _event = null;
        }

        private const string MOBILE_BLOCK_START = "%MOBILE_START%";
        private const string MOBILE_BLOCK_END = "%MOBILE_END%";
        private const string KEY_VALUE_DELIMITER = "=";
        private const string KEY_MAX_PARTICIPANT = "MAX_PARTICIPANT";
        private const string KEY_AUTO_SYNC_DELAY = "AUTO_SYNC_DELAY";
        private const string KEY_MANUAL_SYNC_DELAY = "MANUAL_SYNC_DELAY";
        private const string KEY_LAPS_COUNT = "LAPS_COUNT";
        private const string KEY_CHECKPOINTS_COUNT = "CHECKPOINTS_COUNT";
        private const string KEY_EVENT_NAME = "EVENT_NAME";
        private const string KEY_PARTICIPANT_NAME = "PARTICIPANT_NAME";
        private const string KEY_CHECKPOINT_NAME = "CHECKPOINT_NAME";
        private const char TEMPLATE_STARTER = '{';
        private const char TEMPLATE_ENDER = '}';
        private const char META_CHAR = '%';

        public EventConfiguration(string fileName)
        {
            _event = new Event();
            using (var reader = new StreamReader(File.OpenRead(fileName)))
            {
                int maxParticipant = 0;
                int checkPointsCount = 0;
                int templateStart;
                int templateEnd;
                int id;
                string name;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains(META_CHAR)) continue;

                    int separatorIndex = line.IndexOf(KEY_VALUE_DELIMITER);
                    string key = line.Substring(0, separatorIndex);
                    string value;
                    try
                    {
                        value = line.Substring(separatorIndex + 1);
                    }
                    catch
                    {
                        value = "";
                    }
                    switch (key)
                    {
                        case KEY_EVENT_NAME:
                            Event.Name = value;
                            break;
                        case KEY_AUTO_SYNC_DELAY:
                            Event.AutoSyncDelay = Int32.Parse(value);
                            break;
                        case KEY_MANUAL_SYNC_DELAY:
                            Event.ManualSyncDelay = Int32.Parse(value);
                            break;
                        case KEY_LAPS_COUNT:
                            Event.LapsCount = Int32.Parse(value);
                            break;
                        case KEY_MAX_PARTICIPANT:
                            maxParticipant = Int32.Parse(value);
                            break;
                        case KEY_CHECKPOINTS_COUNT:
                            checkPointsCount = Int32.Parse(value);
                            break;
                        case KEY_PARTICIPANT_NAME:
                        case KEY_CHECKPOINT_NAME:
                            templateStart = value.IndexOf(TEMPLATE_STARTER);
                            templateEnd = value.IndexOf(TEMPLATE_ENDER);
                            id = Int32.Parse(value.Substring(templateStart + 1, templateEnd - templateStart - 1 ));
                            try
                            {
                                name = value.Substring(templateEnd + 1);
                            }
                            catch
                            {
                                name = "";
                            }
                            if (key.Equals(KEY_PARTICIPANT_NAME))
                            {
                                Event.Participants.Add(new Participant { Id = id, Name = name });
                            }
                            else
                            {
                                Event.CheckPoints.Add(new CheckPoint { Id = id, Name = name });
                            }
                            break;
                    }
                }
                if (Event.MaxParticipant == 0)
                {
                    if (maxParticipant == 0) throw new ArgumentNullException("null max participant");

                    for (int i = 1; i <= maxParticipant; ++i)
                    {
                        Event.Participants.Add(new Participant { Id = i });
                    }
                }

                if (Event.MaxCheckPoint == 0)
                {
                    if (checkPointsCount == 0) throw new ArgumentNullException("null checkpoints");

                    for (int i = 1; i <= checkPointsCount; ++i)
                    {
                        Event.CheckPoints.Add(new CheckPoint { Id = i });
                    }
                }
            }
        }


        public bool Write(string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(File.Create(fileName)))
                {
                    writer.WriteLine(MOBILE_BLOCK_START);
                    writer.WriteLine($"{KEY_EVENT_NAME}{KEY_VALUE_DELIMITER}{Event.Name}");
                    writer.WriteLine($"{KEY_MAX_PARTICIPANT}{KEY_VALUE_DELIMITER}{Event.MaxParticipant}");
                    writer.WriteLine($"{KEY_LAPS_COUNT}{KEY_VALUE_DELIMITER}{Event.LapsCount}");
                    writer.WriteLine($"{KEY_AUTO_SYNC_DELAY}{KEY_VALUE_DELIMITER}{Event.AutoSyncDelay}");
                    writer.WriteLine($"{KEY_MANUAL_SYNC_DELAY}{KEY_VALUE_DELIMITER}{Event.ManualSyncDelay}");
                    writer.WriteLine($"{KEY_CHECKPOINTS_COUNT}{KEY_VALUE_DELIMITER}{Event.MaxCheckPoint}");
                    writer.WriteLine(MOBILE_BLOCK_END);
                    foreach (Participant participant in Event.Participants)
                    {
                        writer.WriteLine($"{KEY_PARTICIPANT_NAME}{KEY_VALUE_DELIMITER}{TEMPLATE_STARTER}{participant.Id}{TEMPLATE_ENDER}{participant.Name}");
                    }
                    foreach (CheckPoint checkPoint in Event.CheckPoints)
                    {
                        writer.WriteLine($"{KEY_CHECKPOINT_NAME}{KEY_VALUE_DELIMITER}{TEMPLATE_STARTER}{checkPoint.Id}{TEMPLATE_ENDER}{checkPoint.Name}");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
