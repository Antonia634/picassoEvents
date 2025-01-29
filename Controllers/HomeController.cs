/*
 * Controller för hemsidan
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Picasso.Models;

namespace Picasso.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Söker efter objektet med id 1 i databasen med evenemang så att denna kan visas på första sidan
            using (ArrangementContext db = new ArrangementContext())
            {
                Arrangement arrangement = db.Arrangements.Find(1);
                return View(arrangement);
            }
        }
    }
}
