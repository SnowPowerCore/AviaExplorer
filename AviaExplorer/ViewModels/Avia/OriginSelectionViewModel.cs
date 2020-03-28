using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Models.Utils;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Interfaces;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Navigation;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AviaExplorer.ViewModels.Avia
{
    public class OriginSelectionViewModel : BaseViewModel
    {
        #region Fields
        private readonly IAviaInfoService _aviaInfo;
        private readonly IAnalyticsService _analytics;
        private readonly ILanguageService _language;
        private readonly IKeyboard _keyboard;
        private readonly INavigationService _navigation;

        private string _originIATA = "TOF";
        private AirportChoice[] _choices;
        private ObservableRangeCollection<AirportChoice> _availableChoices = 
            new ObservableRangeCollection<AirportChoice>();
        private bool _choicesUpdating;
        #endregion

        #region Properties
        public string OriginIATA
        {
            get => _originIATA;
            set
            {
                _originIATA = value;
                OnPropertyChanged();
            }
        }

        public AirportChoice[] Choices
        {
            get => _choices;
            set
            {
                _choices = value;
                OnPropertyChanged();
            }
        }

        public ObservableRangeCollection<AirportChoice> AvailableChoices
        {
            get => _availableChoices;
            set
            {
                _availableChoices = value;
                OnPropertyChanged();
            }
        }

        public bool ChoicesUpdating
        {
            get => _choicesUpdating;
            set
            {
                _choicesUpdating = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public IAsyncCommand GetChoicesCommand =>
            new AsyncCommand(GetChoicesAsync,
                _ => !ChoicesUpdating,
                e =>
                {
                    ChoicesUpdating = false;
                    _analytics.TrackError(e);
                });

        public IAsyncCommand<AirportChoice> NavigateToFlightsCommand =>
            new AsyncCommand<AirportChoice>(NavigateToFlightsAsync);

        public ICommand FindAndNavigateCommand =>
            new Command(FindAndNavigate);

        public ICommand SetOriginIATACommand =>
            new Command<string>(iata => OriginIATA = iata);

        public ICommand ClearChoicesCommand =>
            new Command(ClearChoices);

        public ICommand FilterOriginCommand =>
            new Command<string>(FilterOrigin);

        public ICommand HideKeyboardCommand =>
            new Command(() => _keyboard.HideKeyboard());
        #endregion

        #region Constructor
        public OriginSelectionViewModel(IAviaInfoService aviaInfo,
                                        IKeyboard keyboard,
                                        IAnalyticsService analytics,
                                        ILanguageService language,
                                        INavigationService navigation)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
            _keyboard = keyboard;
            _navigation = navigation;

            GetChoicesCommand?.Execute(null);
        }
        #endregion

        #region Methods
        private void FilterOrigin(string text)
        {
            var filterData = text;
            AvailableChoices.Clear();
            FilterOriginCommand?.CanExecute(false);
            if (string.IsNullOrEmpty(filterData))
            {
                AvailableChoices.AddRange(Choices);
                FilterOriginCommand?.CanExecute(true);
                return;
            }
            AvailableChoices.AddRange(Choices.Where(x => x.Name.StartsWith(text)));
            FilterOriginCommand?.CanExecute(true);
        }

        private void FindAndNavigate()
        {
            if (string.IsNullOrEmpty(OriginIATA)) return;
            var item = Choices.FirstOrDefault(x => x.Name.StartsWith(OriginIATA));
            NavigateToFlightsCommand?.Execute(item);
        }

        private Task GetChoicesAsync()
        {
            if (string.IsNullOrEmpty(OriginIATA)) return Task.CompletedTask;

            AvailableChoices.Clear();

            return _aviaInfo.GetSupportedDirectionsAsync(OriginIATA, true, _language.Current)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    Choices = result.Directions
                        .Select(x => new AirportChoice
                        {
                            Name = x.IATA,
                            GeoPosition = new Xamarin.Forms.Maps.Position(
                                x.Coordinates.LastOrDefault(),
                                x.Coordinates.FirstOrDefault())
                        })
                        .Distinct()
                        .OrderBy(x => x.Name)
                        .ToArray();
                    AvailableChoices.AddRange(Choices);
                    ChoicesUpdating = false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void ClearChoices() =>
            AvailableChoices.Clear();

        private Task NavigateToFlightsAsync(AirportChoice origin)
        {
            if (origin is null) return Task.CompletedTask;
            var data = Uri.EscapeDataString(JsonConvert.SerializeObject(origin));
            return _navigation.NavigateToPageAsync($"directions?data={data}", false);
        }
        #endregion
    }
}