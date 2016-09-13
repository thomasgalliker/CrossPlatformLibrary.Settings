using System;
using System.Collections.Generic;
using System.Linq;

using CrossPlatformLibrary.Settings;
using Guards;

using SettingsSample.Model;

namespace SettingsSample.Services
{
    public class DemoSettingsService : IDemoSettingsService
    {
        private readonly SettingsProperty<Guid> id;
        private readonly SettingsProperty<DateTime> lastLoaded;
        private readonly SettingsProperty<int> numberOfClicks;
        private readonly SettingsProperty<decimal> decimalValue;
        private readonly SettingsProperty<float> floatValue;
        private readonly SettingsProperty<IEnumerable<PersonModel>> personModels;
        private readonly SettingsProperty<ushort?> ushortNullable;

        public DemoSettingsService(ISettingsService settingsService)
        {
            Guard.ArgumentNotNull(settingsService, nameof(settingsService));

            this.id = new SettingsProperty<Guid>(settingsService, () => this.Id);
            this.lastLoaded = new SettingsProperty<DateTime>(settingsService, () => this.LastLoaded, DateTime.MinValue);
            this.numberOfClicks = new SettingsProperty<int>(settingsService, () => this.NumberOfClicks);
            this.decimalValue = new SettingsProperty<decimal>(settingsService, () => this.DecimalValue);
            this.floatValue = new SettingsProperty<float>(settingsService, () => this.FloatValue);
            this.personModels = new SettingsProperty<IEnumerable<PersonModel>>(settingsService, () => this.PersonModels, Enumerable.Empty<PersonModel>());
            this.ushortNullable = new SettingsProperty<ushort?>(settingsService, () => this.UshortNullable);
        }
        
        public Guid Id
        {
            get
            {
                return this.id.Value;
            }
            set
            {
                this.id.Value = value;
            }
        }

        public DateTime LastLoaded
        {
            get
            {
                return this.lastLoaded.Value;
            }
            set
            {
                this.lastLoaded.Value = value;
            }
        }

        public int NumberOfClicks
        {
            get
            {
                return this.numberOfClicks.Value;
            }
            set
            {
                this.numberOfClicks.Value = value;
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return this.decimalValue.Value;
            }
            set
            {
                this.decimalValue.Value = value;
            }
        }

        public float FloatValue
        {
            get
            {
                return this.floatValue.Value;
            }
            set
            {
                this.floatValue.Value = value;
            }
        }

        public ushort? UshortNullable
        {
            get
            {
                return this.ushortNullable.Value;
            }
            set
            {
                this.ushortNullable.Value = value;
            }
        }

        public IEnumerable<PersonModel> PersonModels
        {
            get
            {
                return this.personModels.Value;
            }
            set
            {
                this.personModels.Value = value;
            }
        }
    }
}