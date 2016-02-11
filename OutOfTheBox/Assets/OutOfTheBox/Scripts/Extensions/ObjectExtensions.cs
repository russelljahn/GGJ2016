using System;
using System.Collections.Generic;
using System.Reflection;


namespace Sense.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Always use this over the == operator when checking if anything that derives from UnityEngine.Object is null! UnityEngine.Object overrides the == operator, which leads to unexpected results when checking if an instance is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T obj)
        {
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }


        /// <summary>
        /// Always use this over the != operator when checking if anything that derives from UnityEngine.Object is not null! UnityEngine.Object overrides the != operator, which leads to unexpected results when checking if an instance is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(this T obj)
        {
            return !obj.IsNull();
        }

        public static T DeepCopy<T>(this T obj)
        {
            return (T)(DeepCopy((object)obj));
        }

        /// <summary>
        /// Deep copy implementation from: http://stackoverflow.com/questions/4226747/deep-copy-of-listt
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepCopy(this object obj)
        {
            if (obj.IsNull())
            {
                return null;
            }

            var type = obj.GetType();
            if (type.IsValueType || type == typeof (string))
            {
                return obj;
            }
            if (type.IsArray)
            {
                var elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = (Array)obj;
                var copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; ++i)
                {
                    copied.SetValue(DeepCopy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            if (type.IsClass)
            {
                var instance = Activator.CreateInstance(obj.GetType());
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                    {
                        continue;
                    }
                    field.SetValue(instance, DeepCopy(fieldValue));
                }
                return instance;
            }
            throw new ArgumentException("Unknown type!");
        }

    }
}
