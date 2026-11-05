
using System.Diagnostics;
using LinguaSmart.Data;
using LinguaSmart.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinguaSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var words = _context.Words
                .OrderBy(w => w.Id)
                .ToList();

            Word? wordOfDay = null;

            if (words.Count > 0)
            {
                int index = (DateTime.Today.DayOfYear - 1)
                            % words.Count;

                wordOfDay = words[index];
            }

            return View(wordOfDay);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location =
            ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
