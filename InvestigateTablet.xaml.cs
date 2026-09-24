using Microsoft.Maui.Controls.Shapes;
using TabletyRejestrApp.Models;

namespace TabletyRejestrApp;

[QueryProperty(nameof(TabletId), "tabletId")]
public partial class InvestigateTablet : ContentPage
{
    private int _tabletId;

    public string TabletId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                _tabletId = id;
            }
        }
    }
    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
    public InvestigateTablet()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_tabletId > 0)
        {
            await LoadTabletDataAsync();
        }
    }

    private async Task LoadTabletDataAsync()
    {
        try
        {
            // Pobranie wszystkich tabletów
            var tablets = await App.Database.GetTabletsAsync();

            var tablet = tablets.FirstOrDefault(x => x.Id == _tabletId);

            if (tablet == null)
            {
                await DisplayAlert(
                    "B³¹d",
                    "Nie znaleziono wybranego tabletu.",
                    "OK");

                return;
            }

            // Numer tabletu w nag³ówku
            TabletNumberLabel.Text = $"Tablet nr #{tablet.Numer}";

            // Wyczyœæ poprzedni¹ historiê
            HistoryLayout.Children.Clear();

            // Pobierz historiê
            var history = await App.Database.GetUsageHistoryAsync();

            // Tylko historia wybranego tabletu
            var tabletHistory = history
                .Where(x => x.TabletId == _tabletId)
                .OrderByDescending(x => x.DataRozpoczecia)
                .ToList();

            // Brak historii
            if (tabletHistory.Count == 0)
            {
                var noHistoryLabel = new Label
                {
                    Text = "Brak rejestrów dla tego tabletu.",
                    TextColor = Colors.White,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10, 30)
                };

                HistoryLayout.Children.Add(noHistoryLabel);

                return;
            }

            // Pobierz u¿ytkowników
            var users = await App.Database.GetStudentsAsync();

            // Tworzenie wpisów historii
            foreach (var usage in tabletHistory)
            {
                var user = users.FirstOrDefault(x => x.Id == usage.Id);

                string userName;

                if (user != null)
                {
                    userName = $"{user.Imie} {user.Nazwisko}";
                }
                else
                {
                    userName = "Nieznany u¿ytkownik";
                }

                var historyItem = CreateHistoryItem(
                    userName,
                    usage);

                HistoryLayout.Children.Add(historyItem);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê pobraæ historii.\n\n{ex.Message}",
                "OK");
        }
    }


    private Border CreateHistoryItem(
        string userName,
        TabletUsage usage)
    {
        string endDate;

        if (usage.DataZakonczenia.HasValue)
        {
            endDate = usage.DataZakonczenia.Value
                .ToString("dd.MM.yyyy HH:mm");
        }
        else
        {
            endDate = "Aktualnie u¿ywany";
        }

        var userLabel = new Label
        {
            Text = $"U¿ytkownik: {userName}",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        };

        var startLabel = new Label
        {
            Text = $"Rozpoczêcie: {usage.DataRozpoczecia:dd.MM.yyyy HH:mm}",
            FontSize = 17,
            TextColor = Colors.Black
        };

        var endLabel = new Label
        {
            Text = $"Zakoñczenie: {endDate}",
            FontSize = 17,
            TextColor = Colors.Black
        };

        var content = new VerticalStackLayout
        {
            Spacing = 5,
            Children =
            {
                userLabel,
                startLabel,
                endLabel
            }
        };

        var border = new Border
        {
            Padding = new Thickness(20),
            BackgroundColor = Colors.White,
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 15
            },
            Content = content
        };

        return border;
    }
}