using SQLite;

namespace TabletyRejestrApp.Models
{
    public class Tablet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Numer { get; set; }

        public string Nazwa { get; set; }

        public bool Aktywny { get; set; }
    }
}