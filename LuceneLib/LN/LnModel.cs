
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示通用模型
    /// </summary>
    [Serializable]
    internal class LnModel
    {
        /// <summary>
        /// 获取或设置字段
        /// </summary>
        public LnField[] Fields { get; set; }

        /// <summary>
        /// LN通用模型
        /// </summary>
        public LnModel()
        {
        }

        /// <summary>
        /// LN通用模型
        /// </summary>
        /// <param name="fields">字段</param>
        public LnModel(IEnumerable<LnField> fields)
        {
            this.Fields = fields.ToArray();
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public LnField this[string fieldName]
        {
            get
            {
                return this.Fields.FirstOrDefault(item => string.Equals(fieldName, item.Name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
