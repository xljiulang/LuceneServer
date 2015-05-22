using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示排序字段信息
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name ={Name}, Asc = {Asc}")]
    internal class LnSortField
    {
        /// <summary>
        /// 获取或设置字段名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置字段排序类型
        /// </summary>
        public int SortType { get; set; }
        /// <summary>
        /// 获取或设置是否为升序
        /// </summary>
        public bool Asc { get; set; }

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
