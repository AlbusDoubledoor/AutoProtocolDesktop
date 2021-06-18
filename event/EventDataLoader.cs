using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoProtocol.EventMVVM
{
    class EventDataLoader
    {
        public static readonly string FILE_EXTENSION = ".apd";
        private const char META_CHAR = '%';
        private const string META_KEY_POINT_ID = "POINT_ID";
        private const char KEY_VALUE_DELIMITER = '=';
        private const char VALUES_DELIMITER = ';';

        public static void Load(Event targetEvent, String fileName)
        {
            int pointId = 0;
            using (var streamReader = new StreamReader(File.OpenRead(fileName)))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (line.Contains(META_CHAR))
                    {
                        do
                        {
                            line = streamReader.ReadLine();
                            if (line.Contains(META_KEY_POINT_ID))
                            {
                                pointId = Int32.Parse(line.Substring(line.IndexOf(KEY_VALUE_DELIMITER) + 1));
                            }
                        } while (!line.Contains(META_CHAR));

                        continue;
                    }

                    if (pointId == 0) throw new NullReferenceException("null pointId");

                    int delimIndex = line.IndexOf(KEY_VALUE_DELIMITER);
                    int id = Int32.Parse(line.Substring(0, delimIndex));
                    string[] times = line.Substring(delimIndex + 1).Split(VALUES_DELIMITER, StringSplitOptions.RemoveEmptyEntries);
                    Participant targetParticipant = null;
                    foreach (Participant participant in targetEvent.Participants)
                    {
                        if (participant.Id == id)
                        {
                            targetParticipant = participant;
                            break;
                        }
                    }

                    if (targetParticipant == null) continue;

                    ParticipantTime targetParticipantTime = null;
                    foreach (ParticipantTime participantTime in targetParticipant.ParticipantTimes)
                    {
                        if (participantTime.CheckPoint.Id == pointId)
                        {
                            targetParticipantTime = participantTime;
                        }
                    }

                    if (targetParticipantTime == null) continue;

                    int bound = times.Length > targetEvent.LapsCount ? targetEvent.LapsCount : times.Length;
                    for (int i = 0; i < bound; ++i)
                    {
                        targetParticipantTime.Times[i].RawValue = long.Parse(times[i]);
                    }
                }
            }
        }

    }
}
