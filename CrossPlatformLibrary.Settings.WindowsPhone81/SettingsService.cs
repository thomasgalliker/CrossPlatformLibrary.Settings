using System;
using System.Globalization;
using CrossPlatformLibrary.Extensions;
using CrossPlatformLibrary.IO;
using CrossPlatformLibrary.Tracing;
using Guards;
using TypeConverter;
using Windows.Storage;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService // TODO GATH: Test & verify if feature-complete compared to other SettingsServices
    {
        private readonly object locker = new object();
        private readonly ITracer tracer;
        private readonly IConverterRegistry converterRegistry;

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry)
        {
            Guard.ArgumentNotNull(() => tracer);
            Guard.ArgumentNotNull(() => converterRegistry);

            this.tracer = tracer;
            this.converterRegistry = converterRegistry;
        }

        public IConverterRegistry ConverterRegistry
        {
            get
            {
                return this.converterRegistry;
            }
        }

        private static ApplicationDataContainer AppSettings
        {
            get
            {
                return ApplicationData.Current.LocalSettings;
            }
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNullOrEmpty(() => key);

            var typeOf = typeof(T);
            this.tracer.Debug("GetValueOrDefault for key={0}, type={1}.", key, typeOf.GetFormattedName());
            object value = defaultValue;

            lock (this.locker)
            {
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                if (IsWindowsRuntimeType(typeOf))
                {
                    // All of these types are supported by WinRT: https://msdn.microsoft.com/en-us/library/windows/apps/br205768.aspx
                    // The rest of it must be converted
                    value = this.GetValueOrDefaultFunction(key, defaultValue);
                }
                else if (typeOf == typeof(decimal))
                {
                    var savedDecimal = this.GetValueOrDefaultFunction<string>(key, null);
                    if (savedDecimal != null)
                    {
                        value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);
                    }
                }
                else if (typeOf == typeof(DateTime))
                {
                    var stringDateTime = this.GetValueOrDefaultFunction<string>(key, null);
                    if (stringDateTime != null)
                    {
                        var serializableDateTime = stringDateTime.DeserializeFromXml<SerializableDateTime>();
                        if (serializableDateTime != SerializableDateTime.Undefined)
                        {
                            value = new DateTime(serializableDateTime.Ticks, serializableDateTime.Kind);
                        }
                    }
                }
                else if (typeOf == typeof(Uri))
                {
                    string uriString = this.GetValueOrDefaultFunction<string>(key, null);
                    value = new Uri(uriString);
                }
                else
                {
                    var savedObject = this.GetValueOrDefaultFunction<string>(key, null);
                    if (savedObject != null)
                    {
                        value = Convert.ToString(savedObject).DeserializeFromXml<T>();
                    }
                }
            }

            return null != value ? (T)value : defaultValue;
        }

        private T GetValueOrDefaultFunction<T>(string key, T defaultValue)
        {
            T value = defaultValue;
            if (AppSettings.Values.ContainsKey(key))
            {
                var settingsValue = AppSettings.Values[key];
                if (settingsValue != null)
                {
                    value = this.converterRegistry.TryConvert(settingsValue, defaultValue);
                }
            }

            return value;
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            Guard.ArgumentNotNull(() => value);

            lock (this.locker)
            {
                var typeOf = typeof(T);
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                this.tracer.Debug("AddOrUpdateValue for key={0}, type={1}.", key, typeOf);

                if (IsWindowsRuntimeType(typeOf))
                {
                    this.AddOrUpdateFunction(key, value);
                }
                else if (typeOf == typeof(decimal))
                {
                    string decimalString = Convert.ToString(value, CultureInfo.InvariantCulture);
                    this.AddOrUpdateFunction(key, decimalString);
                }
                else if (typeOf == typeof(DateTime))
                {
                    var dateTime = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    var serializableDateTime = SerializableDateTime.FromDateTime(dateTime);
                    string stringDateTime = serializableDateTime.SerializeToXml(preserveTypeInformation: true);
                    this.AddOrUpdateFunction(key, stringDateTime);
                }
                else if (typeOf == typeof(Uri))
                {
                    string uriString = value.ToString();
                    this.AddOrUpdateFunction(key, uriString);
                }
                else
                {
                    string serializedString = value.SerializeToXml(preserveTypeInformation: true);
                    this.AddOrUpdateFunction(key, serializedString);
                }
            }
        }

        private static bool IsWindowsRuntimeType(Type type)
        {
            return
                   type == typeof(bool) ||
                   type == typeof(float) ||
                   type == typeof(long) ||
                   type == typeof(byte) ||
                   type == typeof(char) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(double) ||
                   type == typeof(Guid) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(string) ||
                   type == typeof(uint) ||
                   type == typeof(ulong) ||
                   type == typeof(int);
        }

        private void AddOrUpdateFunction<T>(string key, T value)
        {
            if (!AppSettings.Values.ContainsKey(key))
            {
                AppSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
            }

            AppSettings.Values[key] = value;
        }
    }
}