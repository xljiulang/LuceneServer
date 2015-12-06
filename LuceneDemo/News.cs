using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneDemo
{
    /// <summary>
    /// 表示新闻
    /// </summary>
    public class News
    {
        /// <summary>
        /// ID
        /// </summary>
        [Category("基本信息")]
        [ReadOnlyAttribute(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Category("基本信息")]
        [ReadOnlyAttribute(true)]
        public int OrderIndex { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Category("基本信息")]
        [ReadOnlyAttribute(true)]
        public DateTime? CreateTime { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        [Category("搜索结果")]
        [ReadOnlyAttribute(true)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Category("搜索结果")]
        [ReadOnlyAttribute(true)]
        public string Content { get; set; }
    }
}
