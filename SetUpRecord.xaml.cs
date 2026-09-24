using TabletyRejestrApp.Models;

namespace TabletyRejestrApp;

public partial class SetUpRecord : ContentPage
{
    public SetUpRecord()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadClassesAsync();
    }

    private async Task LoadClassesAsync()
    {
        try
        {
            ClassPicker.Items.Clear();

            // Pobieramy wszystkich studentów
            var students = await App.Database.GetStudentsAsync();

            // Pobieramy unikalne klasy
            var classes = students
                .Where(x => !string.IsNullOrWhiteSpace(x.Klasa))
                .Select(x => x.Klasa)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Dodajemy klasy do Pickera
            foreach (var klasa in classes)
            {
                ClassPicker.Items.Add(klasa);
            }

            // Automatycznie wybierz pierwsz¹ klasê
            if (ClassPicker.Items.Count > 0)
            {
                ClassPicker.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê pobraæ klas z bazy.\n\n{ex.Message}",
                "OK");
        }
    }


    private async void Utworz_Clicked(object sender, EventArgs e)
    {
        // Sprawdzenie czy wybrano klasê
        if (ClassPicker.SelectedItem == null)
        {
            await DisplayAlert(
                "Brak klasy",
                "Wybierz klasê.",
                "OK");

            return;
        }

        // Pobranie wybranej klasy
        string selectedClass = ClassPicker.SelectedItem.ToString();


        // Pobranie godzin
        TimeSpan startTime = StartTimePicker.Time;
        TimeSpan endTime = EndTimePicker.Time;


        // Sprawdzenie poprawnoœci godzin
        if (endTime <= startTime)
        {
            await DisplayAlert(
                "B³êdne godziny",
                "Godzina zakoñczenia musi byæ póŸniejsza ni¿ godzina rozpoczêcia.",
                "OK");

            return;
        }


        // Dzisiejsza data + wybrane godziny
        DateTime startDateTime = DateTime.Today.Add(startTime);
        DateTime endDateTime = DateTime.Today.Add(endTime);


        // Przekazanie danych na nastêpn¹ stronê
        await Shell.Current.GoToAsync(
            $"{nameof(AsignTablet)}" +
            $"?klasa={Uri.EscapeDataString(selectedClass)}" +
            $"&start={Uri.EscapeDataString(startDateTime.ToString("O"))}" +
            $"&end={Uri.EscapeDataString(endDateTime.ToString("O"))}");
    }


    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
