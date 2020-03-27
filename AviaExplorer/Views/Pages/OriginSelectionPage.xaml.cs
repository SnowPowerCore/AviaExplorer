using AviaExplorer.ViewModels.Avia;

namespace AviaExplorer.Views.Pages
{
    public partial class OriginSelectionPage
    {
        private FlightsViewModel FlightsViewModel =>
            (FlightsViewModel)BindingContext;

        public OriginSelectionPage() =>
            InitializeComponent();

        protected override void OnAppearing()
        {
            base.OnAppearing();

            FlightsViewModel?.GetChoicesCommand?.Execute(null);
        }
    }
}