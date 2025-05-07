using Spectre.Console;

namespace Services
{
    /// <summary>
    /// Класс для отображения различных визуальных представлений информации о коллекции игр.
    /// </summary>
    public class Visualizer
    {
        /// <summary>
        /// Отображает календарь с интенсивностью цвета для каждого года, основываясь на количестве выпущенных игр.
        /// </summary>
        /// <param name="games">Список всех игр, для которых нужно показать календарь.</param>
        public static void ShowCalendar(List<Game> games)
        {
            List<string> releaseYears = [.. games.Select(g => g.Year).Distinct().OrderBy(year => year)];

            int maxCount = games.GroupBy(g => g.Year)
                                .Max(g => g.Count());

            Table calendarTable = new Table()
                .AddColumn("Год")
                .AddColumn("Количество игр");

            foreach (string year in releaseYears)
            {
                int count = games.Count(g => g.Year == year);

                double intensity = count / (double)maxCount * 255;

                intensity = Math.Clamp(intensity, 0, 255);

                string colorMarkup = $"[rgb({255 - (int)intensity},{255 - (int)intensity},{(int)intensity})]";

                _ = calendarTable.AddRow($"{colorMarkup}{year}[/]", $"{colorMarkup}{count}[/]");
            }

            AnsiConsole.Write(calendarTable);
        }

        /// <summary>
        /// Отображает иерархию жанров и платформ в виде дерева.
        /// </summary>
        /// <param name="games">Список всех игр для отображения в дереве.</param>
        public static void ShowGameTree(List<Game> games)
        {
            Tree root = new("Игры");

            IEnumerable<string> genres = games.Select(g => g.Genre).Distinct();
            foreach (string genre in genres)
            {
                TreeNode genreNode = root.AddNode(genre);
                IEnumerable<Game> genreGames = games.Where(g => g.Genre == genre);

                IEnumerable<string> platforms = genreGames.SelectMany(g => g.Platforms).Distinct();
                foreach (string platform in platforms)
                {
                    TreeNode platformNode = genreNode.AddNode(platform);
                    IEnumerable<Game> platformGames = genreGames.Where(g => g.Platforms.Contains(platform));

                    foreach (Game game in platformGames)
                    {
                        _ = platformNode.AddNode(game.Title);
                    }
                }
            }

            AnsiConsole.Write(root);
        }

        /// <summary>
        /// Отображает диаграмму распределения игр по платформам.
        /// </summary>
        /// <param name="games">Список всех игр для отображения распределения по платформам.</param>
        public static void ShowPlatformChart(List<Game> games)
        {
            var platformCounts = games.SelectMany(g => g.Platforms)
                                    .GroupBy(p => p)
                                    .Select(g => new { Platform = g.Key, Count = g.Count() })
                                    .OrderByDescending(x => x.Count)
                                    .Take(10);

            BarChart chart = new BarChart()
                .Width(60)
                .Label("[green bold underline]Распределение игр по платформам[/]")
                .CenterLabel();

            foreach (var platform in platformCounts)
            {
                _ = chart.AddItem(platform.Platform, platform.Count, Color.Green);
            }

            AnsiConsole.Write(chart);
        }

        /// <summary>
        /// Запрашивает у пользователя информацию о новой игре и отображает её.
        /// </summary>
        public static void ShowGameDetails()
        {
            Game game = new()
            {
                Title = AnsiConsole.Ask<string>("Введите название игры:"),
                Status = GameRepository.ChooseStatus(),
                Genre = GameRepository.ChooseGenre(),
                Platforms = GameRepository.ChoosePlatforms()
            };

            AnsiConsole.MarkupLine($"Название игры: {game.Title}");
            AnsiConsole.MarkupLine($"Статус игры: {game.Status}");
            AnsiConsole.MarkupLine($"Жанр игры: {game.Genre}");
            AnsiConsole.MarkupLine($"Платформы: {string.Join(", ", game.Platforms)}");
        }
    }
}
