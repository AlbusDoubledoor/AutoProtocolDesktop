using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    /*
     * Интерфейс "Потребитель действия при ошибке"
     * Класс, реализующий данный интерфейс, заявляет о том, что может порождать ошибки, на которые возможно реагировать действием
     */
    interface IErrorActionConsumer
    {
        public Action<string> ErrorAction { get; set; }

        public void DoErrorAction(string errorMessage);
    }
}
