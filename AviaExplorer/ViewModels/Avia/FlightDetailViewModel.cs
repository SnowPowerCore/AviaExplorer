using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AviaExplorer.ViewModels.Avia
{
    public class FlightDetailViewModel : BaseViewModel
    {
        private readonly IAviaInfoService _aviaInfo;
        private readonly IAnalyticsService _analytics;
        private readonly ILanguageService _language;

        private DirectionModel _currentDirection;
        private List<FlightModel> _flights;
        private bool _flightsUpdating;

        public DirectionModel CurrentDirection
        {
            get => _currentDirection;
            private set
            {
                _currentDirection = value;
                OnPropertyChanged();
            }
        }

        public List<FlightModel> Flights
        {
            get => _flights;
            set
            {
                _flights = value;
                OnPropertyChanged();
            }
        }

        public bool FlightsUpdating
        {
            get => _flightsUpdating;
            set
            {
                _flightsUpdating = value;
                OnPropertyChanged();
            }
        }

        public IAsyncCommand GetFlightsDataCommand =>
            new AsyncCommand(GetFlightsDataAsync,
                _ => !FlightsUpdating,
                e =>
                {
                    FlightsUpdating = false;
                    _analytics.TrackError(e);
                });

        public ICommand SetDirectionCommand =>
            new Command<DirectionModel>(direction => CurrentDirection = direction);

        public FlightDetailViewModel(IAviaInfoService aviaInfo,
                                     IAnalyticsService analytics,
                                     ILanguageService language)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
        }

        private Task GetFlightsDataAsync()
        {
            if (CurrentDirection is null) return Task.CompletedTask;

            return _aviaInfo.GetFlightsDataAsync(CurrentDirection.OriginIATA, false, _language.Current,
                "2018-12-10", true, "50000", true, false, false, "1", "7")
                    .ContinueWith(t =>
                    {
                        var result = t.Result;
                        Flights = result
                            .Where(x => x.Actual && x.Destination == CurrentDirection.DestinationIATA)
                            .Select(x => new FlightModel
                            {
                                DepartureDate = DateTime.Parse(x.DepartDate),
                                ArrivalDate = DateTime.Parse(x.ReturnDate),
                                Price = x.FlightPrice
                            })
                            .ToList();
                        FlightsUpdating = false;
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}