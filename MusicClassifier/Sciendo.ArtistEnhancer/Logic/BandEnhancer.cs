using Microsoft.Extensions.Logging;
using Sciendo.ArtistClassifier.NLP.NER.Contracts;
using Sciendo.ArtistEnhancer.Contracts;
using Sciendo.ArtistEnhancer.Contracts.DataTypes;
using Sciendo.ArtistEnhancer.KnowledgeBaseProvider;
using Sciendo.Wiki.Search.Contracts;
using Sciendo.Wiki.Search.Contracts.DataTypes;
using Sciendo.Wiki.Search.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sciendo.ArtistEnhancer.Logic
{
    public class BandEnhancer : IBandEnhancer
    {
        private readonly ILogger<BandEnhancer> logger;
        private readonly KnowledgeBase knowledgeBase;
        private readonly IWiki wikiSearch;
        private readonly IPersonsNameFinder personsNameFinder;
        private const string placeholderMarker = ", ";

        public BandEnhancer(ILogger<BandEnhancer> logger, IKnowledgeBaseFactory knowledgeBaseFactory, string knowledgeBaseFileName, IWiki wikiSearch, IPersonsNameFinder personsNameFinder)
        {
            this.logger = logger;
            this.knowledgeBase = knowledgeBaseFactory.GetKnowledgeBase(knowledgeBaseFileName);
            this.wikiSearch = wikiSearch;
            this.personsNameFinder = personsNameFinder;
        }
        public BandWikiPageInfo FindBandInWikipedia(string bandName)
        {
            if (string.IsNullOrEmpty(bandName))
                throw new ArgumentNullException(nameof(bandName));
            var searchResult = wikiSearch.Search(bandName);
            if (searchResult == null)
                return new BandWikiPageInfo { Name = bandName };
            return GetBandWikiPageInfo(searchResult, bandName);


        }

        private BandWikiPageInfo GetBandWikiPageInfo(SearchResult searchResult, string bandName)
        {
            if (searchResult!=null)
            {
                logger.LogInformation("Band found {0} in WikiPedia with pageId {1} in language {2}",
                    bandName, searchResult.PageId, searchResult.Language);
                var bandWikipageInfo = new BandWikiPageInfo 
                { 
                    Language = searchResult.Language, 
                    PageId = searchResult.PageId, 
                    Name = bandName,
                    Members=GetMembersForBand(searchResult)
                };
                return bandWikipageInfo;
            }
            return new BandWikiPageInfo { Name = bandName };
        }

        private List<string> GetMembersForBand(SearchResult searchResult)
        {
            var parsedPage = wikiSearch.GetPage(searchResult.PageId, searchResult.Language);
            if (parsedPage == null)
                return null;
            var membersAreas = GetMembersAreasFormPage(parsedPage.WikiText.AllText, searchResult.Language);
            if (membersAreas == null || !membersAreas.Any())
                return null;
            List<string> members = new List<string>();
            foreach(var membersArea in membersAreas)
            {
                var membersAreaWithoutNoise = ExcludeNoise(membersArea);
                var membersInArea = personsNameFinder.FindPersonNames(membersAreaWithoutNoise);
                if (membersInArea != null && membersInArea.Any())
                {
                    var sureMembers = membersInArea.ToList();
                    members.AddRange(sureMembers);
                }
            }
            return members.Distinct().ToList();
        }

        private string ExcludeNoise(string inputString)
        {
            string result = inputString;
            foreach(var noisePattern in knowledgeBase.Noise)
            {
                result = Regex.Replace(result, noisePattern, placeholderMarker, RegexOptions.IgnoreCase);
            }
            return result;
        }

        private IEnumerable<string> GetMembersAreasFormPage(string text, string language)
        {
            if (knowledgeBase.MembersMarkersByLanguages.ContainsKey(language))
                foreach(var marker in knowledgeBase.MembersMarkersByLanguages[language])
                {
                    var possibleMatches = Regex.Matches(text, marker);
                    if(possibleMatches.Count>0)
                        foreach(var possibleMatch in possibleMatches)
                        {
                            yield return possibleMatch.ToString();
                        }

                }
        }
    }
}
