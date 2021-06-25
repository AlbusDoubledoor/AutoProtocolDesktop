using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AutoProtocol
{
    /*
     * Класс Базовых диалогов
     * Предоставляет различные общие диалоговые окна
     */
    class BasicDialogs
    {
        /*
         * Диалог экспорта файла. Позволяет выбрать файл с использованием встроенного в Windows диалога сохранения файла
         * 
         * @param filter - фильтр принимаемых файлов
         * @return - выбранное имя файла
         */
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

        /*
         * Диалог импорта файла. Позволяет выбрать файл с использованием встроенного в Windows диалога открытия файла
         * 
         * @param filter - фильтр принимаемых файлов
         * @return - имя выбранного файла
         */
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

        /*
         * Диалог подтверждения. Позволяет подтвердить что-либо (ОК или Отмена)
         * 
         * @param confirmation - сообщение о подтверждении
         * @return - результат подтверждения
         */
        public static bool Confirm(string confirmation)
        {
            string confirmationCaption = Application.Current.FindResource(R.DLG_TITLE__CONFIRMATION).ToString();
            MessageBoxResult confirmResult = MessageBox.Show(confirmation, confirmationCaption, MessageBoxButton.YesNo);
            return (confirmResult == MessageBoxResult.Yes);
        }
    }
}
