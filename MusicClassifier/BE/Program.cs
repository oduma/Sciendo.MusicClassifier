using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.ArtistEnhancer.Contracts;
using Sciendo.ArtistEnhancer.KnowledgeBaseProvider;
using Sciendo.ArtistEnhancer.Logic;
using Sciendo.Config;
using Sciendo.Web;
using Sciendo.Wiki.Search.Contracts;
using Sciendo.Wiki.Search.Logic;
using Sciendo.Wiki.Search.Logic.UrlProviders;
using Serilog;
using System;
using System.IO;

namespace BE
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var bandEnhancerConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection, bandEnhancerConfig, new KnowledgeBase());

            var bandEnhancer = serviceProvider.GetService<IBandEnhancer>();
            Console.WriteLine("Band Name:");
            var proposedName = Console.ReadLine();
            while (proposedName != null)
            {
                var result = bandEnhancer.FindBandInWikipedia(proposedName);
                if (result != null)
                   Console.WriteLine("Found: {0} - {1} in {2}.", result.Name, result.PageId, result.Language);
                proposedName = Console.ReadLine();
            }

        }

        private static BandEnhancerConfiguration ReadConfiguration(ILogger<Program> logger, string[] args)
        {
            try
            {
                return new ConfigurationManager<BandEnhancerConfiguration>().GetConfiguration(new ConfigurationBuilder()
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

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, BandEnhancerConfiguration bandEnhancerConfig, KnowledgeBase knowledgeBase)
        {
            serviceCollection.AddTransient<IUrlProvider>(r => new UrlProvider(bandEnhancerConfig.WikiSearchConfig));
            serviceCollection.AddTransient<IWebGet<string>>(r => new WebStringGet(r.GetRequiredService<ILogger<WebStringGet>>()));
            serviceCollection.AddTransient<IWiki>(r => new Wiki(r.GetRequiredService<IUrlProvider>(), r.GetRequiredService<IWebGet<string>>(), bandEnhancerConfig.WikiSearchConfig));
            serviceCollection.AddTransient<IBandEnhancer>(r => new BandEnhancer(r.GetRequiredService<ILogger<BandEnhancer>>(), knowledgeBase, r.GetRequiredService<IWiki>()));
            return serviceCollection.BuildServiceProvider();
        }

        private static ServiceProvider ConfigureLog(ServiceCollection services)
        {
            return services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger())).BuildServiceProvider();
        }


    }
}
