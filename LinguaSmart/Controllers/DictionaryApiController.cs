
using LinguaSmart.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinguaSmart.Controllers
{
    public class DictionaryApiController : Controller
    {
        private readonly DictionaryApiService _dictionaryApi;

        public DictionaryApiController(
            DictionaryApiService dictionaryApi)
        {
            _dictionaryApi = dictionaryApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string word)
        {
            ViewBag.Word = word;

            ViewBag.Definition =
                await _dictionaryApi.GetDefinitionAsync(word);

            return View();
        }
    }
}
