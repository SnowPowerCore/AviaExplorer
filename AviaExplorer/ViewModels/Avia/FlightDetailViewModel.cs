using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Models.Utils;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AviaExplorer.ViewModels.Avia
{
    public class FlightDetailViewModel : BaseViewModel
    {
        #region Fields
        private readonly IAviaInfoService _aviaInfo;
        private readonly IAnalyticsService _analytics;
        private readonly ILanguageService _language;

        private DirectionModel _currentDirection;
        private ObservableRangeCollection<FlightModel> _flights =
            new ObservableRangeCollection<FlightModel>();
        private bool _flightsUpdating;
        #endregion

        #region Properties
        /// <summary>
        /// Current chosen direction
        /// </summary>
        public DirectionModel CurrentDirection
        {
            get => _currentDirection;
            private set
            {
                _currentDirection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Flights available
        /// </summary>
        public ObservableRangeCollection<FlightModel> Flights
        {
            get => _flights;
            set
            {
                _flights = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Flag determines updating state
        /// </summary>
        public bool FlightsUpdating
        {
            get => _flightsUpdating;
            set
            {
                _flightsUpdating = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Fetches flights
        /// </summary>
        public IAsyncCommand GetFlightsDataCommand =>
            new AsyncCommand(GetFlightsDataAsync,
                _ => !FlightsUpdating,
                e =>
                {
                    FlightsUpdating = false;
                    _analytics.TrackError(e);
                });

        /// <summary>
        /// Sets chosen direction
        /// </summary>
        public ICommand SetDirectionCommand =>
            new Command<DirectionModel>(direction => CurrentDirection = direction);
        #endregion

        public FlightDetailViewModel(IAviaInfoService aviaInfo,
                                     IAnalyticsService analytics,
                                     ILanguageService language)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
        }

        #region Methods
        private Task GetFlightsDataAsync()
        {
            if (CurrentDirection is null) return Task.CompletedTask;

            Flights.Clear();

            return _aviaInfo.GetFlightsDataAsync(CurrentDirection.OriginIATA, false, _language.Current,
                "2018-12-10:season", true, "50000", true, false, false, "1", "7")
                    .ContinueWith(t =>
                    {
                        var result = t.Result;
                        Flights.AddRange(result
                            .Where(x => x.Actual && x.Destination == CurrentDirection.DestinationIATA)
                            .Select(x => new FlightModel
                            {
                                DepartureDate = DateTime.Parse(x.DepartDate),
                                ReturnDate = DateTime.Parse(x.ReturnDate),
                                Price = x.FlightPrice
                            }));
                        FlightsUpdating = false;
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        #endregion
    }
}