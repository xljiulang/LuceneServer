using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示字段信息
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name ={Name}, Value = {Value}, IsString = {IsString}")]
    internal class LnField
    {
        /// <summary>
        /// 获取或设置字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置是否为string类型
        /// </summary>
        public bool IsString { get; set; }
    }
}
