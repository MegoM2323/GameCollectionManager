# Инструкция по установке и запуску GameCollectionManager

## Системные требования

- Windows 10/11 или Linux
- .NET 6.0 SDK или выше
- Visual Studio 2022 (рекомендуется) или Visual Studio Code
- Минимум 2 ГБ свободной оперативной памяти
- 500 МБ свободного места на диске

## Установка

### 1. Клонирование репозитория

```bash
git clone https://github.com/MegoM2323/GameCollectionManager.git
cd GameCollectionManager
```

### 2. Установка зависимостей

#### Windows

```bash
dotnet restore
```

#### Linux

```bash
dotnet restore
```

### 3. Сборка проекта

```bash
dotnet build
```

### 4. Запуск приложения

```bash
dotnet run --project GameCollectionManager
```

## Настройка окружения разработки

### Visual Studio 2022

1. Откройте Visual Studio 2022
2. Выберите "Open a project or solution"
3. Найдите и откройте файл `GameCollectionManager.sln`
4. Дождитесь загрузки всех зависимостей
5. Нажмите F5 для запуска в режиме отладки или Ctrl+F5 для запуска без отладки

### Visual Studio Code

1. Установите расширения:
   - C# Dev Kit
   - .NET Core Tools
2. Откройте папку проекта
3. Нажмите F5 для запуска в режиме отладки

## Структура проекта

```
GameCollectionManager/
├── GameCollectionManager/        # Основной проект
│   ├── Models/                  # Модели данных
│   ├── ViewModels/             # ViewModels для MVVM
│   ├── Views/                  # Представления
│   └── Services/               # Сервисы и бизнес-логика
├── Tests/                      # Модульные тесты
└── docs/                       # Документация
```

## Возможные проблемы и их решение

### 1. Ошибка при восстановлении пакетов

```bash
dotnet nuget locals all --clear
dotnet restore
```

### 2. Ошибка при сборке

Убедитесь, что у вас установлена последняя версия .NET SDK:

```bash
dotnet --version
```

### 3. Проблемы с базой данных

Если возникают проблемы с базой данных:

1. Удалите файл базы данных (если он существует)
2. Перезапустите приложение

## Дополнительная информация

- [Документация по .NET 6.0](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-6)
- [Руководство по C#](https://docs.microsoft.com/dotnet/csharp/)
- [Документация по WPF](https://docs.microsoft.com/dotnet/desktop/wpf/)
