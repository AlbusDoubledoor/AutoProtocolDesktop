using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    interface IErrorActionConsumer
    {
        public Action<string> ErrorAction { get; set; }

        public void DoErrorAction(string errorMessage);
    }
}
