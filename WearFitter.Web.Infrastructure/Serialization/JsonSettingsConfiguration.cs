using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WearFitter.Web.Infrastructure.Serialization;

public static class JsonSettingsConfiguration
{
    private static JsonSerializerSettings? _settings;
    private readonly static Lock _locker = new();

    public static void Configure(JsonSerializerSettings settings)
    {
        settings.Converters.Add(new StringEnumConverter());
        settings.TypeNameHandling = TypeNameHandling.Auto;
        settings.SerializationBinder = new KnownTypesSerializationBinder();
        settings.NullValueHandling = NullValueHandling.Ignore;
    }

    public static JsonSerializerSettings Get()
    {
        if (_settings == null)
        {
            lock (_locker)
            {
                if (_settings == null)
                {
                    _settings = new JsonSerializerSettings();
                    Configure(_settings);
                }
            }
        }

        return _settings;
    }
}