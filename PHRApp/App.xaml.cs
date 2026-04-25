using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PHRApp.Data;
using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Implementations;
using PHRApp.Services.Interfaces;
using PHRApp.ViewModels;
using System.IO;
using System.Windows;

namespace PHRApp
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

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
            services.AddTransient<EntryListViewModel>();

            // UI
            services.AddTransient<MainWindow>();
        }
    }
}