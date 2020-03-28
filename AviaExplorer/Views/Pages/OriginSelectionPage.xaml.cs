using AviaExplorer.ViewModels.Avia;

namespace AviaExplorer.Views.Pages
{
    public partial class OriginSelectionPage
    {
        private OriginSelectionViewModel OriginSelectionViewModel =>
            (OriginSelectionViewModel)BindingContext;

        public OriginSelectionPage() =>
            InitializeComponent();

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            OriginSelectionViewModel?.HideKeyboardCommand?.Execute(null);
        }
    }
}