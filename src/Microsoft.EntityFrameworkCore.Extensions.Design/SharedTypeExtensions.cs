using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    [DebuggerStepThrough]
    internal static class SharedTypeExtensions
    {
        private static readonly Dictionary<Type, object> _commonTypeDictionary = new Dictionary<Type, object>()
    {
      {
        typeof (int),
         0
      },
      {
        typeof (Guid),
         new Guid()
      },
      {
        typeof (DateTime),
         new DateTime()
      },
      {
        typeof (DateTimeOffset),
         new DateTimeOffset()
      },
      {
        typeof (long),
         0L
      },
      {
        typeof (bool),
         false
      },
      {
        typeof (double),
         0.0
      },
      {
        typeof (short),
         (short) 0
      },
      {
        typeof (float),
         0.0f
      },
      {
        typeof (byte),
         (byte) 0
      },
      {
        typeof (char),
         char.MinValue
      },
      {
        typeof (uint),
         0U
      },
      {
        typeof (ushort),
         (ushort) 0
      },
      {
        typeof (ulong),
         0UL
      },
      {
        typeof (sbyte),
         (sbyte) 0
      }
    };

        public static Type UnwrapNullableType(this Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if ((object)underlyingType != null)
                return underlyingType;
            return type;
        }

        public static bool IsNullableType(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsValueType)
                return true;
            if (typeInfo.IsGenericType)
                return typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
            return false;
        }

        public static bool IsValidEntityType(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static Type MakeNullable(this Type type, bool nullable = true)
        {
            if (IsNullableType(type) == nullable)
                return type;
            if (!nullable)
                return UnwrapNullableType(type);
            return typeof(Nullable<>).MakeGenericType(type);
        }

        public static bool IsNumeric(this Type type)
        {
            type = UnwrapNullableType(type);
            if (!IsInteger(type) && !(type == typeof(decimal)) && !(type == typeof(float)))
                return type == typeof(double);
            return true;
        }

        public static bool IsInteger(this Type type)
        {
            type = UnwrapNullableType(type);
            if (!(type == typeof(int)) && !(type == typeof(long)) && (!(type == typeof(short)) && !(type == typeof(byte))) && (!(type == typeof(uint)) && !(type == typeof(ulong)) && (!(type == typeof(ushort)) && !(type == typeof(sbyte)))))
                return type == typeof(char);
            return true;
        }

        public static bool IsSignedInteger(this Type type)
        {
            if (!(type == typeof(int)) && !(type == typeof(long)) && !(type == typeof(short)))
                return type == typeof(sbyte);
            return true;
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type.Name.StartsWith("<>") && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
                return type.Name.Contains("AnonymousType");
            return false;
        }

        public static bool IsTupleType(this Type type)
        {
            if (type == typeof(Tuple))
                return true;
            if (type.IsGenericType)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Tuple<>) || genericTypeDefinition == typeof(Tuple<,>) || (genericTypeDefinition == typeof(Tuple<,,>) || genericTypeDefinition == typeof(Tuple<,,,>)) || (genericTypeDefinition == typeof(Tuple<,,,,>) || genericTypeDefinition == typeof(Tuple<,,,,,>) || (genericTypeDefinition == typeof(Tuple<,,,,,,>) || genericTypeDefinition == typeof(Tuple<,,,,,,,>))) || genericTypeDefinition == typeof(Tuple<,,,,,,,>))
                    return true;
            }
            return false;
        }

        public static PropertyInfo GetAnyProperty(this Type type, string name)
        {
            List<PropertyInfo> list = type.GetRuntimeProperties().Where<PropertyInfo>(p => p.Name == name).ToList<PropertyInfo>();
            if (list.Count > 1)
                throw new AmbiguousMatchException();
            return list.SingleOrDefault<PropertyInfo>();
        }

        public static bool IsInstantiable(this Type type)
        {
            return IsInstantiable(type.GetTypeInfo());
        }

        private static bool IsInstantiable(TypeInfo type)
        {
            if (type.IsAbstract || type.IsInterface)
                return false;
            if (type.IsGenericType)
                return !type.IsGenericTypeDefinition;
            return true;
        }

        public static bool IsGrouping(this Type type)
        {
            return IsGrouping(type.GetTypeInfo());
        }

        private static bool IsGrouping(TypeInfo type)
        {
            if (!type.IsGenericType)
                return false;
            if (!(type.GetGenericTypeDefinition() == typeof(IGrouping<,>)))
                return type.GetGenericTypeDefinition() == typeof(IAsyncGrouping<,>);
            return true;
        }

        public static Type UnwrapEnumType(this Type type)
        {
            bool flag = IsNullableType(type);
            Type type1 = flag ? UnwrapNullableType(type) : type;
            if (!type1.GetTypeInfo().IsEnum)
                return type;
            Type underlyingType = Enum.GetUnderlyingType(type1);
            if (!flag)
                return underlyingType;
            return MakeNullable(underlyingType, true);
        }



        public static Type TryGetElementType(this Type type, Type interfaceOrBaseType)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition)
                return null;
            IEnumerable<Type> typeImplementations = GetGenericTypeImplementations(type, interfaceOrBaseType);
            Type type1 = null;
            foreach (Type type2 in typeImplementations)
            {
                if (type1 == null)
                {
                    type1 = type2;
                }
                else
                {
                    type1 = null;
                    break;
                }
            }
            if ((object)type1 == null)
                return null;
            return ((IEnumerable<Type>)type1.GetTypeInfo().GenericTypeArguments).FirstOrDefault<Type>();
        }

        public static IEnumerable<Type> GetGenericTypeImplementations(
          this Type type,
          Type interfaceOrBaseType)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericTypeDefinition)
            {
                foreach (Type type1 in interfaceOrBaseType.GetTypeInfo().IsInterface ? typeInfo.ImplementedInterfaces : GetBaseTypes(type))
                {
                    if (type1.GetTypeInfo().IsGenericType && type1.GetGenericTypeDefinition() == interfaceOrBaseType)
                        yield return type1;
                }
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == interfaceOrBaseType)
                    yield return type;
            }
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            for (type = type.GetTypeInfo().BaseType; type != null; type = type.GetTypeInfo().BaseType)
                yield return type;
        }

        public static IEnumerable<Type> GetTypesInHierarchy(this Type type)
        {
            for (; type != null; type = type.GetTypeInfo().BaseType)
                yield return type;
        }

        public static ConstructorInfo GetDeclaredConstructor(
          this Type type,
          Type[] types)
        {
            types = types ?? Array.Empty<Type>();
            return type.GetTypeInfo().DeclaredConstructors.SingleOrDefault<ConstructorInfo>(c =>
           {
               if (!c.IsStatic)
                   return ((IEnumerable<ParameterInfo>)c.GetParameters()).Select<ParameterInfo, Type>(p => p.ParameterType).SequenceEqual<Type>(types);
               return false;
           });
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(
          this Type type,
          string name)
        {
        label_2:
            TypeInfo typeInfo = type.GetTypeInfo();
            foreach (PropertyInfo declaredProperty in typeInfo.DeclaredProperties)
            {
                if (declaredProperty.Name.Equals(name, StringComparison.Ordinal))
                {
                    MethodInfo methodInfo = declaredProperty.GetMethod;
                    if ((object)methodInfo == null)
                        methodInfo = declaredProperty.SetMethod;
                    if (!methodInfo.IsStatic)
                        yield return declaredProperty;
                }
            }
            type = typeInfo.BaseType;
            typeInfo = null;
            if (type != null)
                goto label_2;
        }

        public static IEnumerable<MemberInfo> GetMembersInHierarchy(this Type type)
        {
        label_1:
            foreach (MemberInfo memberInfo in type.GetRuntimeProperties().Where<PropertyInfo>(pi =>
           {
               MethodInfo methodInfo = pi.GetMethod;
               if ((object)methodInfo == null)
                   methodInfo = pi.SetMethod;
               return !methodInfo.IsStatic;
           }))
                yield return memberInfo;
            foreach (MemberInfo memberInfo in type.GetRuntimeFields().Where<FieldInfo>(f => !f.IsStatic))
                yield return memberInfo;
            type = type.BaseType;
            if (type != null)
                goto label_1;
        }

        public static IEnumerable<MemberInfo> GetMembersInHierarchy(
          this Type type,
          string name)
        {
            return GetMembersInHierarchy(type).Where<MemberInfo>(m => m.Name == name);
        }

        public static object GetDefaultValue(this Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
                return null;
            if (!_commonTypeDictionary.TryGetValue(type, out object obj))
                return Activator.CreateInstance(type);
            return obj;
        }

        public static IEnumerable<TypeInfo> GetConstructibleTypes(
          this Assembly assembly)
        {
            return GetLoadableDefinedTypes(assembly).Where<TypeInfo>(t =>
           {
               if (!t.IsAbstract)
                   return !t.IsGenericTypeDefinition;
               return false;
           });
        }

        public static IEnumerable<TypeInfo> GetLoadableDefinedTypes(
          this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ((IEnumerable<Type>)ex.Types).Where<Type>(t => t != null).Select<Type, TypeInfo>(new Func<Type, TypeInfo>(IntrospectionExtensions.GetTypeInfo));
            }
        }
    }
}
