using Sciendo.Wiki.Search.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Wiki.Search.Logic.UrlProviders
{
    public class PageUrlProvider : IUrlProvider
    {
        private readonly WikiSearchConfig wikiSearchConfig;

        public PageUrlProvider(WikiSearchConfig wikiSearchConfig)
        {
            this.wikiSearchConfig = wikiSearchConfig;
        }
        public Uri GetUri(string wikiLanguage, params object[] variables)
        {
            var urlParam = variables[0].ToString();
            return new Uri(string.Format(wikiSearchConfig.WikiPageGetTemplateUrl, wikiLanguage, urlParam));
        }
    }
}
