using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AviaExplorer.ViewModels.Avia
{
    public class OriginSelectionViewModel : BaseViewModel
    {
        #region Fields
        private IAviaInfoService _aviaInfo;
        private IAnalyticsService _analytics;
        private ILanguageService _language;
        private INavigationService _navigation;

        private string _originIATA = "TOF";
        private string[] _choices;
        private ObservableCollection<string> _availableChoices;
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

        public string[] Choices
        {
            get => _choices;
            set
            {
                _choices = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AvailableChoices
        {
            get => _availableChoices;
            set
            {
                _availableChoices = value;
                OnPropertyChanged();
            }
        }

        public bool ChoicesUpdating { get; set; }
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

        public IAsyncCommand<string> NavigateToFlightsCommand =>
            new AsyncCommand<string>(NavigateToFlightsAsync);

        public ICommand ClearChoicesCommand =>
            new Command(ClearChoices);

        public ICommand FilterOriginCommand =>
            new Command<string>(FilterOrigin);
        #endregion

        #region Constructor
        public OriginSelectionViewModel(IAviaInfoService aviaInfo,
                                IAnalyticsService analytics,
                                ILanguageService language,
                                INavigationService navigation)
        {
            _aviaInfo = aviaInfo;
            _analytics = analytics;
            _language = language;
            _navigation = navigation;

            GetChoicesCommand?.Execute(null);
        }
        #endregion

        #region Methods
        private void FilterOrigin(string text)
        {
            var filterData = text;
            FilterOriginCommand?.CanExecute(false);
            if (string.IsNullOrEmpty(filterData))
            {
                AvailableChoices = new ObservableCollection<string>(Choices);
                FilterOriginCommand?.CanExecute(true);
                return;
            }
            AvailableChoices = new ObservableCollection<string>(Choices
                .Where(x => x.StartsWith(filterData.ToUpper())));
            FilterOriginCommand?.CanExecute(true);
        }

        private Task GetChoicesAsync()
        {
            ChoicesUpdating = true;
            if (string.IsNullOrEmpty(OriginIATA)) return Task.CompletedTask;

            return _aviaInfo.GetSupportedDirectionsAsync(OriginIATA, true, _language.Current)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    Choices = result.Directions
                        .Select(x => x.IATA)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray();
                    AvailableChoices = new ObservableCollection<string>(Choices);
                    ChoicesUpdating = false;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void ClearChoices()
        {
            AvailableChoices.Clear();
            AvailableChoices = new ObservableCollection<string>();
        }

        private Task NavigateToFlightsAsync(string origin) =>
            _navigation.NavigateToPageAsync($"directions?iata={origin}", false);
        #endregion
    }
}