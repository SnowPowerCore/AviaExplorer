using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AviaExplorer.ViewModels.Avia
{
    public class DirectionsViewModel : BaseViewModel
    {
        #region Fields
        private IAviaInfoService _aviaInfo;
        private IAnalyticsService _analytics;
        private ILanguageService _language;
        private INavigationService _navigation;

        private ObservableCollection<FlightModel> _directions = new ObservableCollection<FlightModel>();
        #endregion

        #region Properties
        public ObservableCollection<FlightModel> Directions
        {
            get => _directions;
            set
            {
                _directions = value;
                OnPropertyChanged();
            }
        }

        private string IATA { get; set; }

        public bool DirectionsUpdating { get; set; }
        #endregion

        #region Commands
        public IAsyncCommand GetSupportedDirectionsCommand =>
            new AsyncCommand(GetSupportedDirectionsAsync,
                _ => !DirectionsUpdating,
                e =>
                {
                    DirectionsUpdating = false;
                    _analytics.TrackError(e);
                });

        public ICommand ClearSupportedDirectionsCommand =>
            new Command(ClearSupportedDirections);

        public ICommand SetIATACommand =>
            new Command<string>(iata => IATA = iata);
        #endregion

        #region Constructor
        public DirectionsViewModel(IAviaInfoService aviaInfo,
                                IAnalyticsService analytics,
                                ILanguageService language,
                                INavigationService navigation)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
            _navigation = navigation;
        }
        #endregion

        #region Methods
        
        private Task GetSupportedDirectionsAsync()
        {
            DirectionsUpdating = true;
            if (string.IsNullOrEmpty(IATA)) return Task.CompletedTask;

            return _aviaInfo.GetSupportedDirectionsAsync(IATA, true, _language.Current)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    Directions = new ObservableCollection<FlightModel>(
                        result.Directions.Select(x => new FlightModel
                        {
                            OriginIATA = result.Origin.IATA,
                            DestinationIATA = x.IATA,
                            OriginName = result.Origin.Name,
                            DestinationName = x.Name,
                            DestinationCountry = x.Country
                        }));
                    DirectionsUpdating = false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void ClearSupportedDirections()
        {
            Directions.Clear();
            Directions = new ObservableCollection<FlightModel>();
        }
        #endregion
    }
}