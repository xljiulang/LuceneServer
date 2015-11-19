using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket;
using NetworkSocket.Fast;
using Lucene.Net.Search;
using LuceneServer.Filters;
using NetworkSocket.Core;

namespace LuceneServer.Services
{
    /// <summary>
    /// Lucene.Net服务
    /// </summary>
    [LoginFilter]
    internal class LnService : FastApiService
    {
        /// <summary>
        /// 创建或更新到索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        [Api]
        public bool SetIndex(string indexName, LnModel[] models)
        {
            return LnHelper.SetIndex(indexName, models);
        }

        /// <summary>
        /// 删除索引的全部记录
        /// </summary>
        /// <param name="indexName">索引名称</param>     
        /// <returns></returns>
        [Api("DeleteAll")]
        public bool DeleteIndex(string indexName)
        {
            return LnHelper.DeleteIndex(indexName);
        }

        /// <summary>
        /// 删除索引的记录
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="ids">记录的id值</param>
        /// <returns></returns>
        [Api]
        public bool DeleteIndex(string indexName, string[] ids)
        {
            var idFields = ids.Select(item => new LnField { Name = "ID", Value = item });
            return LnHelper.DeleteIndex(indexName, idFields);
        }

        /// <summary>
        /// 搜索索引
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns></returns>
        [Api]
        public LnPage SearchIndex(LnQuery query)
        {
            return LnHelper.SearchIndex(query);
        }
    }
}
