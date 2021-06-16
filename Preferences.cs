using System.IO;
using System.Xml;

namespace AutoProtocol
{
    public static class Preferences
    {
        private static readonly string PREFERENCE_FILE_NAME = "preferences.xml";
        private static readonly string PREFERENCE_ROOT_ELEMENT = "preferences";
        private static readonly string PREFERENCE_NODE = "preference";
        private static readonly string PREFERENCE_NODE_ATTRIBUTE_ID = "id";


        private static XmlElement NewElement(XmlDocument xmlDoc, string id, string value)
        {
            XmlElement newElement = xmlDoc.CreateElement(PREFERENCE_NODE);
            newElement.InnerText = value;
            newElement.SetAttribute(PREFERENCE_NODE_ATTRIBUTE_ID, id);
            return newElement;
        }

        private static void SavePreferences(XmlDocument preferences)
        {
            preferences.Save(PREFERENCE_FILE_NAME);
        }


        private static string makePreferenceSelector(string element)
        {
            return $"{PREFERENCE_ROOT_ELEMENT}/{PREFERENCE_NODE}[@{PREFERENCE_NODE_ATTRIBUTE_ID}='{element}']";
        }

        private static XmlDocument InitializePreferences()
        {
            XmlDocument preferences = new XmlDocument();
            if (!File.Exists(PREFERENCE_FILE_NAME))
            {
                XmlElement rootElement = preferences.CreateElement(PREFERENCE_ROOT_ELEMENT);
                preferences.AppendChild(rootElement);
                SavePreferences(preferences);
            } 
            else
            {
                preferences.Load(PREFERENCE_FILE_NAME);
            }
            
            return preferences;
        }

        public static string getString(string element, string defaultValue)
        {
            return InitializePreferences().SelectSingleNode(makePreferenceSelector(element))?.InnerText ?? defaultValue;
        }

        public static void putString(string element, string value)
        {
            XmlDocument preferences = InitializePreferences();
            XmlNode target = preferences.SelectSingleNode(makePreferenceSelector(element));
            if (target == null)
            {
                preferences.DocumentElement.AppendChild(NewElement(preferences, element, value));
            } 
            else
            {
                target.InnerText = value;
            }
            SavePreferences(preferences);
        }
    }
}
