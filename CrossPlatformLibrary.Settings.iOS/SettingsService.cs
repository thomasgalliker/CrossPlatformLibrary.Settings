
using Guards;
using CrossPlatformLibrary.IO;

using System;
using System.Globalization;

using CrossPlatformLibrary.Extensions;
using CrossPlatformLibrary.Tracing;

using TypeConverter;

#if __UNIFIED__
using Foundation;
#else
using MonoTouch.Foundation;
#endif

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly object locker = new object();
        private readonly ITracer tracer;
        private readonly IConverterRegistry converterRegistry;
        private readonly NSUserDefaults defaults;

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry)
        {
            Guard.ArgumentNotNull(() => tracer);
            Guard.ArgumentNotNull(() => converterRegistry);

            this.tracer = tracer;
            this.converterRegistry = converterRegistry;

            this.defaults = NSUserDefaults.StandardUserDefaults;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNullOrEmpty(() => key);

            var typeOf = typeof(T);
            this.tracer.Debug("GetValueOrDefault for key={0}, type={1}.", key, typeOf.GetFormattedName());
            object value = defaultValue;

            lock (this.locker)
            {
                if (this.defaults.ValueForKey(new NSString(key)) == null)
                {
                    return defaultValue;
                }
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                if ((typeOf == typeof(string)) || (typeOf == typeof(double)) || (typeOf == typeof(float)) || (typeOf == typeof(Guid)) || (typeOf == typeof(bool)) || (typeOf == typeof(int))
                    || (typeOf == typeof(long)) || (typeOf == typeof(byte)))
                {
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

            var settingsValue = this.defaults.StringForKey(key);
            if (settingsValue != null)
            {
                value = this.converterRegistry.TryConvert(settingsValue, defaultValue);
            }

            return value;
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            Guard.ArgumentNotNullOrEmpty(() => key);
            Guard.ArgumentNotNull(() => value);

            ////if (value == null) // TODO: Base class for all implementations!
            ////{
            ////    this.Remove(key);
            ////}

            lock (this.locker)
            {
                var typeOf = typeof(T);
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                this.tracer.Debug("AddOrUpdateValue for key={0}, type={1}.", key, typeOf);

                if ((typeOf == typeof(string)) || (typeOf == typeof(double)) || (typeOf == typeof(float)) || (typeOf == typeof(Guid)) || (typeOf == typeof(bool)) || (typeOf == typeof(int))
                    || (typeOf == typeof(long)) || (typeOf == typeof(byte)))
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

        private void AddOrUpdateFunction<T>(string key, T value)
        {
            this.defaults.SetString(value.ToString(), key);
            this.defaults.Synchronize();
        }

        public void Remove(string key)
        {
            ////lock (this.locker)
            ////{
            ////    this.defaults.RemoveObject(key);
            ////    this.defaults.Synchronize();
            ////}
        }
    }
}