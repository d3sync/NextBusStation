using NextBusStation.ViewModels;

namespace NextBusStation.Views;

public partial class EditSchedulePage : ContentPage
{
    public EditSchedulePage(EditScheduleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
