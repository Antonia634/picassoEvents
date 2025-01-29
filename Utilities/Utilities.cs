/*
 * Innehåller en klass som kontrollerar om den valda länken är den aktiva sidan så att "active" klassen kan
 * läggas till och användaren ser tydligare vilken sida dom besöker.
 * 
 * http://www.codingeverything.com/2014/05/mvcbootstrapactivenavbar.html
 */

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Picasso.Utilities
{
    public static class Utilities
    {
        // Kontrollerar om den valda länken är den aktiva sidan
        public static string IsActive(this IHtmlHelper htmlHelper, string control, string action)
        {
            // Skapar en variabel som hämtar data med RouteData som innehåller en mängd information som kan hämtas genom att söka efter specifika Values
            var routeData = htmlHelper.ViewContext.RouteData;
            // Hämtar action och controller värden från routeData och omvandlar dessa till stängar
            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
            // Kontrollerar om inparametrarna control och action matchar de värden hämtade via RouteData, svaret lagrat som en boolean variabel
            var returnActive = control == routeControl && action == routeAction;

            // Är returnActive true så returneras "active" som sätts som en klass på länken, annars sätts ingenting som klass
            return returnActive ? "active" : "";
        }
    }
}
