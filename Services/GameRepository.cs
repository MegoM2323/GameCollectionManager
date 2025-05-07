using System.Text.Json;
using Spectre.Console;

namespace Services
{
    /// <summary>
    /// Класс для работы с репозиторием игр, включая выбор жанров, статусов и платформ, а также загрузку и сохранение данных.
    /// </summary>
    public class GameRepository
    {
        /// <summary>
        /// Множество доступных жанров игр.
        /// </summary>
        private static readonly List<string> Genres =
        [
            "Action", "RPG", "Strategy", "Simulator", "Adventure", "Puzzle", "Fighting", "Racing", "Sports", "Другое"
        ];

        /// <summary>
        /// Множество доступных статусов прохождения игры.
        /// </summary>
        private static readonly List<string> Statuses =
        [
            "Не начата", "В процессе", "Пройдена", "Заброшена", "100% Completion", "Другое"
        ];

        /// <summary>
        /// Множество доступных платформ.
        /// </summary>
        private static readonly List<string> Platforms =
        [
            "PC", "PS5", "Xbox Series X", "Nintendo Switch", "Mobile", "Другое"
        ];

        /// <summary>
        /// Функция для выбора статуса игры.
        /// </summary>
        /// <returns>Выбранный статус игры.</returns>
        public static string ChooseStatus()
        {
            string status = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите статус игры:")
                    .AddChoices(Statuses));

            if (status == "Другое")
            {
                status = AnsiConsole.Ask<string>("Введите свой статус:");
                Statuses.Add(status); // Добавляем новый статус в список
            }

            return status;
        }

        /// <summary>
        /// Функция для выбора жанра игры.
        /// </summary>
        /// <returns>Выбранный жанр игры.</returns>
        public static string ChooseGenre()
        {
            string genre = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите жанр игры:")
                    .AddChoices(Genres));

            if (genre == "Другое")
            {
                genre = AnsiConsole.Ask<string>("Введите свой жанр:");
                Genres.Add(genre); // Добавляем новый жанр в список
            }

            return genre;
        }

        /// <summary>
        /// Функция для выбора платформ игры.
        /// </summary>
        /// <returns>Список выбранных платформ.</returns>
        public static List<string> ChoosePlatforms()
        {
            List<string> platforms = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Выберите платформы:")
                    .AddChoices(Platforms));

            // Обработка выбора "Другое"
            if (platforms.Contains("Другое"))
            {
                string otherPlatform = AnsiConsole.Ask<string>("Введите свою платформу:");
                platforms.Add(otherPlatform); // Добавляем новую платформу
                Platforms.Add(otherPlatform); // Добавляем новую платформу в список доступных
            }

            return platforms;
        }

        /// <summary>
        /// Загружает список игр из указанного файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого нужно загрузить игры.</param>
        /// <returns>Список игр, загруженных из файла.</returns>
        public static List<Game> LoadGames(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return []; // Если файл не найден, возвращаем пустой список
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Game>>(json) ?? [];
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"Ошибка: Файл не найден. {ex.Message}\n");
                return [];
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.Error.WriteLine($"Ошибка: Директория не найдена. {ex.Message}\n");
                return [];
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка ввода: {ex.Message}\n");
                return [];
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при загрузке данных: {ex.Message}[/]");
                return [];
            }
        }

        /// <summary>
        /// Сохраняет список игр в указанный файл.
        /// </summary>
        /// <param name="games">Список игр для сохранения.</param>
        /// <param name="filePath">Путь к файлу для сохранения.</param>
        public static void SaveGames(List<Game> games, string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при сохранении данных: {ex.Message}[/]");
            }
        }

        /// <summary>
        /// Проверяет, является ли указанный путь к файлу допустимым.
        /// </summary>
        /// <param name="filePath">Путь к файлу для проверки.</param>
        /// <returns>True, если путь к файлу корректен, иначе false.</returns>
        public static bool IsValidFilePath(string filePath)
        {
            try
            {
                _ = Path.GetFullPath(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
