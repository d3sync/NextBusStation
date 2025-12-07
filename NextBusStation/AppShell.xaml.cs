using NextBusStation.Views;

namespace NextBusStation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("stopdetails", typeof(StopDetailsPage));
            Routing.RegisterRoute("editschedule", typeof(EditSchedulePage));
        }
    }
}
