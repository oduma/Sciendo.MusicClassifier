using Sciendo.Wiki.Search.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Wiki.Search.Configuration
{
    public class WikiSearchConfig
    {
        //[]{" ", "_"},{"&","%26"},{"'","%27"},{"+","%2B"}
        public Dictionary<string, string> WikiSearchUrlParametersCleanUp { get; set; }

        //English - {"en","(band)"}
        //French - {"fr", "(groupe)"}
        //German - {"de",""}
        //Portuguese - {"pt",""}
        //Spanish - {"es","(banda)"}
        //Swedish - {"sv",""}
        public LanguageHelper[] WikiSearchLanguageHelpers { get; set; }

        //"https://{0}.wikipedia.org/w/api.php?action=query&list=search&srsearch={1}&format=json
        public string WikiSearchTemplateUrl { get; set; }

        //"https://{0}.wikipedia.org/w/api.php?action=parse&pageid={1}&format=json&prop=wikitext"
        public string WikiPageGetTemplateUrl { get; set; }

        public int MaxNoOfResultsConsidered { get; set; }
    }
}
