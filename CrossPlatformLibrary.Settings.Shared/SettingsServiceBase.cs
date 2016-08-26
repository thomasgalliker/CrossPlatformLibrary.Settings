
////using Tracing;
////using Guards;

////using TypeConverter;

////namespace CrossPlatformLibrary.Settings
////{
////    public abstract class SettingsServiceBase : ISettingsService
////    {
////        protected readonly ITracer tracer;
////        protected readonly IConverterRegistry converterRegistry;

////        public SettingsServiceBase(ITracer tracer, IConverterRegistry converterRegistry)
////        {
////            Guard.ArgumentNotNull(() => tracer);
////            Guard.ArgumentNotNull(() => converterRegistry);

////            this.tracer = tracer;
////            this.converterRegistry = converterRegistry;
////        }

////        public IConverterRegistry ConverterRegistry
////        {
////            get
////            {
////                return this.converterRegistry;
////            }
////        }

////        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
////        {
////            this.tracer.Debug("GetValueOrDefault with key={0} of type {1}", key, typeof(T));
////        }

////        public void AddOrUpdateValue<T>(string key, T value)
////        {
////            this.tracer.Debug("AddOrUpdateValue with key={0} of type {1}", key, typeof(T));
////        }
////    }
////}