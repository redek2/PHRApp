# PHRApp — Personal Health Record Application

>**Status projektu: W trakcie realizacji**
> Aplikacja jest aktualnie w zaawansowanej fazie rozwoju i bliska ukończenia, jednak nie jest jeszcze gotowa do wdrożenia produkcyjnego. Część funkcjonalności może być niepełna lub nieprzetestowana.

---

## Opis projektu

PHRApp (Personal Health Record App) to desktopowa aplikacja Windows umożliwiająca użytkownikowi prowadzenie osobistego dziennika zdarzeń zdrowotnych. Pozwala na rejestrowanie wizyt lekarskich, badań, przyjmowanych leków i innych zdarzeń medycznych wraz z możliwością kategoryzowania wpisów, określania ich statusu oraz dołączania plików (dokumentów PDF i zdjęć).

Aplikacja powstała jako projekt na pracę inżynierską.

---

## Technologie

| Technologia | Wersja |
|---|---|
| .NET | 8.0 |
| WPF (Windows Presentation Foundation) | .NET 8.0-windows |
| Entity Framework Core | 8.0.26 |
| SQLite (via EF Core) | 8.0.26 |

Wzorzec architektoniczny: **MVVM** (Model-View-ViewModel) z warstwą serwisów i dependency injection opartym na `Microsoft.Extensions.DependencyInjection`.

---

## Funkcjonalności

### Zrealizowane

- **Lista wpisów** — przeglądanie wszystkich aktywnych (niearchiwalnych) wpisów zdrowotnych w tabeli.
- **Wyszukiwanie i filtrowanie** — filtrowanie wpisów po frazie tekstowej (tytuł, opis, nazwa kategorii), kategorii oraz statusie.
- **Dodawanie wpisów** — formularz tworzenia nowego wpisu z polami: tytuł, opis, data zdarzenia, status.
- **Statusy wpisów** — trzy stany: `Planned` (zaplanowany), `Completed` (zakończony), `Cancelled` (anulowany) z walidacją daty względem statusu.
- **Kategorie** — przypisywanie wpisów do wielu kategorii; możliwość tworzenia nowych kategorii bezpośrednio w formularzu wpisu.
- **Załączniki plików** — dołączanie plików PDF, JPG, JPEG i PNG do wpisów; pliki są kopiowane do lokalnego folderu aplikacji.
- **Lokalna baza danych SQLite** — dane przechowywane lokalnie w `%LocalAppData%\PHRApp\phrapp.db`.
- **Archiwizacja** — model danych przewiduje archiwizację wpisów (pole `IsArchived`).
- **Dependency Injection** — pełna konfiguracja DI z `Microsoft.Extensions.DependencyInjection`.

### W toku / Planowane

- Obsługa zdarzeń przycisków "Dodaj plik" i "Usuń plik" w oknie dodawania wpisu.
- Filtrowanie wpisów po zakresie dat.
- Widok szczegółów wpisu.
- Edycja i usuwanie wpisów.
- Archiwizacja wpisów z poziomu interfejsu.
- Stylizacja i dopracowanie interfejsu użytkownika.

---

## Wymagania systemowe

- System operacyjny: **Windows 10 lub nowszy**
- .NET 8.0 Runtime (Windows Desktop)
- Brak wymagań co do zewnętrznych baz danych — aplikacja korzysta z wbudowanej bazy SQLite

---

## Uruchomienie projektu

### Wymagania deweloperskie

- Visual Studio 2022 (lub nowszy) z obsługą .NET 8 i WPF
- .NET 8.0 SDK
- EF Core CLI Tools (`dotnet-ef`) — opcjonalnie, do zarządzania migracjami

### Kroki

1. Sklonuj lub pobierz repozytorium.
2. Otwórz plik `PHRApp.sln` w Visual Studio.
3. Przywróć pakiety NuGet.
4. Uruchom projekt.

Baza danych SQLite zostanie utworzona automatycznie przy pierwszym uruchomieniu w lokalizacji:

```
%LocalAppData%\PHRApp\phrapp.db
```

Pliki załączników są przechowywane w:

```
%LocalAppData%\PHRApp\attachments\
```

### Migracje bazy danych

Migracje są stosowane automatycznie przy starcie aplikacji. Aby ręcznie dodać nową migrację podczas developmentu:

```bash
dotnet ef migrations add NazwaMigracji --project PHRApp
dotnet ef database update --project PHRApp
```

---

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