namespace TabletyRejestrApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void DokonajRejestru_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SetUpRecord));
        }

        private async void Tablety_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TabletsMenu));
        }

        private void OnExit_Clicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }

    }
}
