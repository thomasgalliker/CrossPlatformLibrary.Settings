using System;
using System.Diagnostics;
using System.Linq.Expressions;

using Guards;

namespace CrossPlatformLibrary.Settings
{
    public class SettingsProperty<T>
    {
        private readonly ISettingsService settingsService;
        private readonly string key;
        private readonly T defaultValue;

        public SettingsProperty(ISettingsService settingsService, Expression<Func<T>> expression, T defaultValue = default(T))
            : this(settingsService, ((MemberExpression)expression.Body).Member.Name, defaultValue)
        {
        }

        public SettingsProperty(ISettingsService settingsService, string key, T defaultValue = default(T))
        {
            Guard.ArgumentNotNull(() => settingsService);
            Guard.ArgumentNotNullOrEmpty(() => key);
            Guard.ArgumentHasMaxLength(() => key, 255);

            this.settingsService = settingsService;
            this.key = key;
            this.defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                return this.settingsService.GetValueOrDefault(this.key, this.defaultValue);
            }
            set
            {
                this.settingsService.AddOrUpdateValue(this.key, value);
            }
        }
    }
}