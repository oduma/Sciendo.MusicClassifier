using Sciendo.Wiki.Search.Contracts.DataTypes;

namespace Sciendo.Wiki.Search.Contracts
{
    public interface IWiki
    {
        SearchResult Search(string text);

        ParsedPage GetPage(long pageId, string language);
    }
}