using NextBusStation.ViewModels;

namespace NextBusStation.Views;

public partial class StopDetailsPage : ContentPage
{
    public StopDetailsPage(StopDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
