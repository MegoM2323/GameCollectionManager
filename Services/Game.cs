using System.Text.Json;

namespace Services
{
    /// <summary>
    /// Перечисление для статусов игры в списке.
    /// </summary>
    public enum ListStatus
    {
        /// <summary>
        /// Игра не добавлена в ни один список.
        /// </summary>
        None,

        /// <summary>
        /// Игра в списке "Хочу поиграть".
        /// </summary>
        WishList,

        /// <summary>
        /// Игра в списке "Пройденные игры".
        /// </summary>
        Completed
    }

    /// <summary>
    /// Класс, представляющий игру.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Название игры.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Платформы, на которых доступна игра.
        /// </summary>
        public List<string> Platforms { get; set; }

        /// <summary>
        /// Жанр игры.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// Год выпуска игры.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Статус прохождения игры.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Описание игры.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL обложки игры.
        /// </summary>
        public string CoverUrl { get; set; }

        /// <summary>
        /// Локальный путь к сохраненной обложке.
        /// </summary>
        public string LocalCoverPath { get; set; }

        /// <summary>
        /// Внутренняя ссылка на обложку игры.
        /// </summary>
        public string LocalUrl { get; set; }

        /// <summary>
        /// Разработчик игры.
        /// </summary>
        public string Developer { get; set; }

        /// <summary>
        /// Издатель игры.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Авторы игры.
        /// </summary>
        public List<string> Authors { get; set; }

        /// <summary>
        /// Ссылка на игру.
        /// </summary>
        public string? Url { get; internal set; }

        /// <summary>
        /// Статус игры в контексте списка (например, "Хочу поиграть", "Пройдено").
        /// </summary>
        public ListStatus ListStatus { get; set; } = ListStatus.None; // По умолчанию игра не в списках
        public string? ReleaseYear { get; internal set; }


        /// <summary>
        /// Конструктор для инициализации объекта игры.
        /// </summary>
        public Game()
        {
            Platforms = [];
            Authors = [];
        }

        /// <summary>
        /// Загрузка списка игр из файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу для чтения.</param>
        /// <returns>Список объектов <see cref="Game"/>.</returns>
        public static List<Game> LoadGames(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return [];
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Game>>(json) ?? [];
        }
    }
}
