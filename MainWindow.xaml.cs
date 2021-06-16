using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace AutoProtocol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string EVENT_FILE_EXTENSION = ".ape";
        public MainWindow()
        {
            InitializeComponent();

            App.ApplyPreferences();
            App.OnLanguageChanged += LanguageChanged;
            App.OnThemeChanged += ThemeChanged;

            CultureInfo currentLanguage = App.Language;

            DataContext = this;
            MenuItem_Language.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currentLanguage);
                menuLang.Click += OnChangeLanguageClick;
                MenuItem_Language.Items.Add(menuLang);
            }

            string currentTheme = App.Theme;
            foreach (var theme in App.Themes)
            {
                MenuItem menuTheme = new MenuItem();
                menuTheme.SetResourceReference(HeaderedItemsControl.HeaderProperty, $"{R.THEME_HEADER_PREFIX}{theme.ToUpper()}");
                menuTheme.Tag = theme;
                menuTheme.IsChecked = theme.ToLower().Equals(currentTheme);
                menuTheme.Click += OnChangeThemeClick;
                MenuItem_Theme.Items.Add(menuTheme);
            }

            CommandBinding saveBind = new CommandBinding(ApplicationCommands.Save);
            saveBind.CanExecute += (sender, e) => e.CanExecute = _eventView != null;
            saveBind.Executed += SaveCommand_Executed;
            this.CommandBindings.Add(saveBind);

            CommandBinding loadBind = new CommandBinding(ApplicationCommands.Open);
            loadBind.CanExecute += (sender, e) => e.CanExecute = true;
            loadBind.Executed += LoadCommand_Executed;
            this.CommandBindings.Add(loadBind);

            CommandBinding newBind = new CommandBinding(ApplicationCommands.New);
            newBind.CanExecute += (sender, e) => e.CanExecute = true;
            newBind.Executed += CloseCommand_Executed;
            newBind.Executed += NewCommand_Executed;
            this.CommandBindings.Add(newBind);

            CommandBinding closeBind = new CommandBinding(ApplicationCommands.Close);
            closeBind.CanExecute += (sender, e) => e.CanExecute = _eventView != null;
            closeBind.Executed += CloseCommand_Executed;
            this.CommandBindings.Add(closeBind);
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            // Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem menuItem in MenuItem_Language.Items)
            {
                CultureInfo cultureInfo = menuItem.Tag as CultureInfo;
                menuItem.IsChecked = cultureInfo != null && cultureInfo.Equals(currLang);
            }
        }

        private void ThemeChanged(Object sender, EventArgs e)
        {
            string currTheme = App.Theme;

            // Отмечаем нужный пункт смены темы как выбранная тема
            foreach (MenuItem menuItem in MenuItem_Theme.Items)
            {
                string theme = menuItem.Tag as string;
                theme = theme.ToLower();
                menuItem.IsChecked = theme != null && currTheme.Equals(theme);
            }
        }

        private void OnChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                CultureInfo language = menuItem.Tag as CultureInfo;
                if (language != null)
                {
                    App.Language = language;
                }
            }
        }

        private void OnChangeThemeClick(Object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                string theme = menuItem.Tag as string;
                if (theme != null)
                {
                    App.Theme = theme;
                }
            }
        }

        private EventMVVM.EventView _eventView;

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_eventView == null)
            {
                _eventView = new EventMVVM.EventView();
                MainParentLayout.Children.Remove(EventPlaceholder);
                MainParentLayout.Children.Add(_eventView);
                MenuItem_Event_Close.Visibility = Visibility.Visible;
            }
        }

        private void CloseCommand_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            if (_eventView != null)
            {
                MainParentLayout.Children.Remove(_eventView);
                MainParentLayout.Children.Add(EventPlaceholder);
                _eventView = null;
                MenuItem_Event_Close.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var saveEventDialog = new SaveFileDialog();
            saveEventDialog.Filter = $"{FindResource(R.FILE_FILTER__EVENT)} (*{EVENT_FILE_EXTENSION})|*{EVENT_FILE_EXTENSION}";
            if (saveEventDialog.ShowDialog() == true)
            {
                if (!_eventView.SaveCommand_Executed_Wrapper(sender, e, saveEventDialog.FileName))
                {
                    MessageBox.Show(String.Format(FindResource(R.DLG_MSG__SAVE_ERROR).ToString(), saveEventDialog.FileName), FindResource(R.DLG_TITLE__SAVE_ERROR).ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var loadEventDialog = new OpenFileDialog();
            loadEventDialog.Filter = $"{FindResource(R.FILE_FILTER__EVENT)} (*{EVENT_FILE_EXTENSION})|*{EVENT_FILE_EXTENSION}";
            if (loadEventDialog.ShowDialog() == true)
            {
                NewCommand_Executed(sender, e);
                if (!_eventView.LoadCommand_Executed_Wrapper(sender, e, loadEventDialog.FileName))
                {
                    MessageBox.Show(String.Format(FindResource(R.DLG_MSG__LOAD_ERROR).ToString(), loadEventDialog.FileName), FindResource(R.DLG_TITLE__LOAD_ERROR).ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    CloseCommand_Executed(sender, e);
                }
            }
        }

        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand ??
                        (_exitCommand = new RelayCommand(obj =>
                        {
                            this.Close();
                        }));
            }
        }
    }
}
