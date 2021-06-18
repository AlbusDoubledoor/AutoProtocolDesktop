using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AutoProtocol
{
    class BasicDialogs
    {
        public static string FileExport(string filter)
        {
            var exportDialog = new SaveFileDialog();
            exportDialog.Filter = filter;
            if (exportDialog.ShowDialog() == true)
            {
                return exportDialog.FileName;
            }
            return "";
        }

        public static string FileImport(string filter)
        {
            var importDialog = new OpenFileDialog();
            importDialog.Filter = filter;
            if (importDialog.ShowDialog() == true)
            {
                return importDialog.FileName;
            }
            return "";
        }

        public static bool Confirm(string confirmation)
        {
            string confirmationCaption = Application.Current.FindResource(R.DLG_TITLE__CONFIRMATION).ToString();
            MessageBoxResult confirmResult = MessageBox.Show(confirmation, confirmationCaption, MessageBoxButton.YesNo);
            return (confirmResult == MessageBoxResult.Yes);
        }
    }
}
