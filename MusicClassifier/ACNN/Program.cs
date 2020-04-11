using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.ArtistClassifier.NLP.NER.Configuration;
using Sciendo.ArtistClassifier.NLP.NER.Contracts;
using Sciendo.ArtistClassifier.NLP.NER.Logic;
using Sciendo.Config;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACNN
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var artistClassifierConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection, artistClassifierConfig);
            var artistClassifier = serviceProvider.GetService<IArtistClassifier>();

            logger.LogInformation("Configuration logged Ok.");
            Console.WriteLine("Artist Name:");
            var proposedName = Console.ReadLine();
            while (proposedName != null)
            {
                var result = artistClassifier.Classify(proposedName);
                Console.WriteLine("Found: {1} - {0}", result.Name, result.ArtistType);
                proposedName = Console.ReadLine();
            }


        }

        private static ArtistClassifierConfig ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            try
            {
                return new ConfigurationManager<ArtistClassifierConfig>().GetConfiguration(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"{AppDomain.CurrentDomain.FriendlyName}.json")
                    .Build());
            }
            catch (Exception e)
            {
                logger.LogError(e, "wrong config!");
                throw e;
            }
        }

        private static ServiceProvider ConfigureLog(ServiceCollection services)
        {
            return services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger())).BuildServiceProvider();
        }

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, ArtistClassifierConfig artistClassifierConfig)
        {

            serviceCollection.AddSingleton<IArtistClassifier>(r => new ArtistClassifier(artistClassifierConfig.NlpConfig));
            return serviceCollection.BuildServiceProvider();
        }

    }
}
