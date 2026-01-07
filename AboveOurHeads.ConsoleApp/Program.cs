using AboveOurHeads.Core.Common;
using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Core.Models;
using AboveOurHeads.Services;
using AboveOurHeads.Services.Tles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AboveOurHeads.ConsoleApp
{
    internal class Program
    {
        private static TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
        private static int _timerUpdate = 2;

        static async Task Main(string[] args)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder
                .AddSimpleConsole()
                //.SetMinimumLevel(LogLevel.Information));
                .SetMinimumLevel(LogLevel.None));

            var satelliteLogger = factory.CreateLogger<SatelliteService>();

            var fileProviderLogger = factory.CreateLogger<FileTleProvider>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "25544.txt");
            ITleProvider tleProvider = new FileTleProvider(fileProviderLogger, filePath);

            //var httpProviderLogger = factory.CreateLogger<HttpTleProvider>();
            //var httpClient = new HttpClient();
            //ITleProvider tleProvider = new HttpTleProvider(httpProviderLogger, httpClient);

            var cachedProviderLogger = factory.CreateLogger<CachedTleProvider>();
            ITleProvider cachedTleProvider = new CachedTleProvider(
                cachedProviderLogger, 
                tleProvider,
                new MemoryCache(new MemoryCacheOptions()),
                _cacheExpiration);

            ISatelliteService service = new SatelliteService(satelliteLogger, cachedTleProvider);

            Console.WriteLine("Tracking de satellite en temps réel (Ctrl+C pour arrêter)");
            Console.WriteLine($"Actualisation toutes les {_timerUpdate} secondes\n");

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_timerUpdate));
            int callCount = 0;

            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    Console.Clear();
                    Console.WriteLine("Tracking de satellite en temps réel (Ctrl+C pour arrêter)\n");

                    callCount++;
                    Console.WriteLine($"┌─── Appel #{callCount} à {DateTime.Now:HH:mm:ss} ───\n");

                    var result = await service.GetSatellitePredictionAsync(25544); // 20580=HUBBLE(HST) / 25544=ISS
                    DisplayResult(result);

                    Console.WriteLine($"└───────────────────────────────────────────\n");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nTracking arrêté.");
            }
        }
        static void DisplayResult(ServiceResult<Satellite> result)
        {
            if (result.IsSuccess)
            {
                Console.WriteLine();
                Console.WriteLine($"│   Satellite    : {result.Value.Name}");
                Console.WriteLine($"│   NoradId      : {result.Value.NoradId}");
                Console.WriteLine($"│   Latitude     : {result.Value.Position.Latitude:F2}°");
                Console.WriteLine($"│   Longitude    : {result.Value.Position.Longitude:F2}°");
                Console.WriteLine($"│   Altitude     : {result.Value.Position.AltitudeKm:F2} km");
                Console.WriteLine($"│   Vitesse      : {result.Value.Velocity.Kmh:F2} kmh");
            }
            else
            {
                Console.WriteLine($"│   Erreur: {result.Error}");
            }
        }
    }
}
