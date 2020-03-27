using AsyncAwaitBestPractices.MVVM;
using AviaExplorer.Models.Avia;
using AviaExplorer.Services.Avia.AviaInfo;
using AviaExplorer.Services.Utils.Language;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AviaExplorer.ViewModels.Avia
{
    public class FlightsViewModel : BaseViewModel
    {
        private IAviaInfoService _aviaInfo;
        private ILanguageService _language;

        private string _originIATA = "TOF";
        private ObservableCollection<FlightModel> _destinations = new ObservableCollection<FlightModel>();
        private string[] _choices;
        private ObservableCollection<string> _availableChoices;
        private bool _choicesUpdating;

        public string OriginIATA
        {
            get => _originIATA;
            set
            {
                _originIATA = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FlightModel> Destinations
        {
            get => _destinations;
            set
            {
                _destinations = value;
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

        public bool ChoicesUpdating
        {
            get => _choicesUpdating;
            set
            {
                _choicesUpdating = value;
                OnPropertyChanged();
            }
        }

        public IAsyncCommand GetChoicesCommand =>
            new AsyncCommand(GetChoicesAsync,
                _ => !ChoicesUpdating,
                e => ChoicesUpdating = false);

        public IAsyncCommand GetSupportedDirectionsCommand =>
            new AsyncCommand(GetSupportedDirectionsAsync);

        public ICommand FilterOriginCommand =>
            new Command<string>(FilterOrigin);

        public FlightsViewModel(IAviaInfoService aviaInfo,
                                ILanguageService language)
        {
            _aviaInfo = aviaInfo;
            _language = language;

            Choices = new[] { _originIATA };
            AvailableChoices = new ObservableCollection<string>(Choices);
        }

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

        private Task GetSupportedDirectionsAsync()
        {
            if (string.IsNullOrEmpty(OriginIATA)) return Task.CompletedTask;

            return _aviaInfo.GetSupportedDirectionsAsync(OriginIATA, true, _language.Current)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    Destinations = new ObservableCollection<FlightModel>(
                        result.Directions.Select(x => new FlightModel
                        {
                            OriginIATA = result.Origin.IATA,
                            DestinationIATA = x.IATA,
                            OriginName = result.Origin.Name,
                            DestinationName = x.Name,
                            DestinationCountry = x.Country
                        }));
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}