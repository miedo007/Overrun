using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mtl.Injection
{
    internal class InjectTypeCache
    {
        private readonly List<InjectFieldInfo> fieldInfoBuffer = new List<InjectFieldInfo>();
        private readonly Dictionary<Type, InjectFieldInfo[]> fieldInfoCache = new Dictionary<Type, InjectFieldInfo[]>();

        public IEnumerable<InjectFieldInfo> GetInjectFieldInfos(Type type)
        {
            if (fieldInfoCache.TryGetValue(type, out var fieldInfos))
            {
                return fieldInfos;
            }

            var parentType = type;
            fieldInfoBuffer.Clear();
            while (parentType != null)
            {
                var typeFieldInfos = parentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var typeFieldInfo in typeFieldInfos)
                {
                    var injectAttribute = typeFieldInfo.GetCustomAttribute<InjectAttribute>();
                    if (injectAttribute != null)
                    {
                        fieldInfoBuffer.Add(new InjectFieldInfo(typeFieldInfo, injectAttribute));
                    }
                }

                parentType = parentType.BaseType;
            }

            fieldInfos = fieldInfoBuffer.ToArray();
            fieldInfoCache.Add(type, fieldInfos);
            return fieldInfos;
        }
    }

    internal class InjectFieldInfo
    {
        public readonly FieldInfo FieldInfo;
        public readonly InjectAttribute InjectAttribute;

        public InjectFieldInfo(FieldInfo fieldInfo, InjectAttribute injectAttribute)
        {
            FieldInfo = fieldInfo;
            InjectAttribute = injectAttribute;
        }
    }
}