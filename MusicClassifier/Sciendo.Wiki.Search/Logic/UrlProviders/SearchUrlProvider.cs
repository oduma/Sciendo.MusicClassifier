using Sciendo.Wiki.Search.Configuration;
using System;

namespace Sciendo.Wiki.Search.Logic.UrlProviders
{
    public class SearchUrlProvider : IUrlProvider
    {
        private readonly WikiSearchConfig wikiSearchConfig;

        public SearchUrlProvider(WikiSearchConfig wikiSearchConfig)
        {
            this.wikiSearchConfig = wikiSearchConfig;
        }

        public Uri GetUri(string wikiLanguage, params object[] variables)
        {
            var urlParam = variables[0].ToString();
            foreach (var key in wikiSearchConfig.WikiSearchUrlParametersCleanUp.Keys)
            {
                urlParam = urlParam.Replace(key, wikiSearchConfig.WikiSearchUrlParametersCleanUp[key]);
            }
            return new Uri(string.Format(wikiSearchConfig.WikiSearchTemplateUrl, wikiLanguage, urlParam));
        }

    }
}
