using Lucene.Net.Documents;
using Lucene.Net.Index;
using PanGu;
using PanGu.HighLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LuceneServer
{
    /// <summary>
    /// 表示字段信息
    /// 字段名自动转换为小写
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name ={Name}, Value = {Value}, IsString = {IsString}")]
    internal class LnField
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
        /// 获取或设置值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置是否为string类型
        /// </summary>
        public bool IsString { get; set; }

        /// <summary>
        /// 设置高亮
        /// </summary>
        /// <param name="keywords">关键字</param>
        /// <param name="matchField">字段信息</param>      
        /// <returns></returns>
        public void SetHighLight(string keywords, LnMatchField matchField)
        {
            var format = new SimpleHTMLFormatter(matchField.KeywordPrefix, matchField.KeywordSuffix);
            var highLighter = new Highlighter(format, new Segment());
            highLighter.FragmentSize = matchField.FragmentSize < 1 ? this.Value.Length : matchField.FragmentSize;
            var newValue = highLighter.GetBestFragment(keywords, this.Value);

            if (string.IsNullOrEmpty(newValue) == false)
            {
                this.Value = newValue;
            }
        }

        /// <summary>
        /// 转换为Field类型
        /// </summary>
        /// <returns></returns>
        public Field ToField()
        {
            if (this.Name == "id" || this.IsString == false)
            {
                return new Field(this.Name, this.Value, Field.Store.YES, Field.Index.NOT_ANALYZED);
            }
            else
            {
                return new Field(this.Name, this.Value, Field.Store.YES, Field.Index.ANALYZED);
            }
        }

        /// <summary>
        /// 转换为Term类型
        /// </summary>
        /// <returns></returns>
        public Term ToTerm()
        {
            if (this.Name == "id")
            {
                return new Term(this.Name, this.Value);
            }
            return null;
        }
    }
}
