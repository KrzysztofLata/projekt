using TabletyRejestrApp.Data;

namespace TabletyRejestrApp
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }

        public App()
        {
            Database = new DatabaseService();

            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            await Database.InitializeAsync();
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}