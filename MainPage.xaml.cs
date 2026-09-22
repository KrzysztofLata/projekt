namespace projekt;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnExitClicked(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Exit", "Kliknięto Exit", "OK");
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new settings());

    }

    private async void OnStudentsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Baza uczniów", "Otwieranie bazy uczniów...", "OK");
    }

    private async void Calendar_Tap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new calendar());

    }
    private async void Register_Tap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new dokonaj_rejestru());

    }
    private async void Tablet_Tap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new tablety());

    }
}