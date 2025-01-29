// Controller för login sidan

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Picasso.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Picasso.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index(string returnURL = "")
        {
            // Sparar den url som användaren har besökt så att användaren kan skickas tillbaka till denna efter inloggning
            @ViewData["ReturnUrl"] = returnURL;
            return View();
        }

        // Anropar denna när användaren skickar in information, tar även in information om vilken url som man ska skickas vidare till sen
        [HttpPost]
        public async Task<IActionResult> Index(User userModel, string returnURL = "")
        {
            // Kontrollerar om användarnamnet och lösenordet stämmer och därför returnerar true i CheckUser metoden
            if (CheckUser(userModel))
            {
                // Loggar in användaren
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, userModel.UserId));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // Skickar användaren till den sida som dom var på innan inloggningen, finns ingen sådan url så skickas denne till startsidan
                if (returnURL != "")
                {
                    return Redirect(returnURL);
                }
                else
                {
                    // Skickar den inloggade användaren till sidan där evenemang kan anordnas
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Visar ett felmeddelande om fel användarnamn och/eller lösenord har angets
                ViewBag.ErrorMessage = "Inloggningen inte godkänd";
                return View();
            }
        }

        /*
         * Kontrollerar om användarnamn och lösenord stämmer.
         * Hårdkodade användarnamn och lösenord rekommenderas EJ men används här.
         */
        private bool CheckUser(User userModel)
        {
            using (UserContext db = new UserContext())
            {
                User userToLogIn = db.Users.Find(userModel.UserId);
                if (userModel.Password == userToLogIn.Password)
                    return true;
                else
                    return false;
            }
        }

        // Sida för att lägga till event, endast inloggade användare har tillgång till denna
        public IActionResult CreateUser()
        {
            return View();
        }

        // Skapar en ny användare baserat på de uppgifter som har skickats med i formuläret och lägger till i databasen
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            using (UserContext db = new UserContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Låter användaren se uppgifterna för sitt konto
        [Authorize]
        public IActionResult ViewUser()
        {
            // Hämtar namn värdet från den inloggade användaren
            var currentUser = User.FindFirst(ClaimTypes.Name).Value;

            using (UserContext db = new UserContext())
            {
                /* 
                 * Gör en lista av användare för att få med värderna i arrangemang listan,
                 * letar sedan efter den inloggade användaren och använder detta objekt
                 * https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.find?view=net-8.0
                 */
                var userList = db.Users.Include(m => m.arrangements).ToList();
                User user = userList.Find(m => m.UserId.Contains(currentUser));
                return View(user);
            }
        }

        [Authorize]
        // Går in på sidan för att redigera användarens uppgifter, användaren måste vara inloggad för att få tillgång till denna
        public IActionResult EditUser()
        {
            // Hämtar namn värdet från den inloggade användaren
            var currentUser = User.FindFirst(ClaimTypes.Name).Value;

            using (UserContext db = new UserContext())
            {
                var userList = db.Users.Include(m => m.arrangements).ToList();
                User user = userList.Find(m => m.UserId.Contains(currentUser));
                return View(user);
            }
        }

        // Uppdaterar användaren i databasen med de uppgifter som användaren har ändrat och skickar sedan tillbaka denna till sidan för att se detaljer om sitt konto
        [HttpPost]
        public IActionResult EditUser(User user)
        {
            using (UserContext db = new UserContext())
            {
                db.Update(user);
                db.SaveChanges();

                return RedirectToAction("ViewUser");
            }
        }

        // Låter användaren ta bort sitt konto, användaren måste vara inloggad för att kunna göra detta
        [Authorize]
        public IActionResult DeleteUser(string id)
        {
            using (UserContext db = new UserContext())
            {
                var userList = db.Users.Include(m => m.arrangements).ToList();
                User user = userList.Find(m => m.UserId.Contains(id));
                return View(user);
            }
        }

        // Tar bort användaren från databasen och tar sedan användaren tillbaka till index sidan för att se alla användare
        [HttpPost]
        public async Task<IActionResult> DeleteUser()
        {
            // Logga ut användaren, annars blir det fel när sidan laddas och användaren är borttagen
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Tar fram den inloggade användaren
            var currentUser = User.FindFirst(ClaimTypes.Name).Value;

            using (UserContext db = new UserContext())
            {
                // Letar upp den inloggade användaren i databasen och tar bort denna
                var userList = db.Users.Include(m => m.arrangements).ToList();
                User user = userList.Find(m => m.UserId.Contains(currentUser));

                db.Users.Remove(user);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
        }

        // Loggar ut användaren och skickar tillbaka till startsidan
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
