using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneServer
{
    /// <summary>
    /// 表示查询信息
    /// </summary>
    [Serializable]
    internal class LnQuery
    {
        /// <summary>
        /// 获取或设置索引名
        /// </summary>
        public string IndexName { get; set; }
        /// <summary>
        ///  获取或设置关键字
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        ///  获取或设置匹配的字段
        /// </summary>
        public LnMatchField[] MatchFields { get; set; }
        /// <summary>
        ///  获取或设置排序的字段
        /// </summary>
        public LnSortField[] SortFields { get; set; }     
        /// <summary>
        ///  获取或设置跳过的元素数量
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        ///  获取或设置取出的元素数量
        /// </summary>
        public int Take { get; set; }
        /// <summary>
        /// 最大搜索记录数
        /// </summary>
        public int MaxSearchCount { get; set; }
    }
}
