using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace WearFitter.Web.Infrastructure.Serialization
{
    public class KnownTypesSerializationBinder : ISerializationBinder
    {
        private static Dictionary<string, Type> _knownTypes = new();
        private static Dictionary<string, Type> _knownFullTypes = new();

        public static IReadOnlyDictionary<string, Type> KnownTypes => _knownTypes;

        public static void AddAssembly(Assembly assembly)
        {
            var exportedTypes = assembly.GetExportedTypes();
            var typesToAdd = exportedTypes
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => !t.IsAssignableTo(typeof(Attribute)));

            foreach (var type in typesToAdd)
            {
                _knownFullTypes.TryAdd(type.FullName, type);
                _knownTypes.TryAdd(type.Name, type);
            }
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            Type t;
            bool isFullyQualifiedTypeName = typeName.Contains(".");
            if (isFullyQualifiedTypeName)
            {
                _knownFullTypes.TryGetValue(typeName, out t);
            }
            else
            {
                _knownTypes.TryGetValue(typeName, out t);
            }

            return t;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}
