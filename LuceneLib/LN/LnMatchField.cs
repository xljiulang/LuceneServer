using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示要匹配的字段
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name ={Name}")]
    internal class LnMatchField
    {
        /// <summary>
        /// 获取或设置字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置摘要段的字符数
        /// 0为全部
        /// </summary>
        public int FragmentSize { get; set; }

        /// <summary>
        /// 获取或设置关键字前缀
        /// </summary>
        public string KeywordPrefix { get; set; }

        /// <summary>
        /// 获取或设置关键字后缀
        /// </summary>
        public string KeywordSuffix { get; set; }
         
        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        /// <summary>
        /// 对象比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj != null && this.GetHashCode() == obj.GetHashCode();
        }
    }
}
