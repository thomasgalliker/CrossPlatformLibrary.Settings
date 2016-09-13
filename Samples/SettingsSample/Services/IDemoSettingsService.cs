using System;
using System.Collections.Generic;

using SettingsSample.Model;

namespace SettingsSample.Services
{
    public interface IDemoSettingsService
    {
        Guid Id { get; set; }

        DateTime LastLoaded { get; set; }

        int NumberOfClicks { get; set; }

        decimal DecimalValue { get; set; }

        float FloatValue { get; set; }

        ////ushort? UshortNullable { get; set; } // TODO GATH: Nullable types are not yet supported

        IEnumerable<PersonModel> PersonModels { get; set; }
    }
}