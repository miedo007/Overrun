using System.Reflection;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class ReflectionUtils
    {
        public static T GetField<T>(object instance, string fieldName)
        {
            var type = instance.GetType();
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)field?.GetValue(instance);
        }
    
        public static T GetProperty<T>(object instance, string fieldName)
        {
            var type = instance.GetType();
            var field = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
            return (T)field?.GetValue(instance);
        }
    }
}
