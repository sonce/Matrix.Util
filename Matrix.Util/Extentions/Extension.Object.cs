using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Matrix.Util.Extentions
{
    /// <summary>
    ///     通用类型扩展方法类
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 获取对象的属性和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetKeyValue<T>(this T obj)
        {
            if (obj == null)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var val = prop.GetValue(obj, null);
                var name = prop.Name;

                sb.AppendFormat("{0}={1}\r\n", name, val);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 反射实现同一个类的对象之间相同属性的值的复制
        /// 适用于没有新建实体之间
        /// </summary>
        /// <param name="target">返回的实体</param>
        /// <param name="source">数据源实体</param>
        /// <returns></returns>
        public static TIn? CopyProperties<TIn>([NotNull] this TIn source, TIn target)
            where TIn : class
        {
            if (source == null)
                return null;
            var Types = source.GetType();//获得类型  
            var Typed = typeof(TIn);
            foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
            {
                foreach (PropertyInfo dp in Typed.GetProperties())
                {
                    if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                    {
                        dp.SetValue(target, sp.GetValue(source, null), null);//获得s对象属性的值复制给d对象的属性  
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// lambda表达式:t=>t.propName==propValue
        /// 多用于where条件
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ToExpression<T>(this T model) where T : class
        {
            var type = typeof(T);
            // 创建节点参数t
            ParameterExpression parameter = Expression.Parameter(type, "t");
            Expression<Func<T, bool>>? exps = null;
            var excludePropertyNames = new[]
            {
                "CreateTime",
                "ModifyTime",
                "IsDeleted",
                "PageIndex",
                "PageSize",
                "SortField",
                "SortType",
                "CompanyId",
                "Keywords"
            };
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var properties = type.GetProperties().Where(x => !excludePropertyNames.Contains(x.Name));
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(model);
                if (propertyValue != null)
                {
                    // 创建一个成员(字段/属性)
                    MemberExpression member = Expression.PropertyOrField(parameter, property.Name);
                    // 创建一个常数
                    ConstantExpression constant = Expression.Constant(propertyValue);
                    // 创建一个相等比较Expression
                    Expression callExp;
                    if (property.PropertyType == typeof(string))
                    {
                        callExp = Expression.Call(member, containsMethod, constant);
                    }
                    else
                    {
                        callExp = Expression.Equal(member, constant);
                    }
                    // 生成lambda表达式
                    var exp = Expression.Lambda<Func<T, bool>>(callExp, parameter);
                    exps = exps == null ? exp : exps.And(exp);
                }
            }
            return exps;
        }

        /// <summary>
        /// 将对象的属性转换成键值对
        /// </summary>
        /// <typeparam name="TValue">value属性值</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, TValue> ToDictionary<TValue>(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json); ;
            return dictionary ?? new Dictionary<string, TValue>();
        }
        private static Dictionary<string, PropertyInfo> cacheProperties = new Dictionary<string, PropertyInfo>();
        public static bool TryGetPropertyValue(this object obj, string propertyName, out object? value)
        {
            Type type = obj.GetType();
            string key = type.FullName + "_" + propertyName;
            if (!cacheProperties.TryGetValue(key, out var property))
            {
                property = type.GetProperty(propertyName, _bindingFlags);
                if (property != null)
                {
                    cacheProperties.TryAdd(key, property);
                }
            }
            value = property?.GetValue(obj);
            return property != null;
        }

        /// <summary>
        /// 是否为默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T value) where T : struct
        {
            bool isDefault = value.Equals(default(T));

            return isDefault;
        }

        /// <summary>
        /// Used to simplify and beautify casting an object to a type.
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.Type)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// Check if an item is in the given enumerable.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="items">Items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, IEnumerable<T> items)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Can be used to conditionally perform a function
        /// on an object and return the modified or the original object.
        /// It is useful for chained calls.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="condition">A condition</param>
        /// <param name="func">A function that is executed only if the condition is <code>true</code></param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Returns the modified object (by the <paramref name="func"/> if the <paramref name="condition"/> is <code>true</code>)
        /// or the original object if the <paramref name="condition"/> is <code>false</code>
        /// </returns>
        public static T If<T>(this T obj, bool condition, Func<T, T> func)
        {
            if (condition)
            {
                return func(obj);
            }

            return obj;
        }

        /// <summary>
        /// Can be used to conditionally perform an action
        /// on an object and return the original object.
        /// It is useful for chained calls on the object.
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="condition">A condition</param>
        /// <param name="action">An action that is executed only if the condition is <code>true</code></param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>
        /// Returns the original object.
        /// </returns>
        public static T If<T>(this T obj, bool condition, Action<T> action)
        {
            if (condition)
            {
                action(obj);
            }

            return obj;
        }
    }
}