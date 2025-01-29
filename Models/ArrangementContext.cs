// Klass för de arrangemang som lagras i databasen

using Microsoft.EntityFrameworkCore;

namespace Picasso.Models
{
    public class ArrangementContext : DbContext
    {
        // Skapar en lista med användarna och evenemang, båda måste skapas här för att hela databasen ska uppdateras
        public DbSet<Arrangement> Arrangements { get; set; }
        public DbSet<User> Users { get; set; }

        public ArrangementContext()
        {
            // Skapar databasen om denna inte redan existerar.
            Database.EnsureCreated();
        }

        // Berättar för systemet vilken databas som ska användas
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Filen skapas i samma mapp som projektet
            optionsBuilder.UseSqlite("Data Source=picassoData.db");
        }
    }
}