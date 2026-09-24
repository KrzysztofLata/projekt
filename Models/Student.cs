using SQLite;

namespace TabletyRejestrApp.Models
{
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Klasa { get; set; }
    }
}