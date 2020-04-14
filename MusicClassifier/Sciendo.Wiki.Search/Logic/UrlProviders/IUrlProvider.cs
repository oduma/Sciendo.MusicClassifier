using System;

namespace Sciendo.Wiki.Search.Logic.UrlProviders
{
    public interface IUrlProvider
    {
        Uri GetUri(string wikiLanguage, params object[] variables);


    }
}
