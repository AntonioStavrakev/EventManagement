# EventManagement

Уеб приложение за управление на събития с ASP.NET Core MVC.

## Какво включва

- начална `Home` страница
- регистрация и логин с реални потребители в локална база
- създаване, редакция и триене на събития
- собственикът може да edit/delete само своите event-и
- регистрация за чужди събития
- страници `My Events`, `My Schedule`, `Profile`
- управление на участници и CSV export

## Стартиране

1. Отвори проекта в:
   `/Users/antoniostavrakev/Documents/C#/EventManagement`
2. Изпълни:

```bash
dotnet restore
dotnet run --project EventManagement/EventManagement.csproj
```

3. Отвори адреса от конзолата, стандартно:
   `http://localhost:5083`

## Технически бележки

- приложението използва локален SQLite файл в `EventManagement/App_Data/eventmanagement.db`
- не изисква MySQL за демото
- при първо стартиране базата се създава автоматично

## Проверка

```bash
dotnet msbuild EventManagement.sln -nologo /t:Compile /m:1 /p:UseSharedCompilation=false
dotnet test EventManagement.Tests/EventManagement.Tests.csproj
```
