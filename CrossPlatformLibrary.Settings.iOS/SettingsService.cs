
using Guards;
using CrossPlatformLibrary.IO;

using System;
using System.Globalization;

using CrossPlatformLibrary.Extensions;
using CrossPlatformLibrary.Tracing;

using TypeConverter;
using TypeConverter.Extensions;
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

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNullOrEmpty(() => key);

            lock (this.locker)
            {
                var defaults = NSUserDefaults.StandardUserDefaults;

                if (defaults.ValueForKey(new NSString(key)) == null)
                {
                    return defaultValue;
                }

                var typeOf = typeof(T);
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                object value = null;
                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        var savedDecimal = defaults.StringForKey(key);
                        value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.Boolean:
                        value = defaults.BoolForKey(key);
                        break;
                    case TypeCode.Int64:
                        var savedInt64 = defaults.StringForKey(key);
                        value = Convert.ToInt64(savedInt64, CultureInfo.InvariantCulture);
                        break;
                    case TypeCode.Double:
                        value = defaults.DoubleForKey(key);
                        break;
                    case TypeCode.String:
                        value = defaults.StringForKey(key);
                        break;
                    case TypeCode.Int32:
#if __UNIFIED__
            value = (Int32)defaults.IntForKey(key);
#else
                        value = defaults.IntForKey(key);
#endif
                        break;
                    case TypeCode.Single:
#if __UNIFIED__
            value = (float)defaults.FloatForKey(key);
#else
                        value = defaults.FloatForKey(key);
#endif
                        break;

                    case TypeCode.DateTime:
                        var savedTime = defaults.StringForKey(key);
                        var ticks = string.IsNullOrWhiteSpace(savedTime) ? -1 : Convert.ToInt64(savedTime, CultureInfo.InvariantCulture);
                        if (ticks == -1)
                        {
                            value = defaultValue;
                        }
                        else
                        {
                            value = new DateTime(ticks);
                        }
                        break;
                    default:

                        if (defaultValue is Guid)
                        {
                            var outGuid = Guid.Empty;
                            var savedGuid = defaults.StringForKey(key);
                            if (string.IsNullOrWhiteSpace(savedGuid))
                            {
                                value = outGuid;
                            }
                            else
                            {
                                Guid.TryParse(savedGuid, out outGuid);
                                value = outGuid;
                            }
                        }
                        else
                        {
                            value = defaultValue;
                            string serializedString = defaults.StringForKey(key);
                            if (!string.IsNullOrEmpty(serializedString))
                            {
                                value = serializedString.DeserializeFromXml<T>();
                            }
                        }

                        break;
                }

                return null != value ? (T)value : defaultValue;
            }
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            Guard.ArgumentNotNullOrEmpty(() => key);

            var typeOf = typeof(T);
            if (typeOf.IsNullable())
            {
                typeOf = Nullable.GetUnderlyingType(typeOf);
            }
            var typeCode = Type.GetTypeCode(typeOf);

            lock (this.locker)
            {
                var defaults = NSUserDefaults.StandardUserDefaults;
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        defaults.SetString(Convert.ToString(value, CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.Boolean:
                        defaults.SetBool(Convert.ToBoolean(value), key);
                        break;
                    case TypeCode.Int64:
                        defaults.SetString(Convert.ToString(value, CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.Double:
                        defaults.SetDouble(Convert.ToDouble(value, CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.String:
                        defaults.SetString(Convert.ToString(value), key);
                        break;
                    case TypeCode.Int32:
                        defaults.SetInt(Convert.ToInt32(value, CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.Single:
                        defaults.SetFloat(Convert.ToSingle(value, CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.DateTime:
                        defaults.SetString(Convert.ToString((Convert.ToDateTime(value)).Ticks), key);
                        break;
                    default:
                        if (value is Guid)
                        {
                            defaults.SetString(value.ToString(), key);
                        }
                        else
                        {
                            string serialized = value.SerializeToXml();
                            defaults.SetString(serialized, key);
                        }
                        break;
                }

                defaults.Synchronize();
            }
        }
    }
}