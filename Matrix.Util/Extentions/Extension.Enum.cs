using System;
using System.Reflection;


namespace Matrix.Util.Extentions
{
    /// <summary>
    ///     枚举扩展方法类
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionForEnum(this object value)
        {
            try
            {
                if (value == null) return string.Empty;
                var type = value.GetType();
                var field = type.GetField(Enum.GetName(type, value));

                if (field == null) return value.ToString();

                var des = CustomAttributeData.GetCustomAttributes(type.GetMember(field.Name)[0]);

                return des.AnyOne() && des[0].ConstructorArguments.AnyOne()
                    ? des[0].ConstructorArguments[0].Value.ToString()
                    : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将字符串转换成枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">字符串值</param>
        /// <param name="defaultValue">默认的枚举值</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue)
            where T : struct, System.Enum
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转换成枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">字符串值</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
            where T : struct, System.Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// 尝试将字符串转换成枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举字符串值</param>
        /// <returns>如果转换失败，则返回null</returns>
        public static Nullable<T> TryToEnum<T>(this string value)
            where T : struct, System.Enum
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (Enum.TryParse(value, out T result))
                return result;
            return null;
            //return Enum.TryParse<T>(value, true, out result) ? result : null;

        }
    }
}