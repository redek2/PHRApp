using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using PHRApp.Data;
using PHRApp.Services.Interfaces;
using PHRApp.Services.Implementations;
using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;

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
            
            var entryService = _serviceProvider.GetRequiredService<IEntryService>();

            var dto = new CreateEntryDto
            {
                Title = "Test Entry",
                Description = "This is a test entry.",
                EventDate = DateTime.UtcNow.AddDays(-1),
                Status = EntryStatus.Completed,
                CategoryIds = new List<int>(),
                FilePaths = new List<string>()
            };

            try
            {
                var id = await entryService.CreateEntryAsync(dto);
                MessageBox.Show($"Entry created with ID: {id}");
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

            // UI
            services.AddTransient<MainWindow>();
        }
    }
}