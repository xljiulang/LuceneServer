### 如何使用？
#### 部署Lucene服务器
编译LuceneServer项目，直接运行LuceneServer.exe即可，可以在LuceneServer.exe.config更改一些配置项。

#### 全文检索相关
引用LuceneLib项目的LuceneLib.dll和NetworkSocket.dll，所有操作方法都在LuceneLib.Lucene<T>类中，Lucene<T>可以使用单例模式或多个实例，每个方法都是线程安全，所有API都是异步的，一般情况下使用长连接的单例模式即可。
```
var client = new Lucene<News>("MyIndex");
var news = client
    .SearchIndex("关键字1 关键字2")
    .MatchField(item => item.Title, "red")
    .MatchField(item => item.Content, Color.Red, 100)
    .OrderByDescending(item => item.CreateTime)
    .Skip(0)
    .Take(10)
    .ToPage();
```

#### Lucene<T>的T实例是如何存储到服务的？
T类型必须要求有一个叫id的唯一标识属性(不分大小写)，以作删除使用，Model的属性中类型为string、decimal、DateTime、Guid、枚举类型和基础类型的属性会被保存到服务器，其它类型比如自定义的class、集合数组等的属性和带有[NoneIndex]特性标注的属性会被忽略，且只有string类型的属性才能通过关键字进行检索。

#### 一些相关博文
[http://www.cnblogs.com/kewei/p/4522974.html](http://www.cnblogs.com/kewei/p/4522974.html)
