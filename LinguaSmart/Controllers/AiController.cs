
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace LinguaSmart.Controllers
{
    public class AiController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration configuration;

        public AiController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string word)
        {
            ViewBag.Word = word;

            if (string.IsNullOrWhiteSpace(word))
            {
                ViewBag.Error = "Моля, въведи английска дума.";
                return View();
            }

            string? apiKey = configuration["GeminiApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ViewBag.Error = "Липсва Gemini API ключ.";
                return View();
            }

            string prompt =
                "Ти си езиков помощник в приложението LinguaSmart. " +
                "Потребителят учи английски език. " +
                $"Анализирай английската дума: {word}. " +
                "Отговори на български точно в следния формат:\n" +
                "Превод: ...\n" +
                "Обяснение: ...\n" +
                "Синоними: ... (два английски синонима)\n" +
                "Пример: ... (английско изречение)\n" +
                "Превод на примера: ...\n" +
                "Използвай само обикновен текст. " +
                "Не използвай Markdown, звездички или таблици. " +
                "Бъди кратък, но попълни всичките пет реда.";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = 1500
                }
            };

            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Post,

            "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent"

                );

                request.Headers.Add("x-goog-api-key", apiKey);
                request.Content = JsonContent.Create(body);

                using var response = await client.SendAsync(request);

                string jsonText =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        $"Грешка от Gemini API: {(int)response.StatusCode}";
                    return View();
                }

                using var document = JsonDocument.Parse(jsonText);

                var candidates = document.RootElement
                    .GetProperty("candidates");

                if (candidates.GetArrayLength() == 0)
                {
                    ViewBag.Error = "ИИ не върна отговор.";
                    return View();
                }

                var candidate = candidates[0];

                string answer = "";

                if (candidate.TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts))
                {
                    foreach (var part in parts.EnumerateArray())
                    {
                        if (part.TryGetProperty("text", out var text))
                        {
                            answer += text.GetString() + "\n";
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(answer))
                {
                    ViewBag.Error = "ИИ не върна текстов отговор.";
                }
                else
                {
                    ViewBag.Answer = answer.Trim();

                    if (candidate.TryGetProperty(
                        "finishReason", out var finishReason) &&
                        finishReason.GetString() == "MAX_TOKENS")
                    {
                        ViewBag.Warning =
                            "Отговорът е прекъснат поради ограничение на дължината.";
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error =
                    "Възникна проблем при връзката с ИИ.";
            }

            return View();
        }
    }
}
