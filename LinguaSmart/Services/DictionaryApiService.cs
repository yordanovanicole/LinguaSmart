
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
                return "Моля, въведи английска дума.";

            word = word.Trim();

            
            string? result = await GetFromDictionaryApi(word);

            if (!string.IsNullOrWhiteSpace(result))
                return result;

            
            result = await GetFromDatamuse(word);

            if (!string.IsNullOrWhiteSpace(result))
                return result;

            return "В момента няма достъпно определение за тази дума.";
        }

        private async Task<string?> GetFromDictionaryApi(string word)
        {
            try
            {
                string url =
                    "https://api.dictionaryapi.dev/api/v2/entries/en/"
                    + Uri.EscapeDataString(word);

                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                string json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);

                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Array)
                    return null;

                var results = new List<string>();

                foreach (var entry in root.EnumerateArray())
                {
                    if (!entry.TryGetProperty("meanings", out var meanings))
                        continue;

                    foreach (var meaning in meanings.EnumerateArray())
                    {
                        if (!meaning.TryGetProperty("definitions", out var definitions))
                            continue;

                        foreach (var definition in definitions.EnumerateArray())
                        {
                            if (definition.TryGetProperty("definition", out var text))
                            {
                                string? value = text.GetString();

                                if (!string.IsNullOrWhiteSpace(value)
                                    && !results.Contains(value))
                                {
                                    results.Add(value);
                                }

                                if (results.Count == 2)
                                    return string.Join(" | ", results);
                            }
                        }
                    }
                }

                return results.Count > 0
                    ? string.Join(" | ", results)
                    : null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private async Task<string?> GetFromDatamuse(string word)
        {
            try
            {
                string url =
                    "https://api.datamuse.com/words?sp="
                    + Uri.EscapeDataString(word)
                    + "&md=d&max=5";

                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                string json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);

                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Array)
                    return null;

                foreach (var entry in root.EnumerateArray())
                {
                    if (!entry.TryGetProperty("word", out var foundWord))
                        continue;

                    if (!string.Equals(
                        foundWord.GetString(),
                        word,
                        StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!entry.TryGetProperty("defs", out var definitions))
                        continue;

                    var results = new List<string>();

                    foreach (var definition in definitions.EnumerateArray())
                    {
                        string? value = definition.GetString();

                        if (string.IsNullOrWhiteSpace(value))
                            continue;

                        int tab = value.IndexOf('\t');

                        if (tab >= 0)
                            value = value.Substring(tab + 1);

                        results.Add(value);

                        if (results.Count == 2)
                            break;
                    }

                    if (results.Count > 0)
                        return string.Join(" | ", results);
                }

                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
