using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    /*
     * Интерфейс "Поставляемый импорт файлов"
     * Класс, реализующий данный интерфейс, заявляет о том, что может импортировать файлы, и ему нужно предсотавить способ сделать это
     */
    interface IFileImportProvidable
    {
        public Func<string, string> ProvideFileImport { get; set; }

        public string RequestFileImport(string filter);
    }
}
