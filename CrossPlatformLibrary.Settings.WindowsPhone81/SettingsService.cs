﻿using System;
using System.Globalization;
using CrossPlatformLibrary.IO;
using CrossPlatformLibrary.Tracing;
using CrossPlatformLibrary.Utils;
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

        // TODO GATH: All of these types are supported https://msdn.microsoft.com/en-us/library/windows/apps/br205768.aspx
        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            object value = defaultValue;

            lock (this.locker)
            {
                if (IsWindowsRuntimeType(typeof(T)))
                {
                    value = this.GetValueOrDefaultFunction(key, defaultValue);
                }
                else if (value is decimal)
                {
                    var savedDecimal = this.GetValueOrDefaultFunction<string>(key, null);
                    if (savedDecimal != null)
                    {
                        value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);
                    }
                }
                else if (value is DateTime)
                {
                    var ticks = this.GetValueOrDefaultFunction<long>(key, -1);
                    if (ticks != -1)
                    {
                        value = new DateTime(ticks);
                    }
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
                if (IsWindowsRuntimeType(typeof(T)))
                {
                    this.AddOrUpdateFunction(key, value);
                }
                else if (value is decimal)
                {
                    var decimalString = value.ToString();
                    this.AddOrUpdateFunction(key, decimalString);
                }
                else if (value is DateTime)
                {
                    long ticks = Convert.ToDateTime(value).Ticks;
                    this.AddOrUpdateFunction(key, ticks);
                }
                else
                {
                    string serializedString = value.SerializeToXml();
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
                   type == typeof(Uri) ||
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