using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LnVersion = Lucene.Net.Util.Version;

namespace LuceneServer
{
    /// <summary>
    /// Lucene.Net操作封装
    /// </summary>
    internal static class LnHelper
    {
        /// <summary>
        /// 写入锁
        /// </summary>
        private static object _writerSyncRoot = new object();

        /// <summary>
        /// 分析器  
        /// </summary>
        private static Analyzer _analyzer = new PanGuAnalyzer();

        /// <summary>
        /// 索引目录
        /// </summary>
        private static ConcurrentDictionary<string, Directory> _indexStores = new ConcurrentDictionary<string, Directory>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 获取索引是否已存在
        /// </summary>
        /// <param name="indexName">索引名</param>
        /// <returns></returns>
        private static bool ExistsIndex(string indexName)
        {
            return System.IO.File.Exists(System.IO.Path.Combine("Indexs/" + indexName, "segments.gen"));
        }

        /// <summary>
        /// 获取索引存储目录
        /// </summary>
        /// <param name="indexName">索引名</param>
        /// <returns></returns>
        private static Directory GetIndexStore(string indexName)
        {
            return _indexStores.GetOrAdd(indexName, (key) =>
            {
                var indexPath = "Indexs/" + key;
                System.IO.Directory.CreateDirectory(indexPath);
                var dirInfo = new System.IO.DirectoryInfo(indexPath);
                return FSDirectory.Open(dirInfo);
            });
        }

        /// <summary>
        /// 提供安全执行Reader操作
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="indexName">索引名</param>
        /// <param name="func">Func</param>
        /// <returns></returns>
        private static T InvokeReaderNoLock<T>(string indexName, Func<IndexReader, T> func)
        {
            var indexStore = GetIndexStore(indexName);
            using (var reader = IndexReader.Open(indexStore, true))
            {
                return func(reader);
            }
        }

        /// <summary>
        /// 提供安全执行Writer操作
        /// </summary>
        /// <param name="indexName">索引名</param>
        /// <param name="action">Action</param>
        private static void InvokeWriterWithLock(string indexName, Action<IndexWriter> action)
        {           
            lock (_writerSyncRoot)
            {
                var indexStore = GetIndexStore(indexName);
                using (var writer = new IndexWriter(indexStore, _analyzer, !ExistsIndex(indexName), IndexWriter.MaxFieldLength.LIMITED))
                {
                    action(writer);
                    writer.Commit();
                }
            }
        }

        /// <summary>
        /// 创建或更新到索引 
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="models">模型</param>
        /// <returns></returns>
        public static bool SetIndex(string indexName, IEnumerable<LnModel> models)
        {
            InvokeWriterWithLock(indexName, (writer) =>
            {
                foreach (var model in models)
                {
                    writer.DeleteDocuments(model.GetTerm());
                    writer.AddDocument(model.ToDocument());
                }
            });
            return true;
        }

        /// <summary>
        /// 删除索引的全部记录
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <returns></returns>
        public static bool DeleteIndex(string indexName)
        {
            InvokeWriterWithLock(indexName, (writer) =>
            {
                writer.DeleteAll();
            });
            return true;
        }

        /// <summary>
        /// 删除索引的记录
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="idFields">记录的Id字段</param>
        /// <returns></returns>
        public static bool DeleteIndex(string indexName, IEnumerable<LnField> idFields)
        {
            InvokeWriterWithLock(indexName, (writer) =>
            {
                writer.DeleteDocuments(idFields.Select(item => item.ToTerm()).ToArray());
            });
            return true;
        }

        /// <summary>
        /// 搜索索引
        /// </summary>
        /// <param name="q">查询对象</param>
        /// <returns></returns>
        public static LnPage SearchIndex(LnQuery q)
        {
            if (ExistsIndex(q.IndexName) == false)
            {
                return new LnPage { Models = new LnModel[0] };
            }

            return InvokeReaderNoLock(q.IndexName, (reader) =>
            {
                using (var searcher = new IndexSearcher(reader))
                {
                    var matchFields = q.MatchFields == null ? null : q.MatchFields.Select(item => item.Name).ToArray();
                    var parser = new MultiFieldQueryParser(LnVersion.LUCENE_30, matchFields, _analyzer);

                    var query = parser.Parse(q.Keywords);

                    Sort sort = null;
                    if (q.SortFields != null && q.SortFields.Length > 0)
                    {
                        sort = new Sort(q.SortFields.Select(f => f.ToSortField()).ToArray());
                    }

                    var docs = sort == null ? searcher.Search(query, null, q.MaxSearchCount) : searcher.Search(query, null, q.MaxSearchCount, sort);
                    var models = docs.ScoreDocs.Skip(q.Skip).Take(q.Take).Select(item => DocToLnModel(searcher.Doc(item.Doc))).ToArray();

                    foreach (var model in models)
                    {
                        model.SetHighLight(q.Keywords, q.MatchFields);
                    }
                    return new LnPage
                    {
                        Models = models,
                        TotalCount = docs.TotalHits
                    };
                }
            });
        }

        /// <summary>
        /// Document对象转换为LnModel
        /// </summary>
        /// <param name="doc">Document</param>
        /// <returns></returns>
        private static LnModel DocToLnModel(Document doc)
        {
            var fields = doc.GetFields().Select(field => new LnField { Name = field.Name, Value = field.StringValue });
            return new LnModel(fields);
        }
    }
}
