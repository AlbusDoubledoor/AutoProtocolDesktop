using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoProtocol.EventMVVM
{
    class EventDataLoader
    {
        private Event _event;

        public EventDataLoader(Event eventObject, String fileName)
        {
            _event = eventObject;
            int pointId = 0;
            using (var streamReader = new StreamReader(File.OpenRead(fileName)))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (line.Contains('%'))
                    {
                        do
                        {
                            line = streamReader.ReadLine();
                            if (line.Contains("POINT_ID"))
                            {
                                pointId = Int32.Parse(line.Substring(line.IndexOf("=") + 1));
                            }
                        } while (!line.Contains('%'));

                        continue;
                    }

                    int delimIndex = line.IndexOf('=');
                    int id = Int32.Parse(line.Substring(0, delimIndex));
                    string[] times = line.Substring(delimIndex + 1).Split(';', StringSplitOptions.RemoveEmptyEntries);
                    Participant targetParticipant = null;
                    foreach (Participant participant in _event.Participants)
                    {
                        if (participant.Id == id)
                        {
                            targetParticipant = participant;
                            break;
                        }
                    }

                    ParticipantTime targetParticipantTime = null;
                    foreach (ParticipantTime participantTime in targetParticipant.ParticipantTimes)
                    {
                        if (participantTime.CheckPoint.Id == pointId)
                        {
                            targetParticipantTime = participantTime;
                        }
                    }

                    int bound = times.Length > _event.LapsCount ? _event.LapsCount : times.Length;
                    for (int i = 0; i < bound; ++i)
                    {
                        targetParticipantTime.Times[i].RawValue = long.Parse(times[i]);
                    }
                }
            }
        }

    }
}
