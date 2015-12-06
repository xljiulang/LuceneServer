using NetworkSocket;
using NetworkSocket.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LuceneLib
{
    /// <summary>
    /// 表示属性
    /// </summary>
    internal class LnProperty
    {
        /// <summary>
        /// 属性的Get委托
        /// </summary>
        private ApiAction geter;

        /// <summary>
        /// 属性的Set委托
        /// </summary>
        private ApiAction seter;

        /// <summary>
        /// 获取属性名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取是否为string类型
        /// </summary>
        public bool IsString { get; private set; }

        /// <summary>
        /// 获取属性的类型
        /// </summary>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// 获取属性的排序类型
        /// </summary>
        public LnSortType SortType { get; private set; }

        /// <summary>
        /// Get属性
        /// </summary>
        /// <param name="property">属性</param>
        public LnProperty(PropertyInfo property)
        {
            this.Name = property.Name;
            this.PropertyType = property.PropertyType;
            this.IsString = property.PropertyType == typeof(string);

            this.SortType = GetLnSortType(property.PropertyType);
            this.geter = new ApiAction(property.GetGetMethod());
            this.seter = new ApiAction(property.GetSetMethod());
        }

        /// <summary>
        /// 获取属性的值并转换为字符串
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public string GetValue(object instance)
        {
            var value = this.geter.Execute(instance);
            if (value == null)
            {
                return null;
            }

            // 枚举转换为整型的字符串
            if (this.PropertyType.IsEnum == true)
            {
                return value.GetHashCode().ToString();
            }
            return value.ToString();
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="value">属性值</param>
        public void SetValue(object instance, string value)
        {
            var castValue = Converter.Cast(value, this.PropertyType);
            this.seter.Execute(instance, castValue);
        }

        /// <summary>
        /// 排序字段类型转换
        /// </summary>
        /// <param name="type">字段类型</param>
        /// <returns></returns>
        private static LnSortType GetLnSortType(Type type)
        {
            if (type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime))
            {
                return LnSortType.STRING;
            }
            if (type == typeof(double))
            {
                return LnSortType.DOUBLE;
            }
            if (type == typeof(float) || type == typeof(decimal))
            {
                return LnSortType.FLOAT;
            }
            if (type == typeof(short) || type == typeof(ushort))
            {
                return LnSortType.SHORT;
            }
            if (type == typeof(Int32) || type == typeof(uint) || type.IsEnum)
            {
                return LnSortType.INT;
            }
            if (type == typeof(Int64) || type == typeof(ulong))
            {
                return LnSortType.LONG;
            }
            if (type == typeof(byte))
            {
                return LnSortType.BYTE;
            }
            return LnSortType.SCORE;
        }         

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
