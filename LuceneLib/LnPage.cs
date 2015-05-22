using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示分页信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LnPage<T> : ILnPage, IEnumerable<T>
    {
        /// <summary>
        /// 分页的模型
        /// </summary>
        private IEnumerable<T> models;

        /// <summary>
        /// 获取所有记录条数
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        /// <param name="models">分页的模型</param>
        /// <param name="totalCount">所有记录条数</param>
        internal LnPage(IEnumerable<T> models, int totalCount)
        {
            this.models = models;
            this.TotalCount = totalCount;
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.models == null)
            {
                yield break;
            }
            foreach (var item in this.models)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
