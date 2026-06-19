# PHRApp - Personal Health Record Application

>**Status projektu: W trakcie realizacji**
> Aplikacja posiada już komplet podstawowych funkcjonalności (dodawanie, edycja, usuwanie, przeglądanie wpisów wraz z załącznikami i filtrowaniem), ale nie jest jeszcze gotowa do wdrożenia produkcyjnego. Trwają prace nad testami jednostkowymi oraz dopracowaniem interfejsu użytkownika.

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

- **Lista wpisów** - przeglądanie wszystkich aktywnych (niearchiwalnych) wpisów zdrowotnych w tabeli.
- **Wyszukiwanie i filtrowanie** - filtrowanie wpisów po frazie tekstowej (tytuł, opis, nazwa kategorii), kategorii, statusie oraz zakresie dat (od/do).
- **Dodawanie wpisów** - formularz tworzenia nowego wpisu z polami: tytuł, opis, data i godzina zdarzenia, status, kategorie, załączniki.
- **Widok szczegółów wpisu** - podgląd pełnych danych wpisu wraz z listą załączników, otwieranych przyciskiem lub podwójnym kliknięciem.
- **Edycja wpisów** - pełna edycja wszystkich pól wpisu, w tym dodawanie i usuwanie załączników (zarówno nowych jak i wcześniej zapisanych).
- **Usuwanie wpisów** - archiwizacja wpisu (`IsArchived = true`) z potwierdzeniem, dostępna z poziomu widoku szczegółów. Wpis i jego załączniki pozostają w bazie i na dysku - usunięcie jest wyłącznie logiczne (ukrycie z list i widoków).
- **Statusy wpisów** - trzy stany: `Planned` (zaplanowany), `Completed` (zakończony), `Cancelled` (anulowany) z walidacją daty i godziny względem statusu, zarówno w warstwie ViewModel jak i w serwisie. Wpisy `Planned` z terminem w przeszłości są automatycznie przepinane na `Completed` przy każdym odświeżeniu listy.
- **Godzina zdarzenia** - wybór godziny zdarzenia (z dokładnością do 15 minut) przez własny komponent `TimePickerControl`, niezależnie od daty.
- **Kategorie** - przypisywanie wpisów do wielu kategorii; możliwość tworzenia nowych kategorii bezpośrednio w formularzu wpisu.
- **Załączniki plików** - dołączanie plików PDF, JPG, JPEG i PNG do wpisów; pliki są kopiowane pod unikalną nazwą (GUID) do lokalnego folderu aplikacji. Otwieranie załączników domyślną aplikacją systemową.
- **Lokalna baza danych SQLite** - dane przechowywane lokalnie w `%LocalAppData%\PHRApp\phrapp.db`.
- **Dependency Injection** - pełna konfiguracja DI z `Microsoft.Extensions.DependencyInjection`.

### Planowane

- Testy jednostkowe (warstwa serwisów).
- Spójne, w pełni polskie etykiety w interfejsie oraz dopracowanie wizualne UI.
- Stronicowanie listy wpisów (przy większej liczbie rekordów).

---

## Wymagania systemowe

- System operacyjny: **Windows 10 lub nowszy**
- .NET 8.0 Runtime (Windows Desktop)
- Brak wymagań co do zewnętrznych baz danych - aplikacja korzysta z wbudowanej bazy SQLite

---

## Uruchomienie projektu

### Wymagania deweloperskie

- Visual Studio 2022 (lub nowszy) z obsługą .NET 8 i WPF
- .NET 8.0 SDK
- EF Core CLI Tools (`dotnet-ef`) - opcjonalnie, do zarządzania migracjami

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