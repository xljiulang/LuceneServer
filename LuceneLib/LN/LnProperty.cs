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
        private Func<object, object[], object> geter;

        /// <summary>
        /// 属性的Set委托
        /// </summary>
        private Func<object, object[], object> seter;

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
        /// Get属性
        /// </summary>
        /// <param name="property">属性</param>
        public LnProperty(PropertyInfo property)
        {
            this.Name = property.Name;
            this.PropertyType = property.PropertyType;
            this.IsString = property.PropertyType == typeof(string);

            this.geter = CreateMethodInvoker(property.GetGetMethod());
            this.seter = CreateMethodInvoker(property.GetSetMethod());
        }

        /// <summary>
        /// 获取属性的值并转换为字符串
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public string GetValue(object instance)
        {
            var value = this.geter(instance, null);
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
            this.seter(instance, new object[] { this.CastStringValue(value) });
        }

        /// <summary>
        /// 将string类型值转换为属性类型的值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        private object CastStringValue(string value)
        {
            if (value == null)
            {
                return this.IsString ? null : Activator.CreateInstance(this.PropertyType);
            }

            if (this.IsString == true)
            {
                return value;
            }

            if (this.PropertyType.IsEnum == true)
            {
                return Enum.Parse(this.PropertyType, value, false);
            }

            if (this.PropertyType == typeof(Guid))
            {
                return Guid.Parse(value);
            }

            return ((IConvertible)value).ToType(this.PropertyType, null);
        }

        /// <summary>
        /// 生成方法的委托
        /// </summary>
        /// <param name="method">方法成员信息</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static Func<object, object[], object> CreateMethodInvoker(MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameters = Expression.Parameter(typeof(object[]), "parameters");

            var instanceCast = method.IsStatic ? null : Expression.Convert(instance, method.ReflectedType);
            var parametersCast = method.GetParameters().Select((p, i) =>
            {
                var parameter = Expression.ArrayIndex(parameters, Expression.Constant(i));
                return Expression.Convert(parameter, p.ParameterType);
            });

            var body = Expression.Call(instanceCast, method, parametersCast);

            if (method.ReturnType == typeof(void))
            {
                var action = Expression.Lambda<Action<object, object[]>>(body, instance, parameters).Compile();
                return (_instance, _parameters) =>
                {
                    action.Invoke(_instance, _parameters);
                    return null;
                };
            }
            else
            {
                var bodyCast = Expression.Convert(body, typeof(object));
                return Expression.Lambda<Func<object, object[], object>>(bodyCast, instance, parameters).Compile();
            }
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
