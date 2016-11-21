using System;
using System.Linq;

using Windows.Storage;

using Tracing;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : SettingsServiceBase
    {
        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry)
            : base(tracer, converterRegistry)
        {
        }

        private static ApplicationDataContainer AppSettings
        {
            get
            {
                return ApplicationData.Current.LocalSettings;
            }
        }

        protected override object GetValueOrDefaultFunction<T>(string key, T defaultValue)
        {
            if (AppSettings.Values.ContainsKey(key))
            {
                return AppSettings.Values[key];
            }

            return defaultValue;
        }

        protected override void AddOrUpdateValueFunction<T>(string key, T value)
        {
            if (!AppSettings.Values.ContainsKey(key))
            {
                AppSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
            }

            AppSettings.Values[key] = value;
        }

        protected override bool IsNativeType(Type type)
        {
            return WindowsRuntimeTypes.Contains(type);
        }

        private static readonly Type[] WindowsRuntimeTypes =
        {
            typeof(bool),
            typeof(float),
            typeof(long),
            typeof(byte),
            typeof(char),
            typeof(DateTimeOffset),
            typeof(double),
            typeof(Guid),
            typeof(short),
            typeof(ushort),
            typeof(string),
            typeof(uint),
            typeof(ulong),
            typeof(int)
        };
    }
}