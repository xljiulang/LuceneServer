### 如何使用？
#### 部署Lucene服务器
编译LuceneServer项目，直接运行LuceneServer.exe即可，可以在LuceneServer.exe.config更改一些配置项。

#### 全文检索相关
引用LuceneLib项目的LuceneLib.dll和NetworkSocket.dll，所有操作方法都在LuceneLib.Lucene<T>类中，Lucene<T>可以使用单例模式或多个实例，每个方法都是线程安全，所有API都是异步的。


#### 一些相关博文
[http://www.cnblogs.com/kewei/p/4522974.html](http://www.cnblogs.com/kewei/p/4522974.html)
