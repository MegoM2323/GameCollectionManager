using System.Text.Json;

namespace Services
{
    /// <summary>
    /// Сервис для взаимодействия с Wikipedia API для получения информации об играх.
    /// </summary>
    public class WikipediaService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Инициализирует новый экземпляр сервиса WikipediaService.
        /// Настроен HttpClient с пользовательским заголовком.
        /// </summary>
        public WikipediaService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GameCollectionManagerApp");
        }

        /// <summary>
        /// Получает подробную информацию об игре по её названию с Wikipedia.
        /// </summary>
        /// <param name="gameTitle">Название игры, для которой требуется получить информацию.</param>
        /// <returns>Объект типа <see cref="Game"/>, содержащий информацию о игре, или null, если игра не найдена.</returns>
        public async Task<Game?> GetGameDetails(string gameTitle)
        {
            string url = $"https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts|info&exintro=true&inprop=url&titles={gameTitle}";

            try
            {
                string response = await _httpClient.GetStringAsync(url);
                JsonDocument jsonDocument = JsonDocument.Parse(response);

                JsonElement pages = jsonDocument.RootElement
                    .GetProperty("query")
                    .GetProperty("pages");

                if (!pages.EnumerateObject().Any())
                {
                    return null;
                }

                JsonElement page = pages.EnumerateObject().First().Value;

                string title = page.GetProperty("title").GetString() ?? "Неизвестно";
                string? extract = page.GetProperty("extract").GetString();
                string? urlLink = page.GetProperty("fullurl").GetString();

                string? developer = null;
                string? publisher = null;

                List<string> authors = [];

                Game game = new()
                {
                    Title = title,
                    Description = extract ?? "No description available",
                    Year = "Неизвестно", // Год выпуска пока не извлекается
                    Genre = "Неизвестно", // Жанр пока не извлекается
                    Developer = developer ?? string.Empty,
                    Publisher = publisher ?? string.Empty,
                    Url = urlLink,
                    Authors = authors
                };

                return game;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
