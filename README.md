# 🎓 Student Assistant - Telegram Bot & Web API System

`StudentAssistant` is an enterprise-grade .NET multi-project solution featuring a **Telegram Bot**, a **REST Web API**, EF Core SQLite persistence with rich CEFR (A1-C2) seed questions, non-repeating question generation, real-time background question timer worker (60 seconds per question), user rating leaderboards, and progress tracking.

---

## 🏗️ Architecture Overview

The solution consists of 6 decoupled projects following Clean Architecture principles:

- **`StudentAssistant.Domain`**: Core domain entities (`User`, `Subject`, `Question`, `AnswerOption`, `TestAttempt`, `StudentAnswer`, `QuestionHistory`, `UserProgress`, `UserRating`), Enums (`CefrLevel`, `DifficultyLevel`, `QuestionType`, `TestStatus`, `UserRole`), and base `Auditable` entity.
- **`StudentAssistant.Data`**: Entity Framework Core DbContext (`AppDbContext`), Fluent API entity configurations, generic `Repository<T>` pattern, and automatic EF Core SQLite database creation with rich seed data.
- **`StudentAssistant.Service`**: Business logic layer containing DTOs, interfaces (`IUserService`, `ISubjectService`, `IQuestionService`, `ITestService`, `ITestAttemptService`, `IProgressService`, `IRatingService`, `IQuestionGeneratorService`, `IStatisticsService`), dynamic random non-repeating question selection, level progression, and rating calculation algorithms.
- **`StudentAssistant.Bot`**: Telegram Bot engine powered by `Telegram.Bot` SDK and `IHostedService` background worker. Features interactive keyboards, full state machine (`UserState`), inline query option buttons, background `QuestionTimerWorker` (60s countdown), leaderboards, user results history, and support contacts.
- **`StudentAssistant.WebApi`**: ASP.NET Core Web API with RESTful controllers (`UsersController`, `SubjectsController`, `QuestionsController`, `TestsController`, `ResultsController`, `ProgressController`, `RatingController`), global OpenAPI documentation, CORS, and custom `ExceptionMiddleware`.
- **`StudentAssistant.Tests`**: xUnit unit tests verifying service logic, EF Core in-memory database interactions, score calculations, CEFR progress, ratings, and bot session management.

---

## 🤖 Telegram Bot Workflow

```text
/start
   ↓
🎓 STUDENT ASSISTANT
   │
   ├── 🎯 Test boshlash
   │      ↓
   │   A1 / A2 / B1 / B2 / C1 / C2
   │      ↓
   │   Easy / Middle / Hard
   │      ↓
   │   5 / 10 / 20 ta savol
   │      ↓
   │   ⏱️ 60 sekundlik timer (Har bir savol uchun)
   │      ↓
   │   Random + takrorlanmaydigan savollar (User history tracking)
   │      ↓
   │   📊 Natija (To'g'ri/Noto'g'ri soni, %, ketgan vaqt, reyting ballari, tahlil)
   │
   ├── 📊 Natijalarim
   │
   ├── 🏆 Reyting (Leaderboard TOP-10 va foydalanuvchi o'rni)
   │
   ├── ℹ️ Bot haqida
   │
   └── 🆘 Support
```

---

## 🛠️ How to Run

### 1. Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### 2. Configure Telegram Bot Token
Open `StudentAssistant.Bot/appsettings.json` and replace `"YOUR_TELEGRAM_BOT_TOKEN_HERE"` with your actual Telegram bot token obtained from [@BotFather](https://t.me/BotFather):

```json
{
  "BotSettings": {
    "Token": "123456789:ABCdefGHIjklMNOpqrsTUVwxyz",
    "QuestionTimeoutSeconds": 60
  }
}
```

### 3. Run Telegram Bot Project
```bash
dotnet run --project StudentAssistant.Bot
```

### 4. Run Web API Project
```bash
dotnet run --project StudentAssistant.WebApi
```
Swagger OpenAPI UI will be accessible in development mode.

### 5. Run Unit Tests
```bash
dotnet test StudentAssistant.slnx
```

---

## 📡 Web API Endpoints Summary

- **Users**: `GET /api/users/{id}`, `GET /api/users/telegram/{telegramId}`, `POST /api/users`
- **Subjects**: `GET /api/subjects`, `GET /api/subjects/{id}`
- **Questions**: `GET /api/questions/{id}`, `GET /api/questions/level/{level}/difficulty/{difficulty}`
- **Tests**: `POST /api/tests/start`, `POST /api/tests/submit/{attemptId}`
- **Results**: `GET /api/results/attempt/{attemptId}`, `GET /api/results/user/{userId}/history`
- **Progress**: `GET /api/progress/user/{userId}`
- **Rating**: `GET /api/rating/top`, `GET /api/rating/user/{userId}`
