using System;
using System.Reflection;

namespace HAMMER.Nails.Extensions
{
    //https://gist.github.com/2464327
    public static class ExtensionMethods
    {
        public static PropertyInfo GetProperty(this Type type, String propertyName)
        {
            return type.GetTypeInfo().GetDeclaredProperty(propertyName);
        }

        public static MethodInfo GetMethod(this Type type, String methodName)
        {
            return type.GetTypeInfo().GetDeclaredMethod(methodName);
        }

        public static bool IsSubclassOf(this Type type, Type parentType)
        {
            return type.GetTypeInfo().IsSubclassOf(parentType);
        }

        public static bool IsAssignableFrom(this Type type, Type parentType)
        {
            return type.GetTypeInfo().IsAssignableFrom(parentType.GetTypeInfo());
        }

        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.GetTypeInfo().IsPrimitive;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static object GetPropertyValue(this Object instance, string propertyValue)
        {
            return instance.GetType().GetTypeInfo().GetDeclaredProperty(propertyValue).GetValue(instance);
        }

        public static TypeInfo GetTypeInfo(this Type type)
        {
            var reflectableType = (IReflectableType)type;
            return reflectableType.GetTypeInfo();
        }
    }
}
