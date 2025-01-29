/*
 * Controller för sidan för event och anordning av event
 */
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picasso.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Picasso.Controllers
{
    public class ArrangementController : Controller
    {
        public IActionResult Index()
        {
            // Skapar en lista av objekten Evenemang i databasen inklusive information om den ansvarige för evenemanget
            using (ArrangementContext db = new ArrangementContext())
            {
                // Inkluderar information om användare som är länkade via främmande nycklar
                var evenemangLista = db.Arrangements.Include(m => m.Users).ToList();
                return View(evenemangLista);
            }
        }

        // Sida för att lägga till event, endast inloggade användare har tillgång till denna
        [Authorize]
        public IActionResult CreateArrangement()
        {
            return View();
        }

        /* 
         * Anropas när användaren har fyllt i uppgifter för ett nytt evenemang,
         * lägger till det nya evenemanget och tar tillbaka användaren till index sidan för event
         */
        [HttpPost]
        public IActionResult CreateArrangement(Arrangement newArrangement)
        {
            using (ArrangementContext db = new ArrangementContext())
            {
                db.Arrangements.Add(newArrangement);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        // Går in på sidan för att redigera ett event som användaren har valt, användaren måste vara inloggad för att få tillgång
        public IActionResult EditArrangement(int Id)
        {
            using (ArrangementContext db = new ArrangementContext())
            {
                Arrangement arrangement = db.Arrangements.Find(Id);
                return View(arrangement);
            }
        }

        // Uppdaterar evenemanget i databasen med de uppgifter som användaren har ändrat och skickar sedan tillbaka användaren till index sidan för event
        [HttpPost]
        public IActionResult EditArrangement(Arrangement arrangement)
        {
            // Hämtar namn värdet från den inloggade användaren
            var currentUser = User.FindFirst(ClaimTypes.Name).Value;

            // Ändrar bara värdet om rätt användare är inloggad
            if (arrangement.UserId == currentUser)
            {
                using (ArrangementContext db = new ArrangementContext())
                {
                    db.Update(arrangement);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            } else
            {
                // Visar ett felmeddelande om fel användare är inloggad
                ViewBag.ErrorMessage = "Du har inte rättighet att ändra detta arrangemang, logga in på rätt användare och försök igen";
                return View();
            }
        }

        // Låter användaren ta bort ett evenemang, användaren måste vara inloggad för att kunna göra detta
        [Authorize]
        public IActionResult DeleteArrangement(int id)
        {
            using (ArrangementContext db = new ArrangementContext())
            {
                Arrangement arrangement = db.Arrangements.Find(id);
                return View(arrangement);
            }
        }

        // Tar bort evenemanget från databasen och tar sedan användaren tillbaka till index sidan för event
        [HttpPost]
        public IActionResult DeleteArrangement(Arrangement arrangement)
        {
            // Hämtar namn värdet från den inloggade användaren
            var currentUser = User.FindFirst(ClaimTypes.Name).Value;

            if (arrangement.UserId == currentUser)
            {
                using (ArrangementContext db = new ArrangementContext())
                {
                    db.Arrangements.Remove(arrangement);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Visar ett felmeddelande om fel användare är inloggad
                ViewBag.ErrorMessage = "Du har inte rättighet att ta bort detta arrangemang, logga in på rätt användare och försök igen";
                return View(arrangement);
            }
        }
    }
}
