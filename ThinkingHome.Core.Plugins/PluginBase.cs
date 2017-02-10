﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NLog;
using ThinkingHome.Core.Plugins.Utils;

namespace ThinkingHome.Core.Plugins
{
    public abstract class PluginBase
    {
        #region properties

        [Import("DCCEE19A-2CEA-423F-BFE5-AE5E12679938")]
        public IServiceContext Context { get; set; }

        protected Logger Logger2 { get; }

        #endregion

        #region life cycle

        protected PluginBase()
        {
            Logger2 = LogManager.GetLogger(GetType().FullName);
        }

        public virtual void InitPlugin(IConfigurationSection config)
        {

        }

        public virtual void StartPlugin()
        {

        }

        public virtual void StopPlugin()
        {

        }

        #endregion

        #region find methods

        public PluginMethodInfo<TAttribute, TDelegate>[] FindMethodsByAttribute<TAttribute, TDelegate>()
            where TAttribute: Attribute where TDelegate : class
        {
            return GetType()
                .GetMethods()
                .SelectMany(GetMethodAttributes<TAttribute>)
                .Select(GetPluginMethodInfo<TAttribute, TDelegate>)
                .ToArray();
        }

        private static IEnumerable<Tuple<MethodInfo, TAttribute>> GetMethodAttributes<TAttribute>(MethodInfo method)
            where TAttribute: Attribute
        {
            return method
                .GetCustomAttributes<TAttribute>()
                .Select(attr => new Tuple<MethodInfo, TAttribute>(method, attr));
        }

        private PluginMethodInfo<TAttribute, TDelegate> GetPluginMethodInfo<TAttribute, TDelegate>(
            Tuple<MethodInfo, TAttribute> obj)
            where TAttribute: Attribute where TDelegate : class
        {
            var delegateType = typeof(TDelegate);

            if (delegateType == typeof(Delegate))
            {
                delegateType = obj.Item1.GetDelegateType();
            }

            var mthodDelegate = obj.Item1.IsStatic
                ? obj.Item1.CreateDelegate(delegateType)
                : obj.Item1.CreateDelegate(delegateType, this);

            return new PluginMethodInfo<TAttribute, TDelegate>(obj.Item2, mthodDelegate as TDelegate);
        }

        #endregion

        public void SafeInvoke<T>(T handler, Action<T> action, bool async = false)
        {
            if (handler == null) return;

            var context = new EventContext<T>(handler, action, Logger2);
            context.Invoke(async);
        }
    }
}
