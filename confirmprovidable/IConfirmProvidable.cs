using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    /*
     * Интерфейс "Поставляемое подтверждение"
     * Класс, реализующий данный интерфейс, заявляет о том, что может запрашивать подтверждение, способ которого ему нужно предоставить
     */
    interface IConfirmProvidable
    {
        public Func<string, bool> ProvideConfirm { get; set; }

        public bool RequestConfirm(string confirmation);
    }
}
