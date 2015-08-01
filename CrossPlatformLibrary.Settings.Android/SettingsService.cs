using System;
using System.Globalization;

using Android.App;
using Android.Content;
using Android.Preferences;

using CrossPlatformLibrary.IO;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsService : ISettingsService
    {
        private static ISharedPreferences SharedPreferences { get; set; }

        private static ISharedPreferencesEditor SharedPreferencesEditor { get; set; }

        private readonly object locker = new object();

        public SettingsService()
        {
            SharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            SharedPreferencesEditor = SharedPreferences.Edit();
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
                Type typeOf = typeof(T);
                if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                object value = null;
                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        value = (decimal)SharedPreferences.GetLong(key, (long)Convert.ToDecimal(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Boolean:
                        value = SharedPreferences.GetBoolean(key, Convert.ToBoolean(defaultValue));
                        break;
                    case TypeCode.Int64:
                        value = (Int64)SharedPreferences.GetLong(key, (long)Convert.ToInt64(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.String:
                        value = SharedPreferences.GetString(key, Convert.ToString(defaultValue));
                        break;
                    case TypeCode.Double:
                        value = (double)SharedPreferences.GetLong(key, (long)Convert.ToDouble(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Int32:
                        value = SharedPreferences.GetInt(key, Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Single:
                        value = SharedPreferences.GetFloat(key, Convert.ToSingle(defaultValue, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.DateTime:
                        var ticks = SharedPreferences.GetLong(key, -1);
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
                            Guid.TryParse(SharedPreferences.GetString(key, Guid.Empty.ToString()), out outGuid);
                            value = outGuid;
                        }
                        else
                        {
                            value = defaultValue;
                            var serializedString = SharedPreferences.GetString(key, string.Empty);
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
        ///     Adds or updates a value
        /// </summary>
        /// <param name="key">key to update</param>
        /// <param name="value">value to set</param>
        /// <returns>True if added or update and you need to save</returns>
        public bool AddOrUpdateValue<T>(string key, T value)
        {
            lock (this.locker)
            {
                Type typeOf = value.GetType();
                if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }
                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        SharedPreferencesEditor.PutLong(key, (long)Convert.ToDecimal(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Boolean:
                        SharedPreferencesEditor.PutBoolean(key, Convert.ToBoolean(value));
                        break;
                    case TypeCode.Int64:
                        SharedPreferencesEditor.PutLong(key, (long)Convert.ToInt64(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.String:
                        SharedPreferencesEditor.PutString(key, Convert.ToString(value));
                        break;
                    case TypeCode.Double:
                        SharedPreferencesEditor.PutLong(key, (long)Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Int32:
                        SharedPreferencesEditor.PutInt(key, Convert.ToInt32(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Single:
                        SharedPreferencesEditor.PutFloat(key, Convert.ToSingle(value, CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.DateTime:
                        SharedPreferencesEditor.PutLong(key, ((DateTime)(object)value).Ticks);
                        break;
                    default:
                        if (value is Guid)
                        {
                            SharedPreferencesEditor.PutString(key, value.ToString());
                        }
                        else
                        {
                            string serialized = value.SerializeToXml();
                            SharedPreferencesEditor.PutString(key, serialized);
                        }
                        break;
                }
            }

            lock (this.locker)
            {
                SharedPreferencesEditor.Commit();
            }
            return true;
        }
    }
}