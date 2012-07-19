using System;
using System.Linq;
using System.Reflection;

namespace HAMMER.Nails.Extensions
{
    //http://code.google.com/p/protobuf-net/source/browse/trunk/protobuf-net/Helpers.cs?r=485
    public static class TypeExtensions
    {
        public readonly static Type[] EmptyTypes = new Type[0];

        public static Type GetInterface(this Type t, string name)
        {
            var interfaces = t.GetTypeInfo().ImplementedInterfaces;
            return interfaces.FirstOrDefault(i => i.Name.Contains(name));
        }

        private static bool IsMatch(ParameterInfo[] parameters, Type[] parameterTypes)
        {
            if (parameterTypes == null) 
                parameterTypes = EmptyTypes;

            if (parameters.Length != parameterTypes.Length) 
                return false;

            return !parameters.Where((t, i) => t.ParameterType != parameterTypes[i]).Any();
        }

        public static ConstructorInfo GetConstructor(this Type t, Type[] parameterTypes, bool nonPublic = false)
        {
            var type = t.GetTypeInfo();
            foreach (ConstructorInfo ctor in type.DeclaredConstructors)
            {
                if (!nonPublic && !ctor.IsPublic) 
                    continue;

                if (IsMatch(ctor.GetParameters(), parameterTypes)) 
                    return ctor;
            }
            return null;
        }

        public static ConstructorInfo[] GetConstructors(TypeInfo typeInfo, bool nonPublic = false)
        {
            if (nonPublic) 
                return typeInfo.DeclaredConstructors.ToArray();
            return 
                typeInfo.DeclaredConstructors.Where(x => x.IsPublic).ToArray();
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }
    }
}