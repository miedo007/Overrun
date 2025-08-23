using System;
using JetBrains.Annotations;

namespace Mtl.Injection
{
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        public string Id { get; }

        public InjectAttribute() { }

        public InjectAttribute(string id)
        {
            Id = id;
        }
    }
}