// Constructor för evenemang objektet

namespace Picasso.Models
{
    public class Arrangement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Sätts DateTime via DB Browser så anges tiden enligt ÅÅÅÅ-MM-DD TT-MM-SS
        public DateTime Date { get; set; }
        public string Description { get; set; }
        // Främmande nyckel referens
        public string UserId { get; set; }
        public User Users { get; set; }
    }
}
