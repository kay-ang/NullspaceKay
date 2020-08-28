using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nullspace
{
    public class XlsxExportRecord
    {
        public string FilePath { get; set; }
        public string FileHash { get; set; }
        public string SheetInfo { get; set; }
        public string SheetHash { get; set; }
    }
}
