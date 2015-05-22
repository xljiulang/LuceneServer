using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示属性不作索引存储
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class NoneIndexAttribute : Attribute
    {
    }
}
