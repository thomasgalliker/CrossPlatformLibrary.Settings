using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization.Extensions;

using CrossPlatformLibrary.Extensions;

using Guards;

using Tracing;

using TypeConverter;
using TypeConverter.Converters;
using TypeConverter.Extensions;

namespace CrossPlatformLibrary.Settings
{
    public abstract class SettingsServiceBase : ISettingsService
    {
        private readonly object locker = new object();
        private readonly ITracer tracer;
        private readonly IConverterRegistry converterRegistry;

        private static readonly IConvertable[] DefaultConverters =
        {
            new StringToBoolConverter(),
            new StringToIntegerConverter(),
            new StringToUriConverter(),
            new StringToGuidConverter(),
            new StringToFloatConverter(),
            new StringToDoubleConverter(),
            new StringToDecimalConverter(),
            new StringToDateTimeConverter(),
            new StringToDateTimeOffsetConverter(),
        };

        protected SettingsServiceBase(ITracer tracer, IConverterRegistry converterRegistry)
        {
            Guard.ArgumentNotNull(tracer, nameof(tracer));
            Guard.ArgumentNotNull(converterRegistry, nameof(converterRegistry));

            this.tracer = tracer;
            this.converterRegistry = converterRegistry;
            this.converterRegistry.RegisterConverters(DefaultConverters);
        }

        protected IConverterRegistry ConverterRegistry
        {
            get
            {
                return this.converterRegistry;
            }
        }

        protected ITracer Tracer
        {
            get
            {
                return this.tracer;
            }
        }

        protected abstract object GetValueOrDefaultFunction<T>(string key, T defaultValue);

        protected abstract void AddOrUpdateValueFunction<T>(string key, T value);

        /// <summary>
        /// Checks if the given <paramref name="type" /> is a native type and can be stored without conversion.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual bool IsNativeType(Type type)
        {
            return DefaultNativeTypes.Contains(type);
        }

        private static readonly Type[] DefaultNativeTypes =
        {
            typeof (char),
            typeof (string),
        };

        /// <summary>
        /// Checks if the given <paramref name="type" /> is convertable to string and can be stored as such.
        /// </summary>
        protected virtual bool IsStringConvertable(Type type)
        {
            return DefaultStringSerializableTypes.Contains(type);
        }

        private static readonly Type[] DefaultStringSerializableTypes = 
        {
            typeof (byte),
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),
            typeof (long),
            typeof (ulong),
            typeof (float),
            typeof (double),
            typeof (decimal),
            typeof (bool),
            typeof (Uri),
            typeof (Guid),
            typeof (DateTime),
            typeof (DateTimeOffset),
        };

        private static Type UnwrapNullableType<T>()
        {
            var type = typeof(T);
            if (type.IsNullable())
            {
                type = Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));

            object value = default(T);

            lock (this.locker)
            {
                var type = UnwrapNullableType<T>();

                this.Tracer.Debug($"{nameof(this.GetValueOrDefault)}<{type.GetFormattedName()}>(key: \"{key}\")");

                if (this.IsNativeType(type))
                {
                    value = this.GetValueOrDefaultFunction(key, defaultValue);
                }
                else if (this.IsStringConvertable(type))
                {
                    value = this.GetValueOrDefaultFunction(key, defaultValue);
                }
                else
                {
                    var xmlSerializedObject = (string)this.GetValueOrDefaultFunction<string>(key, null);
                    if (xmlSerializedObject != null)
                    {
                        value = xmlSerializedObject.DeserializeFromXml<T>();
                    }
                }
            }

            return this.ConverterRegistry.TryConvert(value, defaultValue);
        }

        public void AddOrUpdateValue<T>(string key, T value)
        {
            Guard.ArgumentNotNullOrEmpty(key, nameof(key));
            Guard.ArgumentNotNull(value, nameof(value));

            lock (this.locker)
            {
                var type = UnwrapNullableType<T>();

                this.Tracer.Debug($"{nameof(this.AddOrUpdateValue)}<{type.GetFormattedName()}>(key: \"{key}\")");

                if (this.IsNativeType(type))
                {
                    this.AddOrUpdateValueFunction(key, value);
                }
                else if (this.IsStringConvertable(type))
                {
                    string serializedValue = this.converterRegistry.Convert<string>(value);
                    this.AddOrUpdateValueFunction(key, serializedValue);
                }
                else
                {
                    string xmlSerializedObject = value.SerializeToXml(preserveTypeInformation: true);
                    this.AddOrUpdateValueFunction(key, xmlSerializedObject);
                }
            }
        }
    }
}