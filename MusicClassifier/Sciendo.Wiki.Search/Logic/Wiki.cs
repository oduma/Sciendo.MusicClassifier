using Newtonsoft.Json;
using Sciendo.Web;
using Sciendo.Wiki.Search.Configuration;
using Sciendo.Wiki.Search.Contracts;
using Sciendo.Wiki.Search.Contracts.DataTypes;
using Sciendo.Wiki.Search.Logic.UrlProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Wiki.Search.Logic
{
    public class Wiki : IWiki
    {
        private readonly IUrlProvider searchUrlProvider;
        private readonly IUrlProvider pageUrlProvider;
        private readonly IWebGet<string> webGet;
        private readonly WikiSearchConfig wikiSearchConfig;

        public Wiki(IUrlProvider searchUrlProvider, IUrlProvider pageUrlProvider, IWebGet<string> webGet, WikiSearchConfig wikiSearchConfig)
        {
            this.searchUrlProvider = searchUrlProvider;
            this.pageUrlProvider = pageUrlProvider;
            this.webGet = webGet;
            this.wikiSearchConfig = wikiSearchConfig;
        }

        public ParsedPage GetPage(long pageId, string language)
        {
            string wikiResult = webGet.Get(pageUrlProvider.GetUri(language, pageId));

            if (!string.IsNullOrEmpty(wikiResult))
            {
                var pageResult = JsonConvert.DeserializeObject<WikiPageText>(wikiResult);
                return pageResult.ParsedPage;
            }
            return null;
        }

        public SearchResult Search(string text)
        {
            SearchResult result = SearchInAllLanguages(text);
            if (result == null)
                result = SearchInAllLanguages(text,true);
            return result;
        }

        private SearchResult SearchInAllLanguages(string text, bool usingHelpers = false)
        {

            foreach (var wikiSearchLanguageHelper in wikiSearchConfig.WikiSearchLanguageHelpers)
            {
                var searchForText = text;

                if (usingHelpers && !string.IsNullOrEmpty(wikiSearchLanguageHelper.Helper))
                {
                    searchForText = $"{text} {wikiSearchLanguageHelper.Helper}";
                }
                var searchResult = SearchInLanguage(wikiSearchLanguageHelper.Language, searchForText);
                if (searchResult!=null)
                {
                    return searchResult;
                }
            }
            return null;
        }
        private SearchResult SearchInLanguage(string key, string text)
        {
            var searchResult = JsonConvert.DeserializeObject<WikiSearchResults>(webGet.Get(searchUrlProvider.GetUri(key, text)));

            var noOfSearchResults = (searchResult.Query.SearchInfo.TotalHits > wikiSearchConfig.MaxNoOfResultsConsidered)
                ? wikiSearchConfig.MaxNoOfResultsConsidered
                : searchResult.Query.SearchInfo.TotalHits;
            for (int iteration = 0; iteration < noOfSearchResults; iteration++)
            {
                if (searchResult.Query.SearchResults[iteration].Title.ToLower() == text)
                {
                    var result = searchResult.Query.SearchResults[iteration];
                    result.Language = key;
                    return result;
                }
            }
            return null;
        }
    }
}
