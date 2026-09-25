using TabletyRejestrApp.Data;
using TabletyRejestrApp.Models;

namespace TabletyRejestrApp;

public partial class StudentsDatabase : ContentPage
{
    private readonly DatabaseService _databaseService;

    private List<Student> _students = new();

    private List<string> _classes = new();


    // ============================================================
    // KONSTRUKTOR
    // ============================================================

    public StudentsDatabase()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }


    // ============================================================
    // POJAWIENIE SIÊ STRONY
    // ============================================================

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadDataAsync();
    }


    // ============================================================
    // WCZYTANIE WSZYSTKICH DANYCH
    // ============================================================

    private async Task LoadDataAsync()
    {
        try
        {
            await _databaseService.InitializeAsync();

            await LoadStudentsAsync();

            await LoadClassesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê wczytaæ danych.\n\n{ex.Message}",
                "OK");
        }
    }


    // ============================================================
    // WCZYTANIE UCZNIÓW
    // ============================================================

    private async Task LoadStudentsAsync()
    {
        var allStudents =
            await _databaseService
                .GetStudentsAsync();


        /*
         * Rekordy klas maj¹:
         *
         * Imie = ""
         * Nazwisko = ""
         *
         * Dlatego pokazujemy na liœcie
         * tylko prawdziwych uczniów.
         */

        _students =
            allStudents
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Imie) &&
                    !string.IsNullOrWhiteSpace(x.Nazwisko))
                .ToList();


        StudentsCollectionView.ItemsSource =
            _students;
    }


    // ============================================================
    // WCZYTANIE KLAS
    // ============================================================

    private async Task LoadClassesAsync()
    {
        var allStudents =
            await _databaseService
                .GetStudentsAsync();


        /*
         * Ka¿dy Student ma pole Klasa.
         *
         * Pobieramy wszystkie niepuste klasy
         * i usuwamy powtarzaj¹ce siê wartoœci.
         */

        _classes =
            allStudents
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Klasa))
                .Select(x =>
                    x.Klasa.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();


        KlasaPicker.ItemsSource =
            _classes;
    }


    // ============================================================
    // UTWÓRZ KLASÊ
    // ============================================================

    private async void AddClass_Clicked(
        object sender,
        EventArgs e)
    {
        string className =
            KlasaEntry.Text?.Trim() ?? "";


        // --------------------------------------------
        // SPRAWDZENIE NAZWY
        // --------------------------------------------

        if (string.IsNullOrWhiteSpace(className))
        {
            await DisplayAlert(
                "Brak danych",
                "Wpisz nazwê klasy.",
                "OK");

            return;
        }


        // --------------------------------------------
        // POBRANIE DANYCH
        // --------------------------------------------

        var allStudents =
            await _databaseService
                .GetStudentsAsync();


        // --------------------------------------------
        // SPRAWDZENIE CZY KLASA ISTNIEJE
        // --------------------------------------------

        bool classExists =
            allStudents.Any(
                x =>
                    !string.IsNullOrWhiteSpace(x.Klasa) &&
                    string.Equals(
                        x.Klasa.Trim(),
                        className,
                        StringComparison.OrdinalIgnoreCase));


        if (classExists)
        {
            await DisplayAlert(
                "Klasa ju¿ istnieje",
                $"Klasa „{className}” jest ju¿ w bazie.",
                "OK");

            return;
        }


        // --------------------------------------------
        // UTWORZENIE REKORDU KLASY
        // --------------------------------------------

        /*
         * Nie mamy osobnego Class.cs.
         *
         * Dlatego klasa jest zapisana jako
         * specjalny rekord Student.
         *
         * Puste Imie + puste Nazwisko oznacza,
         * ¿e jest to rekord klasy.
         */

        var classRecord =
            new Student
            {
                Imie = "",
                Nazwisko = "",
                Klasa = className
            };


        try
        {
            // Zapis do SQLite
            await _databaseService
                .AddUserAsync(classRecord);


            // Wyczyœæ pole
            KlasaEntry.Text = "";


            // Odœwie¿ klasy
            await LoadClassesAsync();


            // Od razu wybierz utworzon¹ klasê
            KlasaPicker.SelectedItem =
                _classes.FirstOrDefault(
                    x => string.Equals(
                        x,
                        className,
                        StringComparison.OrdinalIgnoreCase));


            await DisplayAlert(
                "Gotowe",
                $"Utworzono klasê „{className}”.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê utworzyæ klasy.\n\n{ex.Message}",
                "OK");
        }
    }


    // ============================================================
    // DODAJ UCZNIA
    // ============================================================

    private async void AddStudent_Clicked(
        object sender,
        EventArgs e)
    {
        string imie =
            ImieEntry.Text?.Trim() ?? "";

        string nazwisko =
            NazwiskoEntry.Text?.Trim() ?? "";

        string klasa =
            KlasaPicker.SelectedItem?.ToString() ?? "";


        // --------------------------------------------
        // SPRAWDZENIE IMIENIA
        // --------------------------------------------

        if (string.IsNullOrWhiteSpace(imie))
        {
            await DisplayAlert(
                "Brak danych",
                "Wpisz imiê ucznia.",
                "OK");

            return;
        }


        // --------------------------------------------
        // SPRAWDZENIE NAZWISKA
        // --------------------------------------------

        if (string.IsNullOrWhiteSpace(nazwisko))
        {
            await DisplayAlert(
                "Brak danych",
                "Wpisz nazwisko ucznia.",
                "OK");

            return;
        }


        // --------------------------------------------
        // SPRAWDZENIE KLASY
        // --------------------------------------------

        if (string.IsNullOrWhiteSpace(klasa))
        {
            await DisplayAlert(
                "Brak klasy",
                "Najpierw utwórz klasê, a nastêpnie wybierz j¹ z listy.",
                "OK");

            return;
        }


        // --------------------------------------------
        // UTWORZENIE UCZNIA
        // --------------------------------------------

        var student =
            new Student
            {
                Imie = imie,
                Nazwisko = nazwisko,
                Klasa = klasa
            };


        try
        {
            await _databaseService
                .AddUserAsync(student);


            // ----------------------------------------
            // CZYSZCZENIE FORMULARZA
            // ----------------------------------------

            ImieEntry.Text = "";

            NazwiskoEntry.Text = "";


            // Zachowujemy wybran¹ klasê
            // KlasaPicker.SelectedItem = null;


            // ----------------------------------------
            // ODŒWIE¯ENIE LISTY
            // ----------------------------------------

            await LoadStudentsAsync();

            await LoadClassesAsync();


            // Ponownie ustawiamy klasê
            KlasaPicker.SelectedItem =
                _classes.FirstOrDefault(
                    x => string.Equals(
                        x,
                        klasa,
                        StringComparison.OrdinalIgnoreCase));


            await DisplayAlert(
                "Gotowe",
                $"Dodano ucznia:\n\n" +
                $"{imie} {nazwisko}\n" +
                $"Klasa: {klasa}",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê dodaæ ucznia.\n\n{ex.Message}",
                "OK");
        }
    }


    // ============================================================
    // USUWANIE UCZNIA
    // ============================================================

    private async void DeleteStudent_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;


        if (button.CommandParameter is not Student student)
            return;


        // --------------------------------------------
        // POTWIERDZENIE
        // --------------------------------------------

        bool confirm =
            await DisplayAlert(
                "Usuñ ucznia",
                $"Czy na pewno chcesz usun¹æ:\n\n" +
                $"{student.Imie} {student.Nazwisko}\n" +
                $"Klasa: {student.Klasa}",
                "Usuñ",
                "Anuluj");


        if (!confirm)
            return;


        // --------------------------------------------
        // USUNIÊCIE
        // --------------------------------------------

        try
        {
            await _databaseService
                .DeleteStudentAsync(student);


            await LoadStudentsAsync();

            await LoadClassesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                $"Nie uda³o siê usun¹æ ucznia.\n\n{ex.Message}",
                "OK");
        }
    }


    // ============================================================
    // POWRÓT
    // ============================================================

    private async void Back_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
