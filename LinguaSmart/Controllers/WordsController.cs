
using LinguaSmart.Data;
using LinguaSmart.DTOs;
using LinguaSmart.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinguaSmart.Controllers
{
    public class WordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var words = _context.Words.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                words = words.Where(w =>
                    w.EnglishWord.Contains(search) ||
                    w.BulgarianTranslation.Contains(search));
            }

            var result = words.Select(w => new WordDto
            {
                Id = w.Id,
                EnglishWord = w.EnglishWord,
                BulgarianTranslation = w.BulgarianTranslation
            }).ToList();

            ViewBag.Search = search;

            return View(result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Word word)
        {
            word.DifficultyLevel = GetDifficultyLevel(word.EnglishWord);

            if (ModelState.IsValid)
            {
                _context.Words.Add(word);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(word);
        }

        public IActionResult Details(int id)
        {
            var word = _context.Words.Find(id);

            if (word == null)
            {
                return NotFound();
            }

            return View(word);
        }

        public IActionResult Edit(int id)
        {
            var word = _context.Words.Find(id);

            if (word == null)
            {
                return NotFound();
            }

            return View(word);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Word word)
        {
            if (id != word.Id)
            {
                return NotFound();
            }

            word.DifficultyLevel = GetDifficultyLevel(word.EnglishWord);

            if (ModelState.IsValid)
            {
                var existingWord = _context.Words.Find(id);

                if (existingWord == null)
                {
                    return NotFound();
                }

                existingWord.EnglishWord = word.EnglishWord;
                existingWord.BulgarianTranslation = word.BulgarianTranslation;
                existingWord.Definition = word.Definition;
                existingWord.ExampleSentence = word.ExampleSentence;
                existingWord.Synonyms = word.Synonyms;
                existingWord.Category = word.Category;
                existingWord.DifficultyLevel = word.DifficultyLevel;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(word);
        }

        public IActionResult Delete(int id)
        {
            var word = _context.Words.Find(id);

            if (word == null)
            {
                return NotFound();
            }

            return View(word);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var word = _context.Words.Find(id);

            if (word != null)
            {
                _context.Words.Remove(word);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Flashcards()
        {
            var words = _context.Words
                .OrderBy(w => w.DifficultyLevel)
                .ToList();

            return View(words);
        }

        private string GetDifficultyLevel(string englishWord)
        {
            string word = englishWord.Trim().ToLower();

            string[] beginnerWords =
            {
                "beautiful", "comfortable",
                "journey", "knowledge"
            };

            string[] intermediateWords =
            {
                "improve", "grateful",
                "destination", "deadline"
            };

            string[] advancedWords =
            {
                "resilience", "negotiate", "analyze"
            };

            if (beginnerWords.Contains(word))
                return "Beginner";

            if (intermediateWords.Contains(word))
                return "Intermediate";

            if (advancedWords.Contains(word))
                return "Advanced";

            if (word.Length <= 5)
                return "Beginner";

            if (word.Length <= 8)
                return "Intermediate";

            return "Advanced";
        }
    }
}
