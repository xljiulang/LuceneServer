using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示排序类型
    /// </summary>
    [Serializable]
    internal enum LnSortType
    {
        BYTE = 10,
        CUSTOM = 9,
        DOC = 1,
        DOUBLE = 7,
        FLOAT = 5,
        INT = 4,
        LONG = 6,
        SCORE = 0,
        SHORT = 8,
        STRING = 3,
        STRING_VAL = 11,
    }
}
