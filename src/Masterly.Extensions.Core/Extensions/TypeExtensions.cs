using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace System
{
    public static class TypeExtensions
    {
        public static Type[] GetInterfacesAndAbstractClasses(this Type type)
        {
            if (type.BaseType == null)
                return new Type[0];

            var baseTypes = new List<Type>(type.GetInterfaces());
            var currentType = type;

            while ((currentType = currentType.BaseType) != null)
            {
                if (currentType.IsInterface || currentType.IsAbstract)
                    baseTypes.Add(currentType);
            }

            return baseTypes.ToArray();
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || genericType == null)
            {
                return false;
            }

            return givenType == genericType
              || givenType.MapsToGenericTypeDefinition(genericType)
              || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
              || givenType.BaseType.IsAssignableToGenericType(genericType);
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return givenType
              .GetInterfaces()
              .Where(x => x.IsGenericType)
              .Any(x => x.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return genericType.IsGenericTypeDefinition
              && givenType.IsGenericType
              && givenType.GetGenericTypeDefinition() == genericType;
        }

        /// <summary>
        /// Determines whether an instance of this type can be assigned to
        /// an instance of the <typeparamref name="TTarget"></typeparamref>.
        ///
        /// Internally uses <see cref="Type.IsAssignableFrom"/>.
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam> (as reverse).
        public static bool IsAssignableTo<TTarget>([NotNull] this Type type)
        {
            Guard.Against.Null(type, nameof(type));

            return typeof(TTarget).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether an instance of this type can be assigned to
        /// an instance of the <typeparamref name="TTarget"></typeparamref>.
        ///
        /// Internally uses <see cref="Type.IsAssignableFrom"/>.
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam> (as reverse).
        public static bool IsAssignableTo([NotNull] this Type type, [NotNull] Type targetType)
        {
            Guard.Against.Null(type, nameof(type));

            return targetType.IsAssignableFrom(type);
        }

        public static string GetFullNameWithAssemblyName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }

        public static bool IsComplex(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsComplex(typeInfo.GetGenericArguments()[0]);
            }
            return !(typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(DateTime))
              || type.Equals(typeof(Guid))
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal)));
        }
    }
}