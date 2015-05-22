using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuceneServer
{
    /// <summary>
    /// 表示要匹配的字段
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name ={Name}")]
    internal class LnMatchField
    {
        /// <summary>
        /// 字段名小写
        /// </summary>
        private string _name;

        /// <summary>
        /// 获取或设置字段名
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
        /// 是否可以设置高亮
        /// </summary>
        /// <returns></returns>
        public bool CanSetHighLight()
        {
            return this.FragmentSize > 0 || !string.IsNullOrEmpty(this.KeywordSuffix) || !string.IsNullOrEmpty(this.KeywordPrefix);
        }
    }
}
