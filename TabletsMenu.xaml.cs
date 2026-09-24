using TabletyRejestrApp.Models;

namespace TabletyRejestrApp;

public partial class TabletsMenu : ContentPage
{
	public TabletsMenu()
	{
		InitializeComponent();
	}

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadTabletsAsync();
    }

    private async Task LoadTabletsAsync()
    {
        try
        {
            var tablets = await App.Database.GetTabletsAsync();

            TabletsLayout.Children.Clear();

            foreach (var tablet in tablets)
            {
                var cluster = CreateTabletCluster(tablet);

                TabletsLayout.Children.Add(cluster);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê wczytaæ tabletów.\n\n{ex.Message}",
                "OK");
        }
    }

    private VerticalStackLayout CreateTabletCluster(Tablet tablet)
    {
        var button = new Button
        {
            WidthRequest = 200,
            HeightRequest = 200,
            Padding = 35,
            BackgroundColor = Colors.LightPink,
            ImageSource = "tablet.png",
            CornerRadius = 5
        };

        button.Shadow = new Shadow
        {
            Brush = Colors.Black,
            Offset = new Point(10, 10),
            Radius = 20,
            Opacity = 0.5f
        };

        button.Clicked += async (sender, e) =>
        {
            await TabletClicked(tablet);
        };

        var label = new Label
        {
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(10),
            TextColor = Colors.White,
            FontSize = 25,
            Text = $"Tablet #{tablet.Numer}"
        };

        var cluster = new VerticalStackLayout
        {
            Margin = new Thickness(10),
            Children =
            {
                button,
                label
            }
        };

        return cluster;
    }

    private async Task TabletClicked(Tablet tablet)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(InvestigateTablet)}?tabletId={tablet.Id}");
    }
}