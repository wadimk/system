﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace ThinkingHome.Core.Plugins.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Сериализация в JSON
        /// </summary>
        public static string ToJson(this object obj, string defaultValue = "")
        {
            return obj == null ? defaultValue : JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Получить тип делегата для заданного метода
        /// </summary>
        public static Type GetDelegateType(this MethodInfo mi)
        {
            var types2 = mi.GetParameters()
                .Select(p => p.ParameterType)
                .Concat(new[] { mi.ReturnType });

            return Expression.GetDelegateType(types2.ToArray());
        }

        #region parse

        public static int? ParseInt(this string stringValue)
        {
            if (int.TryParse(stringValue, out var result))
            {
                return result;
            }

            return null;
        }

        public static Guid? ParseGuid(this string stringValue)
        {
            if (Guid.TryParse(stringValue, out var result))
            {
                return result;
            }

            return null;
        }

        public static bool? ParseBool(this string stringValue)
        {
            if (bool.TryParse(stringValue, out var result))
            {
                return result;
            }

            return null;
        }

        #endregion

        #region find attrs

        public static (TAttr Meta, TypeInfo Type)[] FindAttrs<TAttr>(this IEnumerable<PluginBase> plugins, Func<TAttr, bool> filter = null)
            where TAttr : Attribute
        {
            return plugins
                .SelectMany(p => p.FindAttrs<TAttr>())
                .ToArray();
        }

        public static (TAttr Meta, TypeInfo Type)[] FindAttrs<TAttr>(this PluginBase plugin, Func<TAttr, bool> filter = null) where TAttr : Attribute
        {
            var fn = filter ?? (a => true);

            var type = plugin.GetType().GetTypeInfo();

            return type
                .GetCustomAttributes<TAttr>()
                .Where(fn)
                .Select(a => (Meta: a, Type: type))
                .ToArray();
        }

        #endregion

        #region find methods

        public static (TAttr Meta, TDelegate Method)[] FindMethods<TAttr, TDelegate>(
            this IEnumerable<PluginBase> plugins) where TAttr: Attribute where TDelegate : class
        {
            return plugins
                .SelectMany(p => p.FindMethods<TAttr, TDelegate>())
                .ToArray();
        }

        public static (TAttr Meta, TDelegate Method)[] FindMethods<TAttr, TDelegate>(this PluginBase plugin)
            where TAttr: Attribute where TDelegate : class
        {
            IEnumerable<Tuple<MethodInfo, TAttr>> GetMethodAttributes(MethodInfo method)
            {
                return method
                    .GetCustomAttributes<TAttr>()
                    .Select(attr => new Tuple<MethodInfo, TAttr>(method, attr));
            }

            (TAttr Meta, TDelegate Method) GetPluginMethodInfo(Tuple<MethodInfo, TAttr> obj)
            {
                var delegateType = typeof(TDelegate);

                if (delegateType == typeof(Delegate))
                {
                    delegateType = obj.Item1.GetDelegateType();
                }

                var mthodDelegate = obj.Item1.IsStatic
                    ? obj.Item1.CreateDelegate(delegateType)
                    : obj.Item1.CreateDelegate(delegateType, plugin);

                return (obj.Item2, mthodDelegate as TDelegate);
            }

            return plugin
                .GetType()
                .GetTypeInfo()
                .GetMethods()
                .SelectMany(GetMethodAttributes)
                .Select(GetPluginMethodInfo)
                .ToArray();
        }

        #endregion

        #region to registry

        public static ObjectRegistry<T> ToRegistry<T, T2>(
            this IEnumerable<T2> collection, Func<T2, string> getKey, Func<T2, T> getValue)
        {
            var registry = new ObjectRegistry<T>();

            return collection.ToRegistry(registry, getKey, getValue);
        }

        public static ObjectRegistry<T> ToRegistry<T, T2>(
            this IEnumerable<T2> collection, ObjectRegistry<T> registry, Func<T2, string> getKey, Func<T2, T> getValue)
        {
            foreach (var item in collection)
            {
                var key = getKey(item);
                var value = getValue(item);
                registry.Register(key, value);
            }

            return registry;
        }

        #endregion
    }
}
