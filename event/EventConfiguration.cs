using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutoProtocol.EventMVVM
{
    class EventConfiguration
    {
        private Event _event;
        public static readonly string FILE_EXTENSION = ".apc";
        public EventConfiguration(Event eventObject)
        {
            _event = eventObject;
        }

        public bool WriteFile(string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(File.Create(fileName)))
                {
                    writer.WriteLine("%MOBILE_START%");
                    writer.WriteLine($"EVENT_NAME={_event.Name}");
                    writer.WriteLine($"MAX_PARTICIPANT={_event.Participants.Count}");
                    writer.WriteLine("%MOBILE_END%");
                }
                return true;
            } catch
            {
                return false;
            }
        }
    }
}
