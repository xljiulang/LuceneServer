using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuceneServer
{
    /// <summary>
    /// 表示排序字段信息
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name ={Name}, Asc = {Asc}")]
    internal class LnSortField
    {
        /// <summary>
        /// 字段名的小写
        /// </summary>
        private string _name;

        /// <summary>
        ///  获取或设置字段名
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value.ToLower();
            }
        }

        /// <summary>
        ///  获取或设置字段的排序类型
        /// </summary>
        public int SortType { get; set; }

        /// <summary>
        ///  获取或设置是否升序排序
        /// </summary>
        public bool Asc { get; set; }

        /// <summary>
        /// 转换为SortField类型
        /// </summary>
        /// <returns></returns>
        public SortField ToSortField()
        {
            return new SortField(this.Name, this.SortType, !Asc);
        }
    }
}
