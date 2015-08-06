﻿using CrossPlatformLibrary.Bootstrapping;
using CrossPlatformLibrary.IoC;

namespace CrossPlatformLibrary.Settings
{
    public class ContainerExtension : IContainerExtension
    {
        public void Initialize(ISimpleIoc container)
        {
            container.RegisterPlatformSpecific<ISettingsService>();
        }
    }
}
