using AviaExplorer.ViewModels.Avia;
using System;
using Xamarin.Forms;

namespace AviaExplorer.Views.Pages
{
    [QueryProperty(nameof(IATA), "iata")]
    public partial class DirectionsListPage
    {
        private string _iata;

        public string IATA
        {
            set => _iata = Uri.UnescapeDataString(value);
        }

        private DirectionsViewModel FlightsViewModel =>
            (DirectionsViewModel)BindingContext;

        public DirectionsListPage() =>
            InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();

            FlightsViewModel?.SetIATACommand?.Execute(_iata);
            FlightsViewModel?.GetSupportedDirectionsCommand?.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            FlightsViewModel?.ClearSupportedDirectionsCommand?.Execute(null);
        }
    }
}