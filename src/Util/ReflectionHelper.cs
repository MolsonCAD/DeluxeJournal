using System.Reflection;

namespace DeluxeJournal.Util
{
    public class ReflectionHelper
    {
        public static FieldInfo TryGetField<T>(string name, BindingFlags flags)
        {
            return TryGetField(typeof(T), name, flags);
        }

        public static FieldInfo TryGetField(Type type, string name, BindingFlags flags)
        {
            if (type.GetField(name, flags) is not FieldInfo field)
            {
                throw new InvalidOperationException(string.Format("Failed to get field {0}.{1} using reflection.", type.FullName, name));
            }

            return field;
        }

        public static PropertyInfo TryGetProperty<T>(string name, BindingFlags flags)
        {
            return TryGetProperty(typeof(T), name, flags);
        }

        public static PropertyInfo TryGetProperty(Type type, string name, BindingFlags flags)
        {
            if (type.GetProperty(name, flags) is not PropertyInfo property)
            {
                throw new InvalidOperationException(string.Format("Failed to get property {0}.{1} using reflection.", type.FullName, name));
            }

            return property;
        }

        public static MethodInfo TryGetMethod<T>(string name, BindingFlags flags)
        {
            return TryGetMethod(typeof(T), name, flags);
        }

        public static MethodInfo TryGetMethod(Type type, string name, BindingFlags flags)
        {
            if (type.GetMethod(name, flags) is not MethodInfo method)
            {
                throw new InvalidOperationException(string.Format("Failed to get method {0}.{1} using reflection.", type.FullName, name));
            }

            return method;
        }
    }
}
