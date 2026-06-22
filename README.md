# PHRApp — Personal Health Record Application

## Opis projektu

PHRApp (Personal Health Record App) to desktopowa aplikacja Windows umożliwiająca prowadzenie osobistego dziennika zdarzeń zdrowotnych. Pozwala na rejestrowanie wizyt lekarskich, badań, przyjmowanych leków i innych zdarzeń medycznych wraz z możliwością kategoryzowania wpisów, określania ich statusu oraz dołączania plików (dokumentów PDF i zdjęć).

## Technologie

| Technologia | Wersja |
|---|---|
| .NET | 8.0 |
| WPF (Windows Presentation Foundation) | net8.0-windows |
| Entity Framework Core | 8.0.26 |
| SQLite (via EF Core) | 8.0.26 |
| xUnit | 2.5.3 |
| Moq | 4.20.72 |

Wzorzec architektoniczny: **MVVM** (Model-View-ViewModel) z warstwą serwisów i dependency injection opartym na `Microsoft.Extensions.DependencyInjection`.

## Funkcjonalności

- **Lista wpisów** — przeglądanie wszystkich aktywnych wpisów zdrowotnych w tabeli z sortowaniem po dacie zdarzenia.
- **Wyszukiwanie i filtrowanie** — filtrowanie po frazie tekstowej (tytuł, opis, nazwa kategorii), kategorii, statusie oraz zakresie dat (od/do).
- **Dodawanie wpisów** — formularz z polami: tytuł, opis, data i godzina zdarzenia, status, kategorie, załączniki.
- **Widok szczegółów wpisu** — podgląd pełnych danych wpisu wraz z listą załączników otwieranych przyciskiem lub podwójnym kliknięciem.
- **Edycja wpisów** — pełna edycja wszystkich pól wpisu, w tym dodawanie i usuwanie załączników.
- **Usuwanie wpisów** — archiwizacja logiczna (`IsArchived = true`) z potwierdzeniem użytkownika. Wpis i załączniki pozostają w bazie i na dysku.
- **Statusy wpisów** — trzy stany: Zaplanowany, Zakończony, Anulowany. Walidacja daty i godziny względem statusu na poziomie ViewModel i serwisu. Wpisy zaplanowane z terminem w przeszłości są automatycznie przepinane na Zakończony przy każdym odświeżeniu listy.
- **Godzina zdarzenia** — wybór godziny z dokładnością do 15 minut przez własny komponent `TimePickerControl`.
- **Kategorie** — przypisywanie wpisów do wielu kategorii (relacja many-to-many) z możliwością tworzenia nowych kategorii bezpośrednio w formularzu wpisu.
- **Załączniki** — obsługa plików PDF, JPG, JPEG i PNG. Pliki kopiowane pod unikalną nazwą (GUID) do lokalnego folderu aplikacji, otwierane domyślną aplikacją systemową.
- **Testy jednostkowe** — 25 testów jednostkowych pokrywających `EntryService` i `CategoryService` (walidacja, filtrowanie, CRUD, archiwizacja, automatyczna aktualizacja statusów). Testy uruchamiane na bazie SQLite in-memory z izolacją dla każdego przypadku testowego.
- **Lokalna baza danych SQLite** — dane przechowywane lokalnie w `%LocalAppData%\PHRApp\phrapp.db`, migracje stosowane automatycznie przy starcie.
- **Dependency Injection** — pełna konfiguracja DI z `Microsoft.Extensions.DependencyInjection`.


## Wymagania systemowe

- System operacyjny: **Windows 10 lub nowszy**
- .NET 8.0 Runtime (Windows Desktop)
- Brak wymagań co do zewnętrznych baz danych — aplikacja korzysta z wbudowanej bazy SQLite


## Uruchomienie projektu

### Wymagania deweloperskie

- Visual Studio 2022 z obsługą .NET 8 i WPF
- .NET 8.0 SDK
- EF Core CLI Tools (`dotnet-ef`) — opcjonalnie, do zarządzania migracjami

### Kroki

1. Sklonuj lub pobierz repozytorium.
2. Otwórz plik `PHRApp.sln` w Visual Studio.
3. Przywróć pakiety NuGet (`dotnet restore`).
4. Uruchom projekt (`F5`).

Baza danych SQLite zostanie utworzona automatycznie przy pierwszym uruchomieniu w lokalizacji: `%LocalAppData%\PHRApp\phrapp.db`

Pliki załączników przechowywane są w: `%LocalAppData%\PHRApp\attachments\`

### Migracje bazy danych

Migracje są stosowane automatycznie przy starcie aplikacji. Aby ręcznie dodać nową migrację podczas developmentu:

```bash
dotnet ef migrations add NazwaMigracji --project PHRApp
dotnet ef database update --project PHRApp
```

### Uruchamianie testów

```bash
dotnet test
```

## Model danych

```
Entry (Wpis)
├── Id, Title, Description
├── EventDate
├── Status
├── IsArchived
├── CreatedAt, UpdatedAt
├── EntryCategories    # many-to-many → Category
└── EntryAttachments   # many-to-many → Attachment

Category (Kategoria)
├── Id
├── Name
└── Description

Attachment (Załącznik)
├── Id
├── FileName
├── StoredFileName
├── FilePath
├── ContentType
├── FileSize
└── CreatedAt
```