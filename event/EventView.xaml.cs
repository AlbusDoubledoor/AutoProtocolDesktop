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
            _eventViewModel.ProvideFileExport = BasicFileExport;
            _eventViewModel.ProvideFileImport = BasicFileImport;
        }

        private string BasicFileExport(string filter)
        {
            var exportDialog = new SaveFileDialog();
            exportDialog.Filter = filter;
            if (exportDialog.ShowDialog() == true)
            {
                return exportDialog.FileName;
            }
            return "";
        }

        private string BasicFileImport(string filter)
        {
            var importDialog = new SaveFileDialog();
            importDialog.Filter = filter;
            if (importDialog.ShowDialog() == true)
            {
                return importDialog.FileName;
            }
            return "";
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
