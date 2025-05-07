using Spectre.Console;
using System.Security;

namespace Services
{
    /// <summary>
    /// Класс для управления играм, загрузки, редактирования и визуализации коллекции игр.
    /// </summary>
    public class GameManager
    {
        private List<Game> _games;
        private string _filePath;
        private readonly GameRepository _repo;
        private readonly Visualizer _visualizer;
        private readonly WikipediaService _wikiService;
        private readonly List<Game> _wishList; 
        private readonly List<Game> _completedGames; 

        /// <summary>
        /// Конструктор для инициализации менеджера игр.
        /// </summary>
        public GameManager()
        {
            _games = [];
            _repo = new GameRepository();
            _visualizer = new Visualizer();
            _wikiService = new WikipediaService();
        }

        /// <summary>
        /// Основная функция для выполнения операций с играми.
        /// </summary>
        public void Run()
        {
            GetFilePath();
            try
            {
                LoadGames();

                while (true)
                {
                    string choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Выберите действие:")
                            .AddChoices("Просмотр игр", "Добавить игру", "Редактировать игру", "Удалить игру",
                                        "Визуализация", "Поиск в Wikipedia", "Управление списками", "Резервное копирование",
                                        "Добавить обложку к игре", "Просмотр обложек игр", "Выход"));

                    switch (choice)
                    {
                        case "Просмотр игр":
                            ViewGames();
                            break;
                        case "Добавить игру":
                            AddGame();
                            break;
                        case "Редактировать игру":
                            EditGame();
                            break;
                        case "Удалить игру":
                            DeleteGame();
                            break;
                        case "Визуализация":
                            Visualize();
                            break;
                        case "Поиск в Wikipedia":
                            SearchWikipedia().Wait();
                            break;
                        case "Управление списками":
                            ManageLists();
                            break;
                        case "Резервное копирование":
                            BackupRestore();
                            break;
                        case "Просмотр обложек игр":
                            _ = ViewGameCoversAsync();
                            break;
                        case "Добавить обложку к игре":
                            _ = AddCoverToGameAsync();
                            break;
                        case "Выход":
                            SaveGames();
                            return;
                        default:
                            break;
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Ошибка: Параметр не может быть null. {ex.Message}\n");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка: Значение выходит за допустимые границы. {ex.Message}\n");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: Некорректный путь или аргумент. {ex.Message}\n");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка: Операция не может быть выполнена в текущем состоянии. {ex.Message}\n");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Ошибка формата: {ex.Message}\n");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Ошибка: Деление на ноль. {ex.Message}\n");
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"Ошибка: Файл не найден. {ex.Message}\n");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.Error.WriteLine($"Ошибка: Директория не найдена. {ex.Message}\n");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка ввода/вывода: {ex.Message}\n");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine($"Ошибка доступа: {ex.Message}\n");
            }
            catch (SecurityException ex)
            {
                Console.WriteLine($"Ошибка безопасности: {ex.Message}\n");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine($"Ошибка: Недостаточно памяти. {ex.Message}\n");
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка: Индекс вне диапазона. {ex.Message}\n");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Ошибка: Ссылка на null. {ex.Message}\n");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Ошибка: Превышено время ожидания. {ex.Message}\n");
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"Ошибка: Некорректное преобразование типа. {ex.Message}\n");
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"Ошибка: Переполнение числового значения. {ex.Message}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Добавляет обложку к выбранной игре.
        /// </summary>
        /// <summary>
        /// Добавляет обложку к выбранной игре.
        /// </summary>
        public async Task AddCoverToGameAsync()
        {
            if (_games.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Нет доступных игр для добавления обложки.[/]");
                return;
            }

            // Выбор игры из списка
            Game game = AnsiConsole.Prompt(
                new SelectionPrompt<Game>()
                    .Title("Выберите игру для добавления обложки")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Используйте стрелки для навигации)[/]")
                    .AddChoices(_games)
                    .UseConverter(g => g.Title)
            );

            AnsiConsole.MarkupLine($"[bold]Выбрана игра:[/] [cyan]{game.Title}[/]");

            string coverUrl = AnsiConsole.Ask<string>("Введите URL обложки:");

            if (string.IsNullOrWhiteSpace(coverUrl) || !Uri.IsWellFormedUriString(coverUrl, UriKind.Absolute))
            {
                AnsiConsole.MarkupLine("[red]Ошибка: Некорректный URL.[/]");
                return;
            }

            string coversDirectory = "covers";
            if (!Directory.Exists(coversDirectory))
            {
                _ = Directory.CreateDirectory(coversDirectory);
            }

            string fileName = $"{game.Title.Replace(" ", "_")}_cover.jpg";
            game.LocalCoverPath = Path.Combine(coversDirectory, fileName);
            game.CoverUrl = coverUrl;
            game.LocalUrl = $"file://{Path.GetFullPath(game.LocalCoverPath)}";

            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                HttpResponseMessage response = await client.GetAsync(coverUrl);

                if (!response.IsSuccessStatusCode)
                {
                    AnsiConsole.MarkupLine($"[red]Ошибка: Сервер вернул статус {response.StatusCode}[/]");
                    return;
                }

                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                if (imageBytes.Length < 100)
                {
                    AnsiConsole.MarkupLine("[red]Ошибка: Файл слишком маленький, возможно, скачивание не удалось.[/]");
                    return;
                }

                await File.WriteAllBytesAsync(game.LocalCoverPath, imageBytes);

                AnsiConsole.MarkupLine($"[green]Обложка успешно загружена для {game.Title}[/]");
                SaveGames();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при скачивании обложки: {ex.Message}[/]");
            }
        }

        /// <summary>
        /// Просмотр игр с обложкой в коллекции.
        /// </summary>
        /// <returns>Игры с обложками.</returns>
        public async Task ViewGameCoversAsync()
        {
            if (_games == null || _games.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Нет доступных игр с обложками.[/]");
                return;
            }

            foreach (Game? game in _games.Where(g => !string.IsNullOrEmpty(g.LocalCoverPath) && File.Exists(g.LocalCoverPath)))
            {
                await DisplayGameCoverAsync(game);
            }
        }

        /// <summary>
        /// Отображает обложку и информацию об игре.
        /// </summary>
        /// <param name="game">Игра для отображения.</param>
        private async Task DisplayGameCoverAsync(Game game)
        {
            if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Ошибка: передана недопустимая игра.[/]");
                return;
            }

            Table table = new()
            {
                Border = TableBorder.Rounded,
                Expand = true
            };

            _ = table.AddColumn("Название");
            _ = table.AddColumn("Жанр");
            _ = table.AddColumn("Год выпуска");
            _ = table.AddColumn("Статус");

            _ = table.AddRow(
                game.Title ?? "Неизвестно",
                game.Genre ?? "Неизвестно",
                !string.IsNullOrEmpty(game.ReleaseYear) ? game.ReleaseYear : "Неизвестно",
                game.Status ?? "Неизвестно"
            );

            AnsiConsole.Write(table);

            AnsiConsole.Write(new CanvasImage(game.LocalCoverPath));
        }


        /// <summary>
        /// Осуществляет поиск информации о игре в Wikipedia.
        /// </summary>
        private async Task SearchWikipedia()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры для поиска в Wikipedia:");
            Game? game = await _wikiService.GetGameDetails(title);

            if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Информация не найдена или произошла ошибка.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Описание: {game.Description}[/]");
                AnsiConsole.MarkupLine($"[green]Жанр: {game.Genre}[/]");
                AnsiConsole.MarkupLine($"[green]Год выпуска: {game.Year}[/]");
                AnsiConsole.MarkupLine($"[green]Разработчик: {game.Developer}[/]");
                AnsiConsole.MarkupLine($"[green]Издатель: {game.Publisher}[/]");

                if (game.Authors != null && game.Authors.Count != 0)
                {
                    AnsiConsole.MarkupLine($"[green]Авторы: {string.Join(", ", game.Authors)}[/]");
                }

                bool save = AnsiConsole.Confirm("Хотите сохранить эту игру в коллекцию?");
                if (save)
                {
                    _games.Add(game);
                    SaveGames();
                    AnsiConsole.MarkupLine("[green]Игра сохранена.[/]");
                }
            }
        }

        /// <summary>
        /// Добавляет новую игру в коллекцию.
        /// </summary>
        private async void AddGame()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры:");
            List<string> platforms = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Выберите платформу(ы):")
                    .AddChoices("PC", "PlayStation", "Xbox", "Nintendo", "Mobile", "Другие")
            );

            List<string> genres = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Выберите жанр(ы):")
                    .AddChoices("Action", "Adventure", "RPG", "Strategy", "Shooter", "Sports", "Simulation", "Other")
            );

            string status = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите прогресс игры:")
                    .AddChoices("Не начато", "Процесс", "Пройдено", "Отложено", "Не завершено")
            );


            Game game = new()
            {
                Title = title,
                Platforms = platforms,
                Genre = string.Join(", ", genres),
                Year = GetYear(),
                Status = status
            };

            /// Функция получения корректного года.
            static string GetYear()
            {
                int currentYear = DateTime.Now.Year;

                while (true)
                {
                    string input = AnsiConsole.Ask<string>("Введите год выпуска:");

                    if (int.TryParse(input, out int year) && year >= 0 && year <= currentYear)
                    {
                        return year.ToString();
                    }
                    else
                    {
                        AnsiConsole.Markup("[red]Пожалуйста, введите корректное число в пределах от 0 до текущего года.[/]");
                    }
                }
            }


            _games.Add(game);
            SaveGames();
            AnsiConsole.MarkupLine("[green]Игра успешно добавлена в коллекцию.[/]");
        }

        /// <summary>
        /// Запрашивает путь к файлу коллекции.
        /// </summary>
        private void GetFilePath()
        {
            _filePath = AnsiConsole.Ask<string>("Введите путь к файлу коллекции:");
            while (!GameRepository.IsValidFilePath(_filePath))
            {
                AnsiConsole.MarkupLine("[red]Неверный путь к файлу.[/]");
                _filePath = AnsiConsole.Ask<string>("Введите корректный путь к файлу коллекции:");
            }
        }

        // <summary>
        /// Загружает игры из файла.
        /// </summary>
        private void LoadGames()
        {
            _games = GameRepository.LoadGames(_filePath);
        }

        /// <summary>
        /// Сохраняет коллекцию игр в файл.
        /// </summary>
        private void SaveGames()
        {
            GameRepository.SaveGames(_games, _filePath);
        }

        /// <summary>
        /// Показывает игры в таблицу с применением сортировок и фильтров..
        /// </summary>
        private void ViewGames()
        {
            bool filterChoice = AnsiConsole.Confirm("Хотите применить фильтрацию?");

            List<Game> filteredGames = _games;

            if (filterChoice)
            {
                string filterOption = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите параметр для фильтрации:")
                        .AddChoices("Жанр", "Год выпуска", "Платформы", "Статус", "Поиск по названию")
                );

                string filterValue = AnsiConsole.Ask<string>($"Введите значение для фильтрации по {filterOption}:").Trim();

                switch (filterOption)
                {
                    case "Жанр":
                        filteredGames = [.. filteredGames.Where(g => g.Genre?.Contains(filterValue, StringComparison.OrdinalIgnoreCase) ?? false)];
                        break;
                    case "Год выпуска":
                        filteredGames = [.. filteredGames.Where(g => g.Year?.Contains(filterValue, StringComparison.OrdinalIgnoreCase) ?? false)];
                        break;
                    case "Платформы":
                        filteredGames = [.. filteredGames.Where(g => g.Platforms.Any(p => p.Contains(filterValue, StringComparison.OrdinalIgnoreCase)))];
                        break;
                    case "Статус":
                        filteredGames = [.. filteredGames.Where(g => g.Status?.Contains(filterValue, StringComparison.OrdinalIgnoreCase) ?? false)];
                        break;
                    case "Поиск по названию":
                        filteredGames = [.. filteredGames.Where(g => g.Title?.Contains(filterValue, StringComparison.OrdinalIgnoreCase) ?? false)];
                        break;
                    default:
                        break;
                }
            }

            string sortOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите сортировку:")
                    .AddChoices("По названию", "По году выпуска", "По статусу")
            );

            string sortOrder = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите порядок сортировки:")
                    .AddChoices("По возрастанию", "По убыванию")
            );

            switch (sortOption)
            {
                case "По названию":
                    filteredGames = sortOrder == "По возрастанию"

                        ? [.. filteredGames.OrderBy(g => g.Title)]

                        : [.. filteredGames.OrderByDescending(g => g.Title)];
                    break;

                case "По году выпуска":
                    filteredGames = sortOrder == "По возрастанию"
                        ? [.. filteredGames.OrderBy(g => int.TryParse(g.Year, out int year) ? year : int.MaxValue)]
                        : [.. filteredGames.OrderByDescending(g => int.TryParse(g.Year, out int year) ? year : int.MinValue)];
                    break;

                case "По статусу":
                    filteredGames = sortOrder == "По возрастанию"
                        ? [.. filteredGames.OrderBy(g => g.Status)]
                        : [.. filteredGames.OrderByDescending(g => g.Status)];
                    break;

                default:
                    break;
            }

            Table table = new()

            {
                Border = TableBorder.Rounded,
                Expand = true
            };

            _ = table.AddColumn("Название");
            _ = table.AddColumn("Платформы");
            _ = table.AddColumn("Жанр");
            _ = table.AddColumn("Год");
            _ = table.AddColumn("Статус");

            foreach (Game game in filteredGames)
            {
                _ = table.AddRow(
                    game.Title ?? "Неизвестно",
                    string.Join(", ", game.Platforms) ?? "Не указано",
                    game.Genre ?? "Неизвестно",
                    game.Year ?? "Неизвестно",
                    game.Status ?? "Не указано"
                );
            }

            AnsiConsole.Write(table);
        }

        /// <summary>
        /// Редактирует информацию о существующей игре в коллекции.
        /// </summary>
        private void EditGame()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры для редактирования:");
            Game? game = _games.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Игра не найдена![/]");
                return;
            }

            game.Title = AnsiConsole.Ask("Новое название:", game.Title);
            string platformsInput = AnsiConsole.Ask("Новые платформы (через запятую):", string.Join(", ", game.Platforms));
            game.Platforms = string.IsNullOrWhiteSpace(platformsInput) ? game.Platforms : [.. platformsInput.Split(',').Select(p => p.Trim())];
            game.Genre = AnsiConsole.Ask("Новый жанр:", game.Genre);
            game.Year = AnsiConsole.Ask("Новый год:", game.Year);
            game.Status = AnsiConsole.Ask("Новый статус:", game.Status);

            SaveGames();
            AnsiConsole.MarkupLine("[green]Игра успешно отредактирована.[/]");
        }

        /// <summary>
        /// Удаляет игру из коллекции.
        /// </summary>
        private void DeleteGame()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры для удаления:");
            Game? game = _games.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Игра не найдена![/]");
                return;
            }

            _ = _games.Remove(game);
            SaveGames();
            AnsiConsole.MarkupLine("[green]Игра успешно удалена.[/]");
        }

        /// <summary>
        /// Отображает различные типы визуализаций для коллекции игр.
        /// </summary>
        private void Visualize()
        {
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите тип визуализации:")
                    .AddChoices("Дерево игр", "Календарь выпуска", "Диаграмма платформ"));

            switch (choice)
            {
                case "Дерево игр":
                    Visualizer.ShowGameTree(_games);
                    break;
                case "Календарь выпуска":
                    Visualizer.ShowCalendar(_games);
                    break;
                case "Диаграмма платформ":
                    Visualizer.ShowPlatformChart(_games);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Управление списками игр (желаемые и пройденные игры).
        /// </summary>
        private void ManageLists()
        {
            while (true)
            {
                string action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите действие:")
                        .AddChoices("Просмотр списков", "Добавить игру в список желаемых", "Добавить игру в список пройденных",
                                    "Переместить игру из одного списка в другой", "Назад"));

                switch (action)
                {
                    case "Просмотр списков":
                        ViewLists();
                        break;
                    case "Добавить игру в список желаемых":
                        AddToWishList();
                        break;
                    case "Добавить игру в список пройденных":
                        AddToCompletedGames();
                        break;
                    case "Переместить игру из одного списка в другой":
                        MoveGameBetweenLists();
                        break;
                    case "Назад":
                        return;
                }
            }
        }

        /// <summary>
        /// Просматривает игры в списках желаемых и пройденных игр.
        /// </summary>
        private void ViewLists()
        {
            List<Game> wishListGames = [.. _games.Where(g => g.ListStatus == ListStatus.WishList)];
            List<Game> completedGames = [.. _games.Where(g => g.ListStatus == ListStatus.Completed)];

            Table wishListTable = CreateGameTable(wishListGames);
            Table completedListTable = CreateGameTable(completedGames);

            AnsiConsole.MarkupLine($"[bold cyan]Список желаемых игр[/]");
            AnsiConsole.Write(wishListTable);

            AnsiConsole.MarkupLine($"[bold cyan]Список пройденных игр[/]");
            AnsiConsole.Write(completedListTable);
        }

        /// <summary>
        /// Создает таблицу для отображения списка игр.
        /// </summary>
        /// <param name="games">Список игр для отображения.</param>
        /// <returns>Таблица с играми.</returns>
        private Table CreateGameTable(List<Game> games)
        {
            Table table = new()
            {
                Border = TableBorder.Rounded,
                Expand = true
            };

            _ = table.AddColumn(new TableColumn("Название игры").Centered());
            _ = table.AddColumn(new TableColumn("Платформы").Centered());
            _ = table.AddColumn(new TableColumn("Жанр").Centered());
            _ = table.AddColumn(new TableColumn("Год выпуска").Centered());
            _ = table.AddColumn(new TableColumn("Статус").Centered());

            foreach (Game game in games)
            {
                string statusColor = game.ListStatus == ListStatus.WishList ? "[yellow]" : "[green]";
                _ = table.AddRow(
                    game.Title ?? "Неизвестно",
                    string.Join(", ", game.Platforms) ?? "Не указано",
                    game.Genre ?? "Неизвестно",
                    game.Year ?? "Неизвестно",
                    $"{statusColor}{game.Status}[/]"
                );
            }

            return table;
        }

        /// <summary>
        /// Добавляет игру в список желаемых игр.
        /// </summary>
        private void AddToWishList()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры для добавления в список желаемых:");
            Game? game = _games.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (game != null && game.ListStatus == ListStatus.None)
            {
                game.ListStatus = ListStatus.WishList;
                AnsiConsole.MarkupLine("[green]Игра добавлена в список желаемых.[/]");
            }
            else if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Игра не найдена![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Игра уже находится в другом списке![/]");
            }

            SaveGames();
        }

        /// <summary>
        /// Добавляет игру в список пройденных игр.
        /// </summary>
        private void AddToCompletedGames()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры для добавления в список пройденных:");
            Game? game = _games.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (game != null && game.ListStatus == ListStatus.None)
            {
                game.ListStatus = ListStatus.Completed;
                AnsiConsole.MarkupLine("[green]Игра добавлена в список пройденных.[/]");
            }
            else if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Игра не найдена![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Игра уже находится в другом списке![/]");
            }

            SaveGames();
        }

        /// <summary>
        /// Перемещает игру между списками (из желаемых в пройденные и наоборот).
        /// </summary>
        private void MoveGameBetweenLists()
        {
            string title = AnsiConsole.Ask<string>("Введите название игры для перемещения:");

            Game? game = _games.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (game == null)
            {
                AnsiConsole.MarkupLine("[red]Игра не найдена![/]");
                return;
            }

            if (game.ListStatus == ListStatus.WishList)
            {
                game.ListStatus = ListStatus.Completed;
                AnsiConsole.MarkupLine("[green]Игра перемещена в список пройденных игр.[/]");
            }
            else if (game.ListStatus == ListStatus.Completed)
            {
                game.ListStatus = ListStatus.WishList;
                AnsiConsole.MarkupLine("[green]Игра перемещена в список желаемых игр.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Игра не находится в одном из списков![/]");
            }

            SaveGames();
        }

        /// <summary>
        /// Создает резервную копию и восстанавливает данные из резервной копии.
        /// </summary>
        /// <summary>
        /// Создает резервную копию и восстанавливает данные из резервной копии.
        /// </summary>
        private void BackupRestore()
        {
            string action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Что вы хотите сделать?")
                    .AddChoices("Создать резервную копию", "Восстановить из резервной копии", "Отмена"));

            string filePath = _filePath;
            string backupPath;
            string restorePath;

            switch (action)
            {
                case "Создать резервную копию":
                    backupPath = AnsiConsole.Ask<string>("Введите путь для резервного копирования:");
                    try
                    {
                        string? directory = Path.GetDirectoryName(backupPath);
                        if (!Directory.Exists(directory))
                        {
                            _ = Directory.CreateDirectory(directory);
                        }

                        File.Copy(filePath, backupPath, true);
                        AnsiConsole.MarkupLine("[green]Резервная копия успешно создана.[/]");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Ошибка при создании резервной копии: {ex.Message}[/]");
                    }
                    break;

                case "Восстановить из резервной копии":
                    backupPath = AnsiConsole.Ask<string>("Введите путь к резервной копии для восстановления:");
                    restorePath = AnsiConsole.Ask<string>("Введите путь для восстановления:");
                    try
                    {
                        if (!File.Exists(backupPath))
                        {
                            AnsiConsole.MarkupLine("[red]Ошибка: файл резервной копии не существует.[/]");
                            return;
                        }

                        File.Copy(backupPath, restorePath, true);
                        AnsiConsole.MarkupLine("[green]Данные успешно восстановлены.[/]");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Ошибка при восстановлении данных: {ex.Message}[/]");
                    }
                    break;

                case "Отмена":
                    AnsiConsole.MarkupLine("[yellow]Операция отменена.[/]");
                    break;
            }
        }
    }
}