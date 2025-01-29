/*
 * Constructor för användare
 * 
 */

using System.ComponentModel.DataAnnotations;

namespace Picasso.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        // Lista av arrangemang personen har ansvar för
        public List<Arrangement> arrangements { get; set; }
    }
}
