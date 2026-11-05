
using System.Net;
using System.Text.Json;

namespace LinguaSmart.Services
{
    public class DictionaryApiService
    {
        private readonly HttpClient _httpClient;

        public DictionaryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetDefinitionAsync(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return "Моля, въведи английска дума.";
            }

            try
            {
                string url =
                    "https://api.dictionaryapi.dev/api/v2/entries/en/"
                    + Uri.EscapeDataString(word.Trim());

                using var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return "Няма намерено определение за тази дума.";
                }

                if (!response.IsSuccessStatusCode)
                {
                    return "Проблем с външния речник. HTTP: "
                           + (int)response.StatusCode;
                }

                string json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);

                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var entry in root.EnumerateArray())
                    {
                        if (!entry.TryGetProperty(
                            "meanings", out var meanings))
                            continue;

                        foreach (var meaning in meanings.EnumerateArray())
                        {
                            if (!meaning.TryGetProperty(
                                "definitions", out var definitions))
                                continue;

                            foreach (var definition in definitions.EnumerateArray())
                            {
                                if (definition.TryGetProperty(
                                    "definition", out var text))
                                {
                                    string? result = text.GetString();

                                    if (!string.IsNullOrWhiteSpace(result))
                                    {
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }

                return "API не върна определение за думата.";
            }
            catch (HttpRequestException)
            {
                return "Няма връзка с външния речник.";
            }
            catch (TaskCanceledException)
            {
                return "Външният речник не отговори навреме.";
            }
            catch (JsonException)
            {
                return "Получен е невалиден отговор от API.";
            }
        }
    }
}
