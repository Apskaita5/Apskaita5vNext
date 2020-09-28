using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apskaita5.Common.ReflectionExtensions
{
    public static class ReflectionExtensions
    {

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda) where TSource : class
        {
            return GetPropertyInfo(propertyLambda);
        }

        public static TAttribute GetCustomAttribute<TAttribute, TSource, TProperty>(this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda) where TSource : class where TAttribute : Attribute
        {
            return GetCustomAttribute<TAttribute, TSource, TProperty>(propertyLambda);
        }

        public static string GetEnumDisplayValue<T>(this T value)
        {
            return value.GetEnumDisplayProperty(a => a.Name, v =>
            {
                if (null == value) return string.Empty;
                return value.ToString();
            });
        }

        public static string GetEnumDisplayShortValue<T>(this T value)
        {
            return value.GetEnumDisplayProperty(a => a.ShortName, v =>
            {
                if (null == value) return string.Empty;
                return value.ToString();
            });
        }

        public static string GetEnumDescription<T>(T value)
        {
            return value.GetEnumDisplayProperty(a => a.Description, v => string.Empty);
        }



        private static readonly ConcurrentDictionary<string, Attribute> _attributeCache 
            = new ConcurrentDictionary<string, Attribute>();

        private static TAttribute GetCustomAttribute<TAttribute, TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda) where TAttribute : Attribute
        {
            var baseProp = GetPropertyInfo<TSource, TProperty>(propertyLambda);

            var cacheKey = string.Format("{0}:{1}:{2}", typeof(TSource).FullName, 
                baseProp.Name, typeof(TAttribute).FullName);

            return (TAttribute)_attributeCache.GetOrAdd(cacheKey,
                s => GetCustomAttribute<TAttribute>(baseProp));
        }

        private static TAttribute GetCustomAttribute<TAttribute>(
            PropertyInfo baseProp) where TAttribute : Attribute
        {
            foreach (var prop in GetInheritedProperties(baseProp))
            {
                var result = prop.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];
                if (null != result && result.Length > 0) return result[0];
            }

            foreach (var prop in GetInterfaceProperties(baseProp))
            {
                var result = prop.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];
                if (null != result && result.Length > 0) return result[0];
            }

            return null;
        }

        private static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        private static List<PropertyInfo> GetInheritedProperties(PropertyInfo prop)
        {
            var result = new List<PropertyInfo>() { prop };
            if (null != prop.DeclaringType?.BaseType)
            {
                foreach (var inheritedProp in prop.DeclaringType.BaseType.GetProperties()
                    .Where(p => p.Name == prop.Name))
                {
                    result.AddRange(GetInheritedProperties(inheritedProp));
                }
            }
            return result;
        }

        private static List<PropertyInfo> GetInterfaceProperties(PropertyInfo prop)
        {
            var result = new List<PropertyInfo>();

            if (null != prop.DeclaringType)
            {
                foreach (var implInterface in prop.DeclaringType.GetInterfaces())
                {
                    result.AddRange(GetInterfaceProperties(implInterface, prop.Name));
                }
            }

            return result;
        }

        private static List<PropertyInfo> GetInterfaceProperties(Type interfaceType, string propName)
        {
            var result = new List<PropertyInfo>(interfaceType.GetProperties().Where(p => p.Name == propName));

            if (null != interfaceType.BaseType) 
                result.AddRange(GetInterfaceProperties(interfaceType.BaseType, propName));

            return result;
        }

        private static string GetEnumDisplayProperty<T>(this T value, Func<DisplayAttribute, string> propGetter, 
            Func<T, string> defaultValueGetter)
        {
            Type enumType = null;
            if (typeof(T).IsEnum) enumType = typeof(T);
            if (null == enumType) enumType = Nullable.GetUnderlyingType(typeof(T));
            if (null == enumType || !enumType.IsEnum) throw new InvalidOperationException(
                "Method GetDescriptionAttributeForEnumValue is only applicable for Enum types.");

            if (null == value) return defaultValueGetter(value);

            var fieldInfo = enumType.GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (null == descriptionAttributes || descriptionAttributes.Length < 1 ||
                propGetter(descriptionAttributes[0]).IsNullOrWhiteSpace())
                return defaultValueGetter(value);

            if (null != descriptionAttributes[0].ResourceType)
                return LookupResource(descriptionAttributes[0].ResourceType, 
                    propGetter(descriptionAttributes[0]));

            return propGetter(descriptionAttributes[0]);
        }

        private static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    var resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

    }
}
