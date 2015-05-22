using Lucene.Net.Documents;
using Lucene.Net.Index;
using PanGu;
using PanGu.HighLight;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LuceneServer
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
                fieldName = fieldName.ToLower();
                return this.Fields.FirstOrDefault(item => item.Name == fieldName);
            }
        }

        /// <summary>
        /// 设置高亮
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <param name="matchFields">高亮信息</param>
        public void SetHighLight(string keywords, IEnumerable<LnMatchField> matchFields)
        {
            foreach (var matchField in matchFields)
            {
                if (matchField.CanSetHighLight() == true)
                {
                    var field = this[matchField.Name];
                    if (field != null)
                    {
                        field.SetHighLight(keywords, matchField);
                    }
                }
            }
        }

        /// <summary>
        /// 获取Term对象
        /// </summary>
        /// <returns></returns>
        public Term GetTerm()
        {
            var idField = this["id"];
            if (idField == null)
            {
                return null;
            }
            return new Term(idField.Name, idField.Value);
        }

        /// <summary>
        /// 转换为Document对象
        /// </summary>
        /// <returns></returns>
        public Document ToDocument()
        {
            var doc = new Document();
            foreach (var field in this.Fields)
            {
                doc.Add(field.ToField());
            }
            return doc;
        }
    }
}
