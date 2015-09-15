using System;
using System.Globalization;

using Android.App;
using Android.Content;
using Android.Preferences;

using CrossPlatformLibrary.IO;
using CrossPlatformLibrary.Tracing;
using CrossPlatformLibrary.Utils;

using TypeConverter;
using TypeConverter.Extensions;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService
    {
        private static ISharedPreferences sharedPreferences;
        private static ISharedPreferencesEditor sharedPreferencesEditor;

        private readonly object locker = new object();
        private readonly ITracer tracer;
        private readonly IConverterRegistry converterRegistry;

        public SettingsService(ITracer tracer, IConverterRegistry converterRegistry)
        {
            Guard.ArgumentNotNull(() => tracer);
            Guard.ArgumentNotNull(() => converterRegistry);

            this.tracer = tracer;
            this.converterRegistry = converterRegistry;

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            sharedPreferencesEditor = sharedPreferences.Edit();
        }

        public IConverterRegistry ConverterRegistry
        {
            get
            {
                return this.converterRegistry;
            }
        }

        /// <summary>
        ///     Gets the current value or the default that you specify.
        /// </summary>
        /// <typeparam name="T">Vaue of t (bool, int, float, long, string)</typeparam>
        /// <param name="key">Key for settings</param>
        /// <param name="defaultValue">default value if not set</param>
        /// <returns>Value or default</returns>
        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            lock (this.locker)
            {
                var typeOf = typeof(T);
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                object value = null;
                var typeCode = Type.GetTypeCode(typeOf);

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        value = sharedPreferences.GetBoolean(key, Convert.ToBoolean(defaultValue));
                        break;
                    case TypeCode.Int64:
                        value = sharedPreferences.GetLong(key, Convert.ToInt64(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.String:
                        value = sharedPreferences.GetString(key, Convert.ToString(defaultValue));
                        break;
                    case TypeCode.Double:
                        value = sharedPreferences.GetString(key, Convert.ToString(defaultValue));
                        value = Convert.ToDouble(value);
                        break;
                    case TypeCode.Decimal:
                        value = sharedPreferences.GetString(key, Convert.ToString(defaultValue));
                        value = Convert.ToDecimal(value);
                        break;
                    case TypeCode.Int32:
                        value = sharedPreferences.GetInt(key, Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Single:
                        value = sharedPreferences.GetFloat(key, Convert.ToSingle(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.DateTime:
                        var ticks = sharedPreferences.GetLong(key, -1);
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
                            Guid outGuid;
                            Guid.TryParse(sharedPreferences.GetString(key, Guid.Empty.ToString()), out outGuid);
                            value = outGuid;
                        }
                        else
                        {
                            value = defaultValue;
                            var serializedString = sharedPreferences.GetString(key, string.Empty);
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

        /// <summary>
        ///     Adds or updates the given value for the given key.
        /// </summary>
        /// <returns>True if added or update and you need to save</returns>
        public void AddOrUpdateValue<T>(string key, T value)
        {
            lock (this.locker)
            {
                var typeOf = typeof(T);
                if (typeOf.IsNullable())
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        sharedPreferencesEditor.PutBoolean(key, Convert.ToBoolean(value));
                        break;
                    case TypeCode.Int64:
                        sharedPreferencesEditor.PutLong(key, Convert.ToInt64(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.String:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        sharedPreferencesEditor.PutString(key, Convert.ToString(value));
                        break;
                    case TypeCode.Int32:
                        sharedPreferencesEditor.PutInt(key, Convert.ToInt32(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Single:
                        sharedPreferencesEditor.PutFloat(key, Convert.ToSingle(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.DateTime:
                        sharedPreferencesEditor.PutLong(key, ((DateTime)(object)value).Ticks);
                        break;
                    default:
                        if (value is Guid)
                        {
                            sharedPreferencesEditor.PutString(key, value.ToString());
                        }
                        else
                        {
                            string serialized = value.SerializeToXml();
                            sharedPreferencesEditor.PutString(key, serialized);
                        }
                        break;
                }

                sharedPreferencesEditor.Commit();
            }
        }
    }
}