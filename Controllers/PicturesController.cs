/* 
 * Controller för visning av bilder och uppladdning av bilder
 * Källa för kod https://www.aspsnippets.com/Articles/3114/ASPNet-MVC-Display-List-of-Files-from-Folder-Directory/
 * samt https://www.thetechplatform.com/post/upload-file-to-a-folder-in-asp-net-core-mvc
 */

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Picasso.Models;
using System.IO;


namespace Picasso.Controllers
{
    public class PicturesController : Controller
    {
        public IActionResult Index()
        {
            // Hämtar alla bilder i uploads mappen
            string[] filePaths = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads"));

            // Kopierar alla filnamnen till en lista
            List<FileModel> files = new List<FileModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new FileModel { FileName = Path.GetFileName(filePath) });
            }

            return View(files);
        }

        // Tillåter nedladdning av filer
        public FileResult DownloadFile(string fileName)
        {
            // Bygger sökvägen till filen
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads", fileName);

            // Läser in filen
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            // Skickar filen för nedladdning.
            return File(bytes, "application/octet-stream", fileName);
        }

        public IActionResult Upload()
        {
            return View();
        }

        // Laddar upp fil till Upload mappen
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Är ingen fil uppladdad så visas ett felmeddelande
            if (file == null || file.Length == 0)
            {
                return Content("ingen fil har valts");
            }

            // Variabel för sökvägen där filen kommer att hamna, i det här fallet en mapp som heter "uploads" i root mappen
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/uploads",
                        file.FileName);

            // Kopierar filen till sökvägen med hjälp av filestream
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Index");
        }
    }
}
