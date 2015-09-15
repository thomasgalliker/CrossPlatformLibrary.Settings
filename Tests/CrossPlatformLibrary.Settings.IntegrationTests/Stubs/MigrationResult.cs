
namespace CrossPlatformLibrary.Settings.IntegrationTests.Stubs
{
    public class MigrationResult<TFrom, TTo>
    {
        public TFrom From { get; private set; }
        public TTo To { get; private set; }

        public MigrationResult(TFrom from, TTo to)
        {
            this.From = from;
            this.To = to;
        }
    }
}
