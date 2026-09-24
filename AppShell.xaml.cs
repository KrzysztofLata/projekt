namespace TabletyRejestrApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SetUpRecord), typeof(SetUpRecord));
            Routing.RegisterRoute(nameof(AsignTablet), typeof(AsignTablet));
            Routing.RegisterRoute(nameof(TabletsMenu), typeof(TabletsMenu));
            Routing.RegisterRoute(nameof(InvestigateTablet), typeof(InvestigateTablet));
            Routing.RegisterRoute(nameof(QrScannerPage), typeof(QrScannerPage));


        }
    }
}
