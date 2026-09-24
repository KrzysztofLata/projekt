using SQLite;
using TabletyRejestrApp.Models;

namespace TabletyRejestrApp.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        private bool _initialized = false;


        // ============================================================
        // KONSTRUKTOR
        // ============================================================

        public DatabaseService()
        {
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "tablety.db3");

            _database =
                new SQLiteAsyncConnection(dbPath);
        }


        // ============================================================
        // INICJALIZACJA BAZY
        // ============================================================

        public async Task InitializeAsync()
        {
            // Nie inicjalizuj bazy drugi raz
            if (_initialized)
                return;


            // ========================================================
            // TWORZENIE TABEL
            // ========================================================

            await _database.CreateTableAsync<Tablet>();

            await _database.CreateTableAsync<Student>();

            await _database.CreateTableAsync<TabletUsage>();


            // ========================================================
            // DANE POCZĄTKOWE - TABLETY
            // ========================================================

            var tabletsCount =
                await _database
                    .Table<Tablet>()
                    .CountAsync();


            if (tabletsCount == 0)
            {
                await _database.InsertAllAsync(
                    new List<Tablet>
                    {
                        new Tablet
                        {
                            Numer = "T-001",
                            Nazwa = "Tablet 1",
                            Aktywny = true
                        },

                        new Tablet
                        {
                            Numer = "T-002",
                            Nazwa = "Tablet 2",
                            Aktywny = true
                        },

                        new Tablet
                        {
                            Numer = "T-003",
                            Nazwa = "Tablet 3",
                            Aktywny = true
                        }
                    });
            }


            // ========================================================
            // DANE POCZĄTKOWE - UCZNIOWIE
            // ========================================================

            var studentsCount =
                await _database
                    .Table<Student>()
                    .CountAsync();


            if (studentsCount == 0)
            {
                await _database.InsertAllAsync(
                    new List<Student>
                    {
                        new Student
                        {
                            Imie = "Sebastian",
                            Nazwisko = "Kusior",
                            Klasa = "5TP"
                        },

                        new Student
                        {
                            Imie = "Krzysztof",
                            Nazwisko = "Łata",
                            Klasa = "5TP"
                        }
                    });
            }


            _initialized = true;
        }


        // ============================================================
        // TABLETY
        // ============================================================

        public async Task<List<Tablet>> GetTabletsAsync()
        {
            await InitializeAsync();

            return await _database
                .Table<Tablet>()
                .ToListAsync();
        }


        public async Task<int> AddTabletAsync(
            Tablet tablet)
        {
            await InitializeAsync();

            return await _database
                .InsertAsync(tablet);
        }


        public async Task<int> UpdateTabletAsync(
            Tablet tablet)
        {
            await InitializeAsync();

            return await _database
                .UpdateAsync(tablet);
        }


        public async Task<int> DeleteTabletAsync(
            Tablet tablet)
        {
            await InitializeAsync();

            return await _database
                .DeleteAsync(tablet);
        }


        // ============================================================
        // UCZNIOWIE
        // ============================================================

        public async Task<List<Student>> GetStudentsAsync()
        {
            await InitializeAsync();

            return await _database
                .Table<Student>()
                .ToListAsync();
        }


        public async Task<int> AddUserAsync(
            Student user)
        {
            await InitializeAsync();

            return await _database
                .InsertAsync(user);
        }


        public async Task<int> UpdateStudentAsync(
            Student student)
        {
            await InitializeAsync();

            return await _database
                .UpdateAsync(student);
        }


        public async Task<int> DeleteStudentAsync(
            Student student)
        {
            await InitializeAsync();

            return await _database
                .DeleteAsync(student);
        }


        // ============================================================
        // TABLET USAGE
        // ============================================================

        /// <summary>
        /// Dodaje nowy rekord użycia tabletu.
        /// </summary>
        public async Task<int> AddTabletUsageAsync(
            TabletUsage usage)
        {
            await InitializeAsync();

            return await _database
                .InsertAsync(usage);
        }


        /// <summary>
        /// Rozpoczyna używanie tabletu.
        /// </summary>
        public async Task<int> StartUsageAsync(
            TabletUsage usage)
        {
            await InitializeAsync();

            return await _database
                .InsertAsync(usage);
        }


        /// <summary>
        /// Kończy używanie tabletu.
        /// </summary>
        public async Task<int> EndUsageAsync(
            int usageId,
            DateTime dataZakonczenia)
        {
            await InitializeAsync();

            var usage =
                await _database
                    .Table<TabletUsage>()
                    .Where(x => x.Id == usageId)
                    .FirstOrDefaultAsync();


            if (usage == null)
                return 0;


            usage.DataZakonczenia =
                dataZakonczenia;


            return await _database
                .UpdateAsync(usage);
        }


        /// <summary>
        /// Pobiera całą historię używania tabletów.
        /// </summary>
        public async Task<List<TabletUsage>>
            GetUsageHistoryAsync()
        {
            await InitializeAsync();

            return await _database
                .Table<TabletUsage>()
                .OrderByDescending(
                    x => x.DataRozpoczecia)
                .ToListAsync();
        }


        /// <summary>
        /// Pobiera historię konkretnego tabletu.
        /// </summary>
        public async Task<List<TabletUsage>>
            GetUsageHistoryForTabletAsync(
                int tabletId)
        {
            await InitializeAsync();

            return await _database
                .Table<TabletUsage>()
                .Where(x => x.TabletId == tabletId)
                .OrderByDescending(
                    x => x.DataRozpoczecia)
                .ToListAsync();
        }


        /// <summary>
        /// Pobiera historię konkretnego ucznia.
        /// </summary>
        public async Task<List<TabletUsage>>
            GetUsageHistoryForStudentAsync(
                int studentId)
        {
            await InitializeAsync();

            return await _database
                .Table<TabletUsage>()
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(
                    x => x.DataRozpoczecia)
                .ToListAsync();
        }


        /// <summary>
        /// Usuwa rekord historii.
        /// </summary>
        public async Task<int> DeleteTabletUsageAsync(
            TabletUsage usage)
        {
            await InitializeAsync();

            return await _database
                .DeleteAsync(usage);
        }
    }
}
