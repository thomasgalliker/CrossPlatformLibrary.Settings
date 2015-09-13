using System;
using System.Linq;
using CrossPlatformLibrary.IoC;
using SettingsSample.Model;
using SettingsSample.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace SettingsSample.WindowsPhone81
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly IDemoSettingsService demoSettingsService;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.demoSettingsService = SimpleIoc.Default.GetInstance<IDemoSettingsService>();
            this.demoSettingsService.Id = Guid.NewGuid();
            var guid = this.demoSettingsService.Id;
            this.demoSettingsService.DecimalValue += this.demoSettingsService.DecimalValue;
            this.demoSettingsService.FloatValue += this.demoSettingsService.FloatValue;
            ////this.demoSettingsService.UshortNullable = 1; // TODO GATH: Nullable<?> doesnt work yet!
            ////this.demoSettingsService.UshortNullable += this.demoSettingsService.UshortNullable;

            var personModels = this.demoSettingsService.PersonModels.ToList();
            personModels.Add(new PersonModel { Name = "Person1", Age = 1 });
            personModels.Add(new PersonModel { Name = "Person2", Age = 2 });
            this.demoSettingsService.PersonModels = personModels;

            this.demoSettingsService.LastLoaded += new TimeSpan(1, 0, 0, 0);
            this.demoSettingsService.LastLoaded += new TimeSpan(1, 0, 0, 0);
            this.demoSettingsService.LastLoaded += new TimeSpan(1, 0, 0, 0);

            this.Button.Click += this.OnButtonClick;

            this.RefreshNumberOfClicks();
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            this.demoSettingsService.NumberOfClicks++;
            this.RefreshNumberOfClicks();
        }

        private void RefreshNumberOfClicks()
        {
            this.Button.Content = string.Format("IDemoSettingsService.NumberOfClicks={0}", this.demoSettingsService.NumberOfClicks);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
