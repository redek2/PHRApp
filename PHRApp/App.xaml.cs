using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PHRApp.Data;
using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Implementations;
using PHRApp.Services.Interfaces;
using System.IO;
using System.Windows;

namespace PHRApp
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            var categoryService = _serviceProvider.GetRequiredService<ICategoryService>();
            var entryService = _serviceProvider.GetRequiredService<IEntryService>();

            var dto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category."
            };

            try
            {
                // 1. Create category
                var categoryId = await categoryService.CreateCategoryAsync(new CreateCategoryDto
                {
                    Name = "Badania krwi3",
                    Description = "Kategoria dotycząca badań krwi3"
                });

                // 2. Create entry
                var entryId = await entryService.CreateEntryAsync(new CreateEntryDto
                {
                    Title = "Wizyta u lekarza3",
                    Description = "Rutynowe badanie krwi3",
                    EventDate = DateTime.UtcNow.AddDays(-1),
                    Status = EntryStatus.Completed,
                    CategoryIds = new List<int> { categoryId },
                    FilePaths = new List<string>()
                });

                MessageBox.Show($"Entry created with ID: {entryId} and linked to Category {categoryId}");

                // 3. Read entries
                var entries = await entryService.GetEntriesAsync(new EntryQueryDto
                {
                    CategoryId = categoryId,
                    Status = EntryStatus.Completed,
                    FromDate = DateTime.UtcNow.AddDays(-7),
                    ToDate = DateTime.UtcNow,
                    SearchTerm = "badanie"
                });

                var message = string.Join("\n\n", entries.Select(e =>
                $"[{e.Id}] {e.Title}\n" +
                $"Date: {e.EventDate}\n" +
                $"Status: {e.Status}\n" +
                $"Categories: {string.Join(", ", e.CategoryNames)}"
                ));

                MessageBox.Show(message);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHRApp",
                "phrapp.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"),
                ServiceLifetime.Transient);

            // Services
            services.AddTransient<IEntryService, EntryService>();
            services.AddTransient<IFileStorageService, FileStorageService>();
            services.AddTransient<ICategoryService, CategoryService>();

            // UI
            services.AddTransient<MainWindow>();
        }
    }
}