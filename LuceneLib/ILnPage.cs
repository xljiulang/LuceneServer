using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示分页接口
    /// </summary>
    public interface ILnPage : IEnumerable
    {
        /// <summary>
        /// 获取所有记录条数
        /// </summary>
        int TotalCount { get; }
    }
}
