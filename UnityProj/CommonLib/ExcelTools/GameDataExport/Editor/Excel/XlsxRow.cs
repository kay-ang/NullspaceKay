using System.Collections.Generic;

namespace Nullspace
{
    public class XlsxRow
    {
        public List<string> Values { get; set; }
        public XlsxSheet Parent { get; set; }
        public int RowIndex { get; set; }
        public XlsxRow(int rowIndex, List<string> v, XlsxSheet parent)
        {
            RowIndex = rowIndex;
            Values = v;
            Parent = parent;
        }

        public string this[int index]
        {
            get { return Values[index]; }
        }

        public int Count { get { return Values.Count; } }
    }
}
