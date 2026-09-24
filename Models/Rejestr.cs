using SQLite;

namespace TabletyRejestrApp.Models
{
    public class TabletUsage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TabletId { get; set; }

        public int StudentId { get; set; }

        public DateTime DataRozpoczecia { get; set; }

        public DateTime? DataZakonczenia { get; set; }
    }
}