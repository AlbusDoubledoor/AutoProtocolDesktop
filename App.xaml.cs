using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Windows;

namespace AutoProtocol
{
    public partial class App : Application
    {
        private static List<CultureInfo> LanguagesList = new List<CultureInfo>();
        private static List<string> ThemesList = new List<string>();
        private static string CurrentTheme;
        private static readonly string PREFERENCE_LANGUAGE = "lang";
        private static readonly string PREFERENCE_THEME = "theme";
        private static readonly string LANGUAGE_DEFAULT = "ru-RU";
        private static readonly string THEME_DEFAULT = "day";

        public static List<CultureInfo> Languages
        {
            get
            {
                return LanguagesList;
            }
        }

        public static List<string> Themes
        {
            get
            {
                return ThemesList;
            }
        }

        public App()
        {
            App.OnLanguageChanged += OnLanguageChangedApp;
            App.OnThemeChanged += OnThemeChangedApp;

            LanguagesList.Clear();
            LanguagesList.Add(new CultureInfo("en-US"));
            LanguagesList.Add(new CultureInfo("ru-RU"));

            ThemesList.Clear();
            ThemesList.Add("Day");
            ThemesList.Add("Night");
        }

        public static event EventHandler OnLanguageChanged;
        public static event EventHandler OnThemeChanged;

        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                /* Меняем язык приложения */
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                /* Создаём ResourceDictionary для новой культуры */
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        resourceDictionary.Source = new Uri(String.Format("resources/langs/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                    default:
                        resourceDictionary.Source = new Uri("resources/langs/lang.xaml", UriKind.Relative);
                        break;
                }

                /* Находим старый ResourceDictionary и удаляем его и добавляем новый ResourceDictionary */
                ResourceDictionary oldDictionary = (from d in Application.Current.Resources.MergedDictionaries
                                                    where d.Source != null && d.Source.OriginalString.StartsWith("resources/langs/lang.")
                                                    select d).First();
                if (oldDictionary != null)
                {
                    int dictIndex = Application.Current.Resources.MergedDictionaries.IndexOf(oldDictionary);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDictionary);
                    Application.Current.Resources.MergedDictionaries.Insert(dictIndex, resourceDictionary);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }

                /* Вызываем эвент для оповещения всех окон */
                OnLanguageChanged(Application.Current, new EventArgs());
            }
        }

        public static string Theme
        {
            get
            {
                return CurrentTheme.ToLower();
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == CurrentTheme) return;

                value = value.ToLower();
                /* Меняем тему приложения */
                CurrentTheme = value;

                /* Создаём ResourceDictionary для новой темы */
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(String.Format("resources/themes/{0}.xaml", value), UriKind.Relative);

                /* Находим старый ResourceDictionary и удаляем его и добавляем новый ResourceDictionary */
                ResourceDictionary oldDictionary = (from d in Application.Current.Resources.MergedDictionaries
                                                    where d.Source != null && d.Source.OriginalString.StartsWith("resources/themes/")
                                                    select d).First();
                if (oldDictionary != null)
                {
                    int dictIndex = Application.Current.Resources.MergedDictionaries.IndexOf(oldDictionary);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDictionary);
                    Application.Current.Resources.MergedDictionaries.Insert(dictIndex, resourceDictionary);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }

                /* Вызываем эвент для оповещения всех окон */
                OnThemeChanged(Application.Current, new EventArgs());
            }
        }

        public static void ApplyPreferences()
        {
            Language = new CultureInfo(Preferences.getString(PREFERENCE_LANGUAGE, LANGUAGE_DEFAULT));
            Theme = Preferences.getString(PREFERENCE_THEME, THEME_DEFAULT);

        }

        private void OnLanguageChangedApp(Object sender, EventArgs e)
        {
            Preferences.putString(PREFERENCE_LANGUAGE, Language.Name);
        }

        private void OnThemeChangedApp(Object sender, EventArgs e)
        {
            Preferences.putString(PREFERENCE_THEME, Theme);
        }
    }
}
