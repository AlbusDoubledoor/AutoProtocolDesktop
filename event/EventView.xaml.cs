using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoProtocol;
using Microsoft.Win32;

namespace AutoProtocol.EventMVVM
{
    public partial class EventView
    {
        private EventViewModel _eventViewModel;

        public EventView()
        {
            InitializeComponent();

            _eventViewModel = new EventViewModel();
            DataContext = _eventViewModel;

            _eventViewModel.ErrorAction = (errorMessage) => MessageBox.Show(errorMessage, FindResource(R.DLG_TITLE__EVENT_ERROR).ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            _eventViewModel.ProvideFileExport = BasicDialogs.FileExport;
            _eventViewModel.ProvideFileImport = BasicDialogs.FileImport;
            _eventViewModel.ProvideConfirm = BasicDialogs.Confirm;
        }

        public bool SaveCommand_Executed_Wrapper(object sender, ExecutedRoutedEventArgs e, String savePath)
        {
            if (_eventViewModel != null)
            {
                return _eventViewModel.Save(savePath);
            }
            return false;
        }

        public bool LoadCommand_Executed_Wrapper(object sender, ExecutedRoutedEventArgs e, String loadPath)
        {
            if (_eventViewModel != null)
            {
                 return _eventViewModel.Load(loadPath);
            }
            return false;
        }
    }
}
