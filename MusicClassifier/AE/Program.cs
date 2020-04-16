using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.ArtistClassifier.NLP.NER.Contracts;
using Sciendo.ArtistClassifier.NLP.NER.Logic;
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
using System.Collections.Generic;
using System.IO;

namespace AE
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = ConfigureLog(serviceCollection);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var bandEnhancerConfig = ReadConfiguration(logger, args);
            serviceProvider = ConfigureServices(serviceCollection, bandEnhancerConfig, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bandEnhancerConfig.KnowledgeBaseFile));

            var bandEnhancer = serviceProvider.GetService<IBandEnhancer>();
            Console.WriteLine("Band Name:");
            var proposedName = Console.ReadLine();
            while (proposedName != null)
            {
                var result = bandEnhancer.FindBandInWikipedia(proposedName);
                if (result != null)
                {
                    Console.WriteLine("Found: {0} - {1} in {2}, with following members:", result.Name, result.PageId, result.Language);
                    if (result.Members != null && result.Members.Count>0)
                        foreach (var member in result.Members)
                        {
                            Console.WriteLine(member);
                        }
                    else
                        Console.WriteLine("No Members found.");
                }
                proposedName = Console.ReadLine();
            }

        }

        public delegate IUrlProvider UrlProviderResolver(string key);

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

        private static ServiceProvider ConfigureServices(ServiceCollection serviceCollection, BandEnhancerConfiguration bandEnhancerConfig, string knowledgeBaseFileName)
        {
            serviceCollection.AddTransient<SearchUrlProvider>(r => new SearchUrlProvider(bandEnhancerConfig.WikiSearchConfig));
            serviceCollection.AddTransient<PageUrlProvider>(r => new PageUrlProvider(bandEnhancerConfig.WikiSearchConfig));
            serviceCollection.AddTransient<UrlProviderResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "SEARCH":
                        return serviceProvider.GetService<SearchUrlProvider>();
                    case "PAGE":
                        return serviceProvider.GetService<PageUrlProvider>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
            serviceCollection.AddTransient<IWebGet<string>>(r => new WebStringGet(r.GetRequiredService<ILogger<WebStringGet>>()));
            serviceCollection.AddTransient<IWiki>(r => new Wiki(r.GetService<UrlProviderResolver>()("SEARCH"), r.GetService<UrlProviderResolver>()("PAGE"), r.GetRequiredService<IWebGet<string>>(), bandEnhancerConfig.WikiSearchConfig));
            serviceCollection.AddSingleton<IPersonsNameFinder>(r => new PersonsNameFinder(bandEnhancerConfig.NlpConfig));
            serviceCollection.AddTransient<IKnowledgeBaseFactory, KnowledgeBaseFactory>();
            serviceCollection.AddTransient<IBandEnhancer>(r => new BandEnhancer(r.GetRequiredService<ILogger<BandEnhancer>>(), r.GetRequiredService<IKnowledgeBaseFactory>(),knowledgeBaseFileName, r.GetRequiredService<IWiki>(), r.GetRequiredService<IPersonsNameFinder>()));
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
