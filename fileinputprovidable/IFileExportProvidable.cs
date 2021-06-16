using System;
using System.Collections.Generic;
using System.Text;

namespace AutoProtocol
{
    interface IFileExportProvidable
    {
        public Func<string, string> ProvideFileExport { get; set; }

        public string RequestFileExport(string filter);
    }
}
