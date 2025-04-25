# CSharpExtensions

Библиотека для .NET 9.0, содержащая: 
- базовые компоненты и утилиты для разработки
- функционал для реализации подхода Railway Programming в .NET приложениях
- функционал для работы с ошибками и исключениями

Эти подходы позволяют создавать более надежный и предсказуемый код, обрабатывая ошибки в функциональном стиле.

## Основные концепции

- Railway Programming - Это подход к обработке ошибок, где успешные операции и ошибки обрабатываются как равноправные пути выполнения программы. Это позволяет создавать более чистый и поддерживаемый код.
- Возврат ошибок - При обработке запросов наши сервисы могут возвращать ошибки, которые должны быть обработаны клиентом. Для этого мы используем стандартный формат ошибок ProblemDetails, который описан в спецификации RFC 9457. Внутри него так же
используются ряд обязательных Extensions, описанных ниже.

## Содержание

- [Требования](#требования)
- [Установка](#установка)
- [Структура проекта](#структура-проекта)
- [Основные компоненты](#основные-компоненты)
- [Работа с ошибками](#работа-с-ошибками)
- [Использование](#использование)

## Требования

- .NET 8.0 
- Microsoft.AspNetCore.App

## Установка

```bash
dotnet add package AcheronSoft.CSharpExtensions
```

## Структура проекта

Проект организован в следующие основные директории:

- `Constants/` - Константы и статические значения
- `CustomObjectResults/` - Пользовательские результаты выполнения
- `Enums/` - Перечисления
- `Exceptions/` - Пользовательские исключения
- `Extensions/` - Методы расширения
- `Infrastructure/` - Инфраструктурные компоненты
- `JsonConverters/` - Конвертеры для JSON
- `JsonNamingPolicies/` - Политики именования для JSON
- `Railway/` - Классы и компоненты для реализации подхода Railway

### Основные компоненты

1. `Result` - базовый класс для представления результата операции
2. `Result<T>` - обобщенный класс для представления результата операции с возвращаемым значением
3. `Error` - класс для представления ошибок

### Constants
Определяет константы и статические значения, используемые во всем проекте.

### CustomObjectResults
Содержит пользовательские классы для возврата результатов выполнения операций.

### Enums
Определяет перечисления, используемые в проекте.

### Exceptions
Содержит пользовательские исключения для обработки специфических ошибок.

### Extensions
Предоставляет методы расширения для различных типов данных.

### Infrastructure
Содержит инфраструктурные компоненты, такие как:
- Логирование
- Кэширование
- Конфигурация
- Валидация

### JsonConverters
Содержит конвертеры для сериализации/десериализации JSON.

### JsonNamingPolicies
Определяет политики именования для JSON-сериализации.

### Railway

## Базовое использование

### Создание успешного результата

```csharp
// Создание успешного результата без значения
Result success = Result.Success();

// Создание успешного результата со значением
Result<int> successWithValue = Result.Success(42);
```

### Создание результата с ошибкой

```csharp
// Создание результата с ошибкой
Result failure = Result.Failure(new Error("Произошла ошибка"));

// Создание результата с ошибкой (краткая форма)
Result failure = Result.Failure("Произошла ошибка");

// Создание результата с ошибкой для типизированного результата
Result<int> failureWithValue = Result.Failure<int>(new Error("Ошибка операции"));
```

### Работа с типизированным результатом

```csharp
// Получение значения (выбрасывает исключение, если результат неуспешен)
int value = result.Value;

// Получение значения или значения по умолчанию
int valueOrDefault = result.ValueOrDefault;

// Проверка успешности операции
if (result.IsSuccess)
{
    // Обработка успешного результата
}
else
{
    // Обработка ошибки
    Error error = result.Error;
}
```

## Интеграция с ASP.NET Core

### Преобразование Result в ActionResult

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        Result<User> result = await _userService.GetUserAsync(id);
        return result.ToActionResult();
    }
}
```

### Асинхронные операции

```csharp
// Асинхронное преобразование Result в ActionResult
public async Task<ActionResult<User>> GetUserAsync(int id)
{
    var resultTask = _userService.GetUserAsync(id);
    return await resultTask.ToActionResult();
}
```

## Лучшие практики

1. **Используйте Result для всех операций, которые могут завершиться с ошибкой**
   ```csharp
   public Result<User> CreateUser(UserDto userDto)
   {
       if (string.IsNullOrEmpty(userDto.Email))
           return Result.Failure<User>(new Error("Email не может быть пустым"));
           
       // Логика создания пользователя
       return Result.Success(newUser);
   }
   ```

2. **Цепочка операций**
   ```csharp
   public Result<Order> ProcessOrder(OrderDto orderDto)
   {
       return Result.Create(orderDto);
   }
   ```

3. **Обработка ошибок**
   ```csharp
   public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
   {
       var result = await _orderService.CreateOrderAsync(orderDto);
       
       if (result.IsFailure)
       {
           _logger.LogError(result.Error);
           return result.ToActionResult();
       }
       
       return result.ToActionResult();
   }
   ```

## Настройка профилей ActionResult

Вы можете настроить, как `Result` преобразуется в `ActionResult`, используя профили:

```csharp
public class CustomActionResultProfile : IActionResultProfile
{
    public ActionResult Transform(Result result)
    {
        if (result.IsSuccess)
            return new OkResult();
            
        return new BadRequestObjectResult(new { error = result.Error.Message });
    }
    
    public ActionResult<T> Transform<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);
            
        return new BadRequestObjectResult(new { error = result.Error.Message });
    }
}
```

## Работа с ошибками

Библиотека предоставляет мощный механизм обработки ошибок, основанный на классе `Error` и соответствующих исключениях.

### Класс Error

`Error` - это record-класс, который представляет собой универсальный объект ошибки. Он содержит следующую информацию:

- `Message` - текст ошибки
- `Title` - заголовок ошибки
- `Type` - тип ошибки (в формате CamelCase с суффиксом "Error")
- `HttpStatusCode` - HTTP-статус код
- `Timestamp` - время возникновения ошибки
- `Metadata` - дополнительные реквизиты ошибки
- `Details` - список причин ошибки
- `StackTraces` - стек вызовов

### Создание ошибки

```csharp
// Создание базовой ошибки
var error = new Error("Произошла ошибка при обработке запроса");

// Создание ошибки с типом и заголовком
var badRequestError = new Error("Неверный формат данных")
    .AsBadRequest("InvalidDataError", "Ошибка валидации данных");

// Создание ошибки с метаданными
var errorWithMetadata = new Error("Ошибка доступа")
    .AsForbidden()
    .WithMetadata("UserId", 123)
    .WithMetadata("Resource", "Documents");
```

### Типы ошибок

Библиотека поддерживает следующие типы ошибок:

1. **BadRequestError** (400)
   ```csharp
   error.AsBadRequest("ValidationError", "Ошибка валидации");
   ```

2. **UnauthorizedError** (401)
   ```csharp
   error.AsUnauthorized();
   ```

3. **ForbiddenError** (403)
   ```csharp
   error.AsForbidden();
   ```

4. **NotFoundError** (404)
   ```csharp
   error.AsNotFound();
   ```

5. **InternalServerError** (500)
   ```csharp
   error.AsInternalServer("DatabaseError", "Ошибка базы данных");
   ```

### Исключения

Каждому типу ошибки соответствует исключение:

- `BadRequestException`
- `UnauthorizedException`
- `ForbiddenException`
- `NotFoundException`
- `InternalServerException`

### Дополнительные возможности

1. **Добавление деталей**
   ```csharp
   error.WithDetails("Дополнительная информация об ошибке");
   ```

2. **Добавление исключения**
   ```csharp
   try
   {
       // Ваш код
   }
   catch (Exception ex)
   {
       error.CausedBy(ex);
   }
   ```

3. **Добавление метаданных**
   ```csharp
   error.WithMetadata("Key", "Value");
   // или
   error.WithMetadata(new Dictionary<string, object> 
   { 
       { "Key1", "Value1" },
       { "Key2", "Value2" }
   });
   ```

4. **Проверка метаданных**
   ```csharp
   if (error.HasMetadataKey("UserId"))
   {
       // Действия
   }
   ```

### Пример использования в контроллере

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                var error = new Error($"Пользователь с ID {id} не найден")
                    .AsNotFound();
                return NotFound(error);
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            var error = new Error("Ошибка при получении пользователя")
                .AsInternalServer("UserRetrievalError", "Ошибка получения данных пользователя")
                .CausedBy(ex);
            return StatusCode(500, error);
        }
    }
}
```

## Использование

### Пример использования базовых компонентов

```csharp
using CSharpExtensions.Lib.Base;
using CSharpExtensions.Lib.Extensions;

// Использование базовых классов
public class MyService : BaseService
{
    public async Task<Result> DoSomethingAsync()
    {
        try
        {
            // Ваш код
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
```

### Пример использования JSON конвертеров

```csharp
using CSharpExtensions.Lib.JsonConverters;

// Настройка JSON опций
var options = new JsonSerializerOptions
{
    Converters = { new CustomJsonConverter() }
};
```

## Лицензия

MIT