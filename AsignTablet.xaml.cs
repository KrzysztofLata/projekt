using Microsoft.Maui.Controls.Shapes;
using TabletyRejestrApp.Models;

namespace TabletyRejestrApp;

[QueryProperty(nameof(Klasa), "klasa")]
[QueryProperty(nameof(Start), "start")]
[QueryProperty(nameof(End), "end")]
[QueryProperty(nameof(Qr), "qr")]
public partial class AsignTablet : ContentPage
{
    private string _klasa = string.Empty;

    private DateTime _start;
    private DateTime _end;

    private Tablet? _scannedTablet;

    // Przypisania tworzone podczas tworzenia lekcji
    private readonly List<TabletUsage> _usages = new();

    // Uczniowie należący do wybranej klasy
    private List<Student> _classStudents = new();


    // ============================================================
    // DANE Z SETUPRECORD
    // ============================================================

    public string Klasa
    {
        get => _klasa;

        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _klasa = Uri.UnescapeDataString(value);
            }
        }
    }


    public string Start
    {
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime.TryParse(
                    Uri.UnescapeDataString(value),
                    out _start);
            }
        }
    }


    public string End
    {
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime.TryParse(
                    Uri.UnescapeDataString(value),
                    out _end);
            }
        }
    }


    // ============================================================
    // WYNIK SKANOWANIA QR
    // ============================================================

    public string Qr
    {
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string qrValue =
                Uri.UnescapeDataString(value);

            MainThread.BeginInvokeOnMainThread(
                async () =>
                {
                    await ProcessQrCodeAsync(qrValue);
                });
        }
    }


    // ============================================================
    // KONSTRUKTOR
    // ============================================================

    public AsignTablet()
    {
        InitializeComponent();
    }


    // ============================================================
    // ON APPEARING
    // ============================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        UpdateLessonInformation();

        await LoadStudentsAsync();
    }


    // ============================================================
    // INFORMACJE O LEKCJI
    // ============================================================

    private void UpdateLessonInformation()
    {
        if (!string.IsNullOrWhiteSpace(_klasa))
        {
            ClassLabel.Text =
                $"Klasa: {_klasa}";
        }

        if (_start != default &&
            _end != default)
        {
            TimeLabel.Text =
                $"Czas: {_start:HH:mm} - {_end:HH:mm}";
        }
    }


    // ============================================================
    // UCZNIOWIE
    // ============================================================

    private async Task LoadStudentsAsync()
    {
        try
        {
            StudentPicker.Items.Clear();

            var students =
                await App.Database.GetStudentsAsync();


            _classStudents = students
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Klasa) &&
                    x.Klasa.Equals(
                        _klasa,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Nazwisko)
                .ThenBy(x => x.Imie)
                .ToList();


            foreach (var student in _classStudents)
            {
                StudentPicker.Items.Add(
                    $"{student.Imie} {student.Nazwisko}");
            }


            if (StudentPicker.Items.Count > 0)
            {
                StudentPicker.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Błąd",
                $"Nie udało się pobrać uczniów.\n\n{ex.Message}",
                "OK");
        }
    }


    // ============================================================
    // SKANOWANIE QR
    // ============================================================

    private async void ScanButton_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(QrScannerPage));
    }


    // ============================================================
    // OBSŁUGA QR
    // ============================================================

    private async Task ProcessQrCodeAsync(
        string qrValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(qrValue))
                return;


            // ----------------------------------------------------
            // FORMAT QR
            // ----------------------------------------------------

            if (!qrValue.StartsWith(
                    "TABLET:",
                    StringComparison.OrdinalIgnoreCase))
            {
                await DisplayAlert(
                    "Nieprawidłowy kod",
                    "Zeskanowany kod nie jest kodem tabletu.\n\n" +
                    "Poprawny format:\n" +
                    "TABLET:T-001",
                    "OK");

                return;
            }


            // ----------------------------------------------------
            // NUMER TABLETU
            // ----------------------------------------------------

            string tabletNumber =
                qrValue
                    .Substring("TABLET:".Length)
                    .Trim();


            if (string.IsNullOrWhiteSpace(tabletNumber))
            {
                await DisplayAlert(
                    "Nieprawidłowy kod",
                    "Kod QR nie zawiera numeru tabletu.",
                    "OK");

                return;
            }


            // ----------------------------------------------------
            // TABLETY Z BAZY
            // ----------------------------------------------------

            var tablets =
                await App.Database.GetTabletsAsync();


            // ----------------------------------------------------
            // SZUKANIE TABLETU
            // ----------------------------------------------------

            var tablet =
                tablets.FirstOrDefault(x =>
                    x.Numer.Equals(
                        tabletNumber,
                        StringComparison.OrdinalIgnoreCase));


            if (tablet == null)
            {
                await DisplayAlert(
                    "Nie znaleziono tabletu",
                    $"Tablet {tabletNumber} " +
                    "nie znajduje się w bazie danych.",
                    "OK");

                return;
            }


            // ----------------------------------------------------
            // SPRAWDZENIE CZY TABLET JEST JUŻ PRZYPISANY
            // ----------------------------------------------------

            bool tabletAlreadyUsed =
                _usages.Any(x =>
                    x.TabletId == tablet.Id);


            if (tabletAlreadyUsed)
            {
                await DisplayAlert(
                    "Tablet już przypisany",
                    $"Tablet #{tablet.Numer} " +
                    "został już przypisany.",
                    "OK");

                return;
            }


            // ----------------------------------------------------
            // ZAPIS TABLETU
            // ----------------------------------------------------

            _scannedTablet = tablet;


            ScannedTabletLabel.Text =
                $"Tablet #{tablet.Numer}";
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Błąd skanowania",
                ex.ToString(),
                "OK");
        }
    }


    // ============================================================
    // DODAWANIE TABLETU + UCZNIA
    // ============================================================

    private async void AddButton_Clicked(
        object sender,
        EventArgs e)
    {
        // --------------------------------------------------------
        // SPRAWDZENIE UCZNIA
        // --------------------------------------------------------

        if (StudentPicker.SelectedIndex < 0)
        {
            await DisplayAlert(
                "Brak ucznia",
                "Wybierz ucznia.",
                "OK");

            return;
        }


        // --------------------------------------------------------
        // SPRAWDZENIE TABLETU
        // --------------------------------------------------------

        if (_scannedTablet == null)
        {
            await DisplayAlert(
                "Brak tabletu",
                "Najpierw zeskanuj kod QR tabletu.",
                "OK");

            return;
        }


        // --------------------------------------------------------
        // WYBRANY UCZEŃ
        // --------------------------------------------------------

        Student student =
            _classStudents[
                StudentPicker.SelectedIndex];


        // --------------------------------------------------------
        // SPRAWDZENIE TABLETU
        // --------------------------------------------------------

        bool tabletAlreadyUsed =
            _usages.Any(x =>
                x.TabletId == _scannedTablet.Id);


        if (tabletAlreadyUsed)
        {
            await DisplayAlert(
                "Tablet już przypisany",
                "Ten tablet został już przypisany.",
                "OK");

            return;
        }


        // --------------------------------------------------------
        // SPRAWDZENIE UCZNIA
        // --------------------------------------------------------

        bool studentAlreadyUsed =
            _usages.Any(x =>
                x.StudentId == student.Id);


        if (studentAlreadyUsed)
        {
            await DisplayAlert(
                "Uczeń już ma tablet",
                "Ten uczeń ma już przypisany tablet.",
                "OK");

            return;
        }


        // ========================================================
        // UTWORZENIE TABLET USAGE
        // ========================================================

        var usage =
            new TabletUsage
            {
                TabletId =
                    _scannedTablet.Id,

                StudentId =
                    student.Id,

                DataRozpoczecia =
                    _start,

                DataZakonczenia =
                    _end
            };


        _usages.Add(usage);


        // --------------------------------------------------------
        // WYŚWIETLENIE
        // --------------------------------------------------------

        AddUsageToView(
            usage,
            student,
            _scannedTablet);


        // --------------------------------------------------------
        // WYCZYSZCZENIE TABLETU
        // --------------------------------------------------------

        _scannedTablet = null;

        ScannedTabletLabel.Text =
            "Nie zeskanowano tabletu";
    }


    // ============================================================
    // WYŚWIETLANIE PRZYPISANIA
    // ============================================================

    private void AddUsageToView(
        TabletUsage usage,
        Student student,
        Tablet tablet)
    {
        var studentLabel =
            new Label
            {
                Text =
                    $"{student.Imie} {student.Nazwisko}",

                TextColor =
                    Colors.Black,

                FontAttributes =
                    FontAttributes.Bold,

                FontSize = 16
            };


        var tabletLabel =
            new Label
            {
                Text =
                    $"Tablet #{tablet.Numer}",

                TextColor =
                    Colors.Black,

                FontSize = 15
            };


        var timeLabel =
            new Label
            {
                Text =
                    $"{usage.DataRozpoczecia:HH:mm} - " +
                    $"{usage.DataZakonczenia:HH:mm}",

                TextColor =
                    Colors.Gray,

                FontSize = 13
            };


        var deleteButton =
            new Button
            {
                Text = "Usuń",

                BackgroundColor =
                    Colors.LightPink,

                TextColor =
                    Colors.Black,

                FontSize = 13,

                WidthRequest = 70,

                HeightRequest = 40
            };


        var informationLayout =
            new VerticalStackLayout
            {
                Spacing = 3,

                Children =
                {
                    studentLabel,
                    tabletLabel,
                    timeLabel
                }
            };


        var content =
            new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    },

                    new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    }
                }
            };


        content.Add(
            informationLayout,
            0,
            0);


        content.Add(
            deleteButton,
            1,
            0);


        var border =
            new Border
            {
                BackgroundColor =
                    Colors.White,

                Padding =
                    new Thickness(15),

                Stroke =
                    Colors.LightGray,

                StrokeThickness = 1,

                StrokeShape =
                    new RoundRectangle
                    {
                        CornerRadius = 10
                    },

                Content =
                    content
            };


        // ========================================================
        // USUWANIE
        // ========================================================

        deleteButton.Clicked += async (sender, e) =>
        {
            bool result =
                await DisplayAlert(
                    "Usuwanie",
                    $"Czy usunąć przypisanie?\n\n" +
                    $"{student.Imie} {student.Nazwisko}\n" +
                    $"Tablet #{tablet.Numer}",
                    "Tak",
                    "Nie");


            if (!result)
                return;


            _usages.Remove(
                usage);


            AssignmentsLayout.Children.Remove(
                border);
        };


        // ========================================================
        // DODANIE DO PRAWEJ STRONY
        // ========================================================

        AssignmentsLayout.Children.Add(
            border);
    }


    // ============================================================
    // UTWÓRZ
    // ============================================================

    private async void Utworz_Clicked(
        object sender,
        EventArgs e)
    {
        if (_usages.Count == 0)
        {
            await DisplayAlert(
                "Brak przypisań",
                "Dodaj przynajmniej jedną parę tablet - uczeń.",
                "OK");

            return;
        }


        string message =
            $"Utworzono {_usages.Count} przypisań.\n\n";


        var students =
            await App.Database.GetStudentsAsync();

        var tablets =
            await App.Database.GetTabletsAsync();


        foreach (var usage in _usages)
        {
            var student =
                students.FirstOrDefault(
                    x => x.Id == usage.StudentId);

            var tablet =
                tablets.FirstOrDefault(
                    x => x.Id == usage.TabletId);


            if (student == null ||
                tablet == null)
            {
                continue;
            }


            message +=
                $"Tablet #{tablet.Numer} → " +
                $"{student.Imie} {student.Nazwisko}\n";
        }


        await DisplayAlert(
            "Gotowe",
            message,
            "OK");


        // ========================================================
        // NA TYM ETAPIE NIE ZAPISUJEMY JESZCZE DO BAZY
        // ========================================================

        await Shell.Current.GoToAsync(
            "//MainPage");
    }


    // ============================================================
    // BACK
    // ============================================================

    private async void Back_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void ZakonczRejestr_Clicked(
    object sender,
    EventArgs e)
    {
        try
        {
            if (_usages.Count == 0)
            {
                await DisplayAlert(
                    "Brak danych",
                    "Nie dodano żadnego przypisania.",
                    "OK");

                return;
            }

            bool confirm = await DisplayAlert(
                "Zakończenie rejestru",
                $"Czy na pewno chcesz zakończyć rejestr?\n\n" +
                $"Klasa: {_klasa}\n" +
                $"Liczba przypisań: {_usages.Count}",
                "Zakończ",
                "Anuluj");

            if (!confirm)
                return;


            // ========================================================
            // ZAPIS DO BAZY
            // ========================================================

            foreach (var usage in _usages)
            {
                await App.Database.AddTabletUsageAsync(usage);
            }


            // ========================================================
            // POTWIERDZENIE
            // ========================================================

            await DisplayAlert(
                "Rejestr zakończony",
                $"Zapisano {_usages.Count} przypisań.",
                "OK");


            // ========================================================
            // POWRÓT DO STRONY GŁÓWNEJ
            // ========================================================

            await Shell.Current.GoToAsync(
                "//MainPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Błąd zapisu",
                $"Nie udało się zapisać rejestru.\n\n{ex.Message}",
                "OK");
        }
    }
}
