using NetworkSocket.Fast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LuceneLib
{
    /// <summary>
    /// 表示索引查询对象
    /// 不可继承
    /// </summary>
    /// <typeparam name="T">Model类型</typeparam>   
    public sealed class LnQuery<T>
    {
        private readonly IFastSession Session;
        private readonly LnProperty[] properties;

        private readonly string indexName;
        private readonly string keywords;

        private int skipCount = 0;
        private int takeCount = int.MaxValue;

        private HashSet<LnMatchField> matchFields = new HashSet<LnMatchField>();
        private HashSet<LnSortField> sortFields = new HashSet<LnSortField>();

        /// <summary>
        /// 索引查询对象
        /// </summary>
        /// <param name="session">会话对象</param>
        /// <param name="properties">模型的属性</param>
        /// <param name="indexName">索引名称</param>
        /// <param name="keywords">关键字</param>
        internal LnQuery(IFastSession session, LnProperty[] properties, string indexName, string keywords)
        {
            this.Session = session;
            this.properties = properties;
            this.indexName = indexName;
            this.keywords = keywords;
        }

        /// <summary>
        /// 跳过元素
        /// </summary>
        /// <param name="count">元素个数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public LnQuery<T> Skip(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.skipCount = count;
            return this;
        }

        /// <summary>
        /// 取元素
        /// </summary>
        /// <param name="count">元素个数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public LnQuery<T> Take(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.takeCount = count;
            return this;
        }

        /// <summary>
        /// 获取属性名
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private string GetPropertyName<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            var body = keySelector.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("keySelector不是MemberExpression类型");
            }
            if (body.Member.IsDefined(typeof(NoneIndexAttribute), true) == true)
            {
                throw new ArgumentException(string.Format("类型{0}的属性{1}为含NoneIndex特性，不能进行相关检索", typeof(T).Name, body.Member.Name));
            }
            return body.Member.Name;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private LnProperty GetLnProperty<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var name = this.GetPropertyName(keySelector);
            var property = this.properties.FirstOrDefault(item => item.Name == name);
            if (property == null)
            {
                throw new ArgumentException(string.Format("类型{0}的属性{1}为不支持的检索类型：{2}", typeof(T).Name, name, typeof(TKey).Name));
            }
            return property;
        }

        /// <summary>
        /// 设置搜索匹配的文本字段
        /// </summary>      
        /// <param name="keySelector">字段</param>    
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> MatchField(Expression<Func<T, string>> keySelector)
        {
            return this.MatchField(keySelector, null, null, 0);
        }

        /// <summary>
        /// 设置搜索匹配的文本字段
        /// 并设置关键字Html元素的class名
        /// </summary>      
        /// <param name="keySelector">字段</param>    
        /// <param name="keywordClass">class名</param>      
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> MatchField(Expression<Func<T, string>> keySelector, string keywordClass)
        {
            return this.MatchField(keySelector, keywordClass, 0);
        }

        /// <summary>
        /// 设置搜索匹配的文本字段
        /// 并设置关键字Html元素的class名
        /// 返回其最佳摘要段
        /// </summary>      
        /// <param name="keySelector">字段</param>    
        /// <param name="keywordClass">class名</param>
        /// <param name="fragmentSize">摘要段的字符数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> MatchField(Expression<Func<T, string>> keySelector, string keywordClass, int fragmentSize)
        {
            var prefix = string.Format(@"<font class=""{0}"">", keywordClass);
            var suffix = "</font>";
            return this.MatchField(keySelector, prefix, suffix, fragmentSize);
        }

        /// <summary>
        /// 设置搜索匹配的文本字段
        /// 并设置关键字的高亮颜色
        /// </summary>       
        /// <param name="keySelector">字段</param>
        /// <param name="keywordColor">关键字高亮颜色</param>     
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> MatchField(Expression<Func<T, string>> keySelector, Color keywordColor)
        {
            return this.MatchField(keySelector, keywordColor, 0);
        }

        /// <summary>
        /// 设置搜索匹配的文本字段
        /// 并设置关键字的高亮颜色
        /// 返回其最佳摘要段
        /// </summary>       
        /// <param name="keySelector">字段</param>
        /// <param name="keywordColor">关键字高亮颜色</param>
        /// <param name="fragmentSize">摘要段的字符数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> MatchField(Expression<Func<T, string>> keySelector, Color keywordColor, int fragmentSize)
        {
            var prefix = string.Format(@"<font color=""{0}"">", ColorTranslator.ToHtml(keywordColor));
            var suffix = "</font>";
            return this.MatchField(keySelector, prefix, suffix, fragmentSize);
        }

        /// <summary>
        /// 设置搜索匹配的文本字段
        /// 并设置关键字的前缀和后缀
        /// 返回其最佳摘要段
        /// </summary>       
        /// <param name="keySelector">字段</param>
        /// <param name="keywordPrefix">关键字的前缀</param>
        /// <param name="keywordSuffix">关键字的后缀</param>
        /// <param name="fragmentSize">摘要段的字符数(0为全部)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> MatchField(Expression<Func<T, string>> keySelector, string keywordPrefix, string keywordSuffix, int fragmentSize)
        {
            var name = this.GetPropertyName(keySelector);
            var matchField = new LnMatchField
            {
                Name = name,
                FragmentSize = fragmentSize,
                KeywordPrefix = keywordPrefix,
                KeywordSuffix = keywordSuffix
            };
            this.matchFields.Add(matchField);
            return this;
        }

        /// <summary>
        /// 设置升序排序字段
        /// </summary>
        /// <typeparam name="TKey">字段类型</typeparam>
        /// <param name="keySelector">排序字段</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var property = this.GetLnProperty(keySelector);
            var sortFile = new LnSortField
            {
                Asc = true,
                Name = property.Name,
                SortType = (int)property.SortType
            };
            this.sortFields.Add(sortFile);
            return this;
        }

        /// <summary>
        /// 设置降序排序字段
        /// </summary>
        /// <typeparam name="TKey">字段类型</typeparam>
        /// <param name="keySelector">排序字段</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public LnQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var property = this.GetLnProperty(keySelector);
            var sortFile = new LnSortField
            {
                Asc = false,
                Name = property.Name,
                SortType = (int)property.SortType
            };
            this.sortFields.Add(sortFile);
            return this;
        }

        /// <summary>
        /// LN模型转换为业务Model
        /// </summary>
        /// <param name="lnModel"></param>
        /// <returns></returns>
        private T LnModelToTModel(LnModel lnModel)
        {
            var instance = Activator.CreateInstance<T>();
            for (var i = 0; i < this.properties.Length; i++)
            {
                var property = this.properties[i];
                var lnField = lnModel[property.Name];
                if (lnField != null)
                {
                    property.SetValue(instance, lnField.Value);
                }
            }
            return instance;
        }

        /// <summary>
        /// LN模型转换为业务Model
        /// </summary>
        /// <param name="lnModels">LN模型</param>
        /// <returns></returns>
        private IEnumerable<T> LnModelToTModel(IEnumerable<LnModel> lnModels)
        {
            if (lnModels == null)
            {
                yield break;
            }

            foreach (var lnModel in lnModels)
            {
                yield return this.LnModelToTModel(lnModel);
            }
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="maxSearchCount">最大搜索记录数</param>
        /// <returns></returns>
        private Task<LnPage> Query(int maxSearchCount)
        {
            var query = new
            {
                IndexName = this.indexName,
                Keywords = this.keywords,
                MatchFields = this.matchFields.ToArray(),
                SortFields = this.sortFields.ToArray(),
                Skip = this.skipCount,
                Take = this.takeCount,
                MaxSearchCount = maxSearchCount,
            };
            return this.Session.InvokeApi<LnPage>("SearchIndex", query);
        }

        /// <summary>
        /// 转换为ArrayOf(T)的Task对象
        /// </summary>
        /// <exception cref="SocketException"></exception>       
        /// <returns></returns>
        public Task<T[]> ToArray()
        {
            return this.Query(this.skipCount + this.takeCount)
                .ContinueWith((task) => this.LnModelToTModel(task.Result.Models).ToArray());
        }

        /// <summary>
        /// 转换为ListOf(T)的Task对象        
        /// </summary>
        /// <exception cref="SocketException"></exception>    
        /// <returns></returns>
        public Task<List<T>> ToList()
        {
            return this.Query(this.skipCount + this.takeCount)
                .ContinueWith((task) => this.LnModelToTModel(task.Result.Models).ToList());
        }

        /// <summary>
        /// 转换为LnPageOf(T)的Task对象        
        /// </summary>       
        /// <exception cref="SocketException"></exception>    
        /// <returns></returns>
        public Task<LnPage<T>> ToPage()
        {
            return this.ToPage(int.MaxValue);
        }

        /// <summary>
        /// 转换为LnPageOf(T)的Task对象        
        /// </summary>
        /// <param name="maxSearchCount">最大搜索记录数</param>
        /// <exception cref="SocketException"></exception>    
        /// <returns></returns>
        public Task<LnPage<T>> ToPage(int maxSearchCount)
        {
            return this.Query(maxSearchCount)
                .ContinueWith((task) => new LnPage<T>(this.LnModelToTModel(task.Result.Models), task.Result.TotalCount));
        }

        #region Hidden
        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public Type GetType()
        {
            return base.GetType();
        }
        #endregion
    }
}
