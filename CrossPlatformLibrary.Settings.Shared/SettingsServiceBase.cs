using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization.Extensions;

using CrossPlatformLibrary.Extensions;

using Guards;

using Tracing;

using TypeConverter;

namespace CrossPlatformLibrary.Settings
{
    public abstract class SettingsServiceBase : ISettingsService
    {
        private readonly object locker = new object();
        protected readonly ITracer tracer;
        protected readonly IConverterRegistry converterRegistry;

        private const string RFormatString = "R"; // https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx#RFormatString

        private readonly IDictionary<Type, string> formatMapping = new Dictionary<Type, string>
        {
            { typeof(double), RFormatString },
            { typeof(float), RFormatString },
        };

        protected SettingsServiceBase(ITracer tracer, IConverterRegistry converterRegistry)
        {
            Guard.ArgumentNotNull(tracer, nameof(tracer));
            Guard.ArgumentNotNull(converterRegistry, nameof(converterRegistry));

            this.tracer = tracer;
            this.converterRegistry = converterRegistry;
        }

        protected abstract T GetValueOrDefaultFunction<T>(string key, T defaultValue);

        protected abstract void AddOrUpdateValueFunction<T>(string key, T value);

        /// <summary>
        ///     Checks if the given <paramref name="type" /> is a native type
        ///     and can be stored without conversion.
        /// </summary>
        protected virtual bool IsNativeType(Type type)
        {
            return type == typeof(string) ||
                    (type == typeof(double)) ||
                    (type == typeof(float)) ||
                    (type == typeof(Guid)) ||
                    (type == typeof(bool)) ||
                    (type == typeof(short)) ||
                    (type == typeof(ushort)) ||
                    (type == typeof(int)) ||
                    (type == typeof(uint)) ||
                    (type == typeof(long)) ||
                    (type == typeof(ulong)) ||
                    (type == typeof(byte));
        }

        protected virtual TTarget TryConvert<TSource, TTarget>(TSource source, TTarget defaultValue)
        {
            this.tracer.Debug($"Trying to convert from source type {typeof(TSource).GetFormattedName()} to target type {typeof(TTarget).GetFormattedName()}.");
            return this.converterRegistry.TryConvert(source, defaultValue);
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));

            object value = defaultValue;

            lock (this.locker)
            {
                var type = typeof(T);
                if (type.IsNullable())
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                this.tracer.Debug($"{nameof(this.GetValueOrDefault)}<{type.GetFormattedName()}>(key: \"{key}\")");

                if (this.IsNativeType(type))
                {
                    value = this.GetValueOrDefaultFunction(key, defaultValue);
                }

                else if (type == typeof(decimal))
                {
                    var savedDecimal = this.GetValueOrDefaultFunction<string>(key, null);
                    if (savedDecimal != null)
                    {
                        value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);
                    }
                }
                else if (type == typeof(DateTime))
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
                else if (type == typeof(Uri))
                {
                    string uriString = this.GetValueOrDefaultFunction<string>(key, null);
                    value = new Uri(uriString);
                }
                else
                {
                    value = this.DefaultDeserialization(key, defaultValue);
                }
            }

            return null != value ? (T)value : defaultValue;
        }

        protected virtual object DefaultDeserialization<T>(string key, T defaultValue)
        {
            var serializedObject = this.GetValueOrDefaultFunction<string>(key, null);
            if (serializedObject != null)
            {
                return Convert.ToString(serializedObject, CultureInfo.InvariantCulture).DeserializeFromXml<T>();
            }

            return defaultValue;
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNull(value, nameof(value));

            lock (this.locker)
            {
                var type = typeof(T);
                if (type.IsNullable())
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                this.tracer.Debug($"{nameof(this.AddOrUpdateValue)}<{type.GetFormattedName()}>(key: \"{key}\")");

                if (this.IsNativeType(type))
                {
                    string serializedValue;
                    var formattable = value as IFormattable;
                    if (formattable != null)
                    {
                        string formatter;
                        this.formatMapping.TryGetValue(typeof(T), out formatter);

                        serializedValue = formattable.ToString(formatter, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        serializedValue = value.ToString();
                    }

                    this.AddOrUpdateValueFunction(key, serializedValue);
                }
                else if (type == typeof(decimal))
                {
                    string decimalString = Convert.ToString(value, CultureInfo.InvariantCulture);
                    this.AddOrUpdateValueFunction(key, decimalString);
                }
                else if (type == typeof(DateTime))
                {
                    var dateTime = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    var serializableDateTime = SerializableDateTime.FromDateTime(dateTime);
                    string stringDateTime = serializableDateTime.SerializeToXml(preserveTypeInformation: true);
                    this.AddOrUpdateValueFunction(key, stringDateTime);
                }
                else if (type == typeof(Uri))
                {
                    string uriString = value.ToString();
                    this.AddOrUpdateValueFunction(key, uriString);
                }
                else
                {
                    string serializedString = value.SerializeToXml(preserveTypeInformation: true);
                    this.AddOrUpdateValueFunction(key, serializedString);
                }
            }
        }
    }
}