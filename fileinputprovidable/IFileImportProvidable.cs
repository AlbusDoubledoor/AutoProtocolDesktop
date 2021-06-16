using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    interface IFileImportProvidable
    {
        public Func<string, string> ProvideFileImport { get; set; }

        public string RequestFileImport(string filter);
    }
}
