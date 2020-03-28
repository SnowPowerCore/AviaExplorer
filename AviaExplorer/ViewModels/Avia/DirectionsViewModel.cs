using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Models.Utils;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AviaExplorer.ViewModels.Avia
{
    public class DirectionsViewModel : BaseViewModel
    {
        #region Fields
        private readonly IAviaInfoService _aviaInfo;
        private readonly IAnalyticsService _analytics;
        private readonly ILanguageService _language;
        private readonly INavigationService _navigation;

        private ObservableRangeCollection<DirectionModel> _directions =
            new ObservableRangeCollection<DirectionModel>();
        private List<DirectionModel> _pins;
        private bool _directionsUpdating;
        #endregion

        #region Properties
        public ObservableRangeCollection<DirectionModel> Directions
        {
            get => _directions;
            set
            {
                _directions = value;
                OnPropertyChanged();
            }
        }

        public List<DirectionModel> Pins
        {
            get => _pins;
            set
            {
                _pins = value;
                OnPropertyChanged();
            }
        }

        private AirportChoice OriginAirport { get; set; }

        public bool DirectionsUpdating
        {
            get => _directionsUpdating;
            set
            {
                _directionsUpdating = value;
                OnPropertyChanged();
            }
        }
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

        public IAsyncCommand<string> NavigateAirportCommand =>
            new AsyncCommand<string>(NavigateAirportAsync);

        public ICommand ClearSupportedDirectionsCommand =>
            new Command(ClearSupportedDirections);

        public ICommand SetOriginAirportCommand =>
            new Command<AirportChoice>(iata => OriginAirport = iata);
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
            if (string.IsNullOrEmpty(OriginAirport.Name)) return Task.CompletedTask;

            Directions.Clear();

            return _aviaInfo.GetSupportedDirectionsAsync(OriginAirport.Name, true, _language.Current)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    Directions.AddRange(result.Directions
                        .Select(x => new DirectionModel
                        {
                            OriginIATA = result.Origin.IATA,
                            DestinationIATA = x.IATA,
                            OriginName = result.Origin.Name,
                            DestinationName = x.Name,
                            DestinationCountry = x.Country,
                            GeoPosition = new Position(
                                x.Coordinates.LastOrDefault(),
                                x.Coordinates.FirstOrDefault())
                        }));
                    Pins = Directions.ToList();
                    DirectionsUpdating = false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private Task NavigateAirportAsync(string name)
        {
            var destAirport = Directions.FirstOrDefault(x => x.DestinationName == name);
            var data = Uri.EscapeDataString(JsonConvert.SerializeObject(destAirport));
            return _navigation.NavigateToPageAsync($"flights?data={data}");
        }

        private void ClearSupportedDirections() =>
            Directions.Clear();
        #endregion
    }
}