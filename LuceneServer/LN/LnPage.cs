using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneServer
{
    /// <summary>
    /// 表示LN通用分页信息
    /// </summary>
    internal class LnPage
    {
        /// <summary>
        /// 所有记录条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页的记录
        /// </summary>
        public IEnumerable<LnModel> Models { get; set; }
    }
}
