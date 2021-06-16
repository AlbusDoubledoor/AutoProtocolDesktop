using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    interface IConfirmProvidable
    {
        public Func<string, bool> ProvideConfirm { get; set; }

        public bool RequestConfirm(string confirmation);
    }
}
