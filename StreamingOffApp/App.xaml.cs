using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StreamingOffApp.Data;
using StreamingOffApp.Models;
using StreamingOffApp.Services;
using StreamingOffApp.ViewModels;
using System;
using System.Windows;
using System.Collections.Generic;

namespace StreamingOffApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            services.AddDbContext<StreamingContext>(options =>
                options.UseSqlServer("Server=PC_DAWID;Database=StreamingOffDB;Trusted_Connection=True;TrustServerCertificate=True;"));
            services.AddScoped<IStreamingService, StreamingService>();
            services.AddSingleton<MainViewModel>();
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                SeedDatabase(_serviceProvider); // Poprawka: przekazujemy _serviceProvider zamiast context

                var mainWindow = new MainWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
                };
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd uruchamiania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void SeedDatabase(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<StreamingContext>();
                context.Database.EnsureCreated(); // Tworzy bazę, jeśli nie istnieje

                // Sprawdź, czy tabela StreamingOffers jest pusta
                if (!context.StreamingOffers.Any())
                {
                    // Dodaj przykładowe oferty tylko, jeśli tabela jest pusta
                    var sampleOffers = new List<StreamingOffer>
                    {
                        new StreamingOffer { PlatformName = "Netflix", Price = 29.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Standard plan" },
                        new StreamingOffer { PlatformName = "Disney+", Price = 34.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Premium plan" },
                        new StreamingOffer { PlatformName = "HBO Max", Price = 24.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Basic plan" },
                        new StreamingOffer { PlatformName = "Amazon Prime", Price = 19.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Monthly subscription" },
                        new StreamingOffer { PlatformName = "Spotify", Price = 19.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Premium music" },
                        new StreamingOffer { PlatformName = "Apple TV+", Price = 24.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Streaming service" },
                        new StreamingOffer { PlatformName = "YouTube Premium", Price = 29.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Ad-free streaming" },
                        new StreamingOffer { PlatformName = "Crunchyroll", Price = 14.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Anime streaming" },
                        new StreamingOffer { PlatformName = "Twitch", Price = 9.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Turbo plan" },
                        new StreamingOffer { PlatformName = "Showtime", Price = 39.99m, PlanDays = 30, Status = OfferStatus.Active, Description = "Premium plan" }
                    };

                    context.StreamingOffers.AddRange(sampleOffers);
                    try
                    {
                        context.SaveChanges(); // Synchroniczne dla prostoty w inicjalizacji
                    }
                    catch (Exception ex)
                    {
                        var innerMessage = ex.InnerException?.Message ?? ex.Message;
                        MessageBox.Show($"Błąd seedowania bazy: {innerMessage}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw;
                    }
                }
            }
        }
    }
}