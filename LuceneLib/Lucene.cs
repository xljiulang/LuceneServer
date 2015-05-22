using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSocket;
using NetworkSocket.Fast;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Net.Sockets;

namespace LuceneLib
{
    /// <summary>
    /// 表示Lucene操作对象
    /// 不可继承
    /// </summary>
    /// <typeparam name="T">模型类型</typeparam>
    [DebuggerDisplay("IndexName ={indexName}")]
    public sealed class Lucene<T> : IDisposable
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        private readonly string indexName;

        /// <summary>
        /// 模型的属性
        /// </summary>
        private readonly LnProperty[] properties;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// 客户端
        /// </summary>
        private FastTcpClient client = new FastTcpClient();


        /// <summary>
        /// 获取是否连接到远程端
        /// </summary>
        public Boolean IsConnected
        {
            get
            {
                return this.client.IsConnected;
            }
        }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return this.client.RemoteEndPoint;
            }
        }

        /// <summary>
        /// 获取或设置请求等待超时时间(毫秒)
        /// 默认30秒      
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Timeout
        {
            get
            {
                return this.client.TimeOut;
            }
            set
            {
                this.client.TimeOut = value;
            }
        }

        /// <summary>
        /// Lucene操作对象
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <exception cref="ArgumentNullException"></exception>   
        /// <exception cref="ArgumentException"></exception>
        public Lucene(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentNullException("indexName");
            }

            var supportTypes = new[] { typeof(string), typeof(DateTime), typeof(Guid), typeof(decimal) };
            var properties = typeof(T)
                .GetProperties()
                .Where(p => p.GetAccessors().Length == 2)
                .Where(p => p.PropertyType.IsPrimitive || p.PropertyType.IsEnum || supportTypes.Contains(p.PropertyType));

            if (properties.Any(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase)) == false)
            {
                throw new ArgumentException("类型" + typeof(T).Name + "必须声明get和set的Id属性，表示唯一标识符");
            }

            this.indexName = indexName;
            this.properties = properties.Select(p => new LnProperty(p)).ToArray();
        }

        /// <summary>
        /// 连接到远程终端
        /// </summary>
        /// <param name="ip">远程ip</param>
        /// <param name="port">远程端口</param>
        /// <returns></returns>
        public Task<Boolean> Connect(IPAddress ip, int port)
        {
            return this.client.Connect(ip, port);
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public Task<bool> Login(string account, string password)
        {
            return this.client.InvokeApi<bool>("Login", account, password);
        }

        /// <summary>
        /// 将业务模型转换为LN模型
        /// 用for以支持多线程遍历
        /// </summary>       
        /// <param name="model">业务模型</param>
        /// <returns></returns>
        private LnModel TModelToLnModel(T model)
        {
            var fields = new LnField[this.properties.Length];
            for (var i = 0; i < this.properties.Length; i++)
            {
                var property = this.properties[i];
                fields[i] = new LnField
                {
                    Name = property.Name,
                    Value = property.GetValue(model),
                    IsString = property.IsString
                };
            }
            return new LnModel { Fields = fields };
        }

        /// <summary>
        /// 创建或更新到索引   
        /// </summary>    
        /// <param name="model">模型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <returns></returns>
        public Task<bool> SetIndex(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            return this.SetIndex(new[] { model });
        }

        /// <summary>
        /// 创建或更新到索引 
        /// </summary>            
        /// <param name="models">模型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="SerializerException"></exception>
        /// <returns></returns>
        public Task<bool> SetIndex(IEnumerable<T> models)
        {
            if (models == null)
            {
                throw new ArgumentNullException("models");
            }
            var lnModels = models.Select(item => this.TModelToLnModel(item));
            return this.client.InvokeApi<bool>("SetIndex", indexName, lnModels);
        }

        /// <summary>
        /// 删除索引的全部记录
        /// </summary>
        /// <exception cref="SocketException"></exception>       
        /// <returns></returns>
        public Task<bool> DeleteIndex()
        {
            return this.client.InvokeApi<bool>("DeleteAll", this.indexName);
        }

        /// <summary>
        /// 删除索引的记录
        /// </summary>        
        /// <param name="id">记录的id值</param>
        /// <exception cref="ArgumentNullException"></exception>     
        /// <exception cref="SocketException"></exception>       
        /// <returns></returns>
        public Task<bool> DeleteIndex(object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            return this.DeleteIndex(new[] { id });
        }

        /// <summary>
        /// 删除索引
        /// </summary>          
        /// <param name="ids">id值</param>
        /// <exception cref="ArgumentNullException"></exception>   
        /// <exception cref="SocketException"></exception>      
        /// <returns></returns>
        public Task<bool> DeleteIndex(IEnumerable<object> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException("ids");
            }
            var idValues = ids.Select(item => item.ToString());
            return this.client.InvokeApi<bool>("DeleteIndex", this.indexName, idValues);
        }

        /// <summary>
        /// 索引搜索
        /// </summary>   
        /// <param name="keywords">关键字</param>
        /// <returns></returns>
        public LnQuery<T> SearchIndex(string keywords)
        {
            return new LnQuery<T>(this.client, this.properties, this.indexName, keywords);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed == false)
            {
                this.isDisposed = true;
                ((IDisposable)this.client).Dispose();
            }
        }
    }
}
