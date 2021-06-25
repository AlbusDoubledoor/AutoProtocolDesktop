using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    /*
     * Интерфейс "Поставляемый экспорт файлов"
     * Класс, реализующий данный интерфейс, заявляет о том, что может экспортировать файлы, и ему нужно предсотавить способ сделать это
     */
    interface IFileExportProvidable
    {
        public Func<string, string> ProvideFileExport { get; set; }

        public string RequestFileExport(string filter);
    }
}
