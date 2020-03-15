using Sciendo.ArtistClassifier.Contracts;
using Sciendo.ArtistClassifier.Contracts.DataTypes;
using Sciendo.MusicClassifier.KnowledgeBaseProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.ArtistClassifier.Logic
{
    public class ArtistProcessor : IArtistProcessor
    {
        private readonly KnowledgeBase knowledgeBase;

        public ArtistProcessor(IKnowledgeBaseFactory knowledgeBaseFactory, string knowledgeBasefile)
        {
            this.knowledgeBase = knowledgeBaseFactory.GetKnowledgeBase(knowledgeBasefile);
        }
        private string GetSimpleLatinLowerCaseString(string input)
        {
            var result = input.ToLower().Trim();
            foreach (var key in knowledgeBase.Transforms.LatinAlphabetTransformations.Keys)
            {
                result = result.Replace(key.ToLower(), knowledgeBase.Transforms.LatinAlphabetTransformations[key]);
            }

            return result;
        }

        public IEnumerable<Artist> GetArtists(string proposedArtist)
        {
            #region Sanitation
            if (string.IsNullOrEmpty(proposedArtist))
                throw new ArgumentNullException(nameof(proposedArtist));
            #endregion

            var simpleLatinLowerCaseProposedArtists = GetSimpleLatinLowerCaseString(proposedArtist);

            //return pre-canned artists
            foreach (var artistExcludedFromSplitting in knowledgeBase.Excludes.ArtistsForSplitting)
            {
                var artistExcludedFromSplittingLowerTrimmed = artistExcludedFromSplitting.ToLower().Trim();
                if (simpleLatinLowerCaseProposedArtists.Contains(artistExcludedFromSplittingLowerTrimmed))
                {
                    simpleLatinLowerCaseProposedArtists =
                        simpleLatinLowerCaseProposedArtists.Replace(artistExcludedFromSplittingLowerTrimmed,
                            string.Empty);
                    yield return new Artist
                    { Name = artistExcludedFromSplittingLowerTrimmed, Type = ArtistType.Artist };
                }
            }

            //return pre-canned bands
            foreach (var bandExcludedFromSplitting in knowledgeBase.Excludes.BandsForSplitting)
            {
                var bandExcludedFromSplittingLowerTrimmed = bandExcludedFromSplitting.ToLower().Trim();
                if (simpleLatinLowerCaseProposedArtists.Contains(bandExcludedFromSplittingLowerTrimmed))
                {
                    simpleLatinLowerCaseProposedArtists =
                        simpleLatinLowerCaseProposedArtists.Replace(bandExcludedFromSplittingLowerTrimmed,
                            string.Empty);
                    yield return new Artist
                    {
                        Name = knowledgeBase.Transforms.ArtistNamesMutation.Keys.Contains(bandExcludedFromSplittingLowerTrimmed)
                            ? knowledgeBase.Transforms.ArtistNamesMutation[bandExcludedFromSplittingLowerTrimmed]
                            : bandExcludedFromSplittingLowerTrimmed,
                        Type = ArtistType.Band
                    };
                }
            }

            var firstPassSplitParts = simpleLatinLowerCaseProposedArtists.Split(
                knowledgeBase.Excludes.CharactersSeparatorsForWords, StringSplitOptions.RemoveEmptyEntries);
            foreach (var firstPassSplitPart in firstPassSplitParts)
            {
                var wordParts = firstPassSplitPart.Split(new[] { knowledgeBase.Spliters.WordsSimpleSplitter },
                    StringSplitOptions.RemoveEmptyEntries);
                var decomposedArtistName = new List<string>();
                Artist artist;
                foreach (var wordPart in wordParts)
                {
                    if (!knowledgeBase.Excludes.WordsSeparatorsGlobal.Contains(wordPart))
                    {
                        decomposedArtistName.Add(wordPart);
                    }
                    else
                    {
                        artist = ComposeArtistAndType(ref decomposedArtistName);
                        if (artist != null)
                            yield return artist;
                    }
                }

                artist = ComposeArtistAndType(ref decomposedArtistName);
                if (artist != null)
                    yield return artist;
            }
        }

        private Artist ComposeArtistAndType(ref List<string> decomposedArtistName)
        {
            if (decomposedArtistName.Count > 0)
            {

                var artist = new Artist
                {
                    Name = ComposeArtistName(decomposedArtistName),
                    Type = DetermineArtistType(decomposedArtistName)
                };
                decomposedArtistName = new List<string>();
                return artist;
            }

            return null;
        }

        private string ComposeArtistName(List<string> decomposedArtistName)
        {
            var recomposedArtistName =
                string.Join(knowledgeBase.Spliters.WordsSimpleSplitter.ToString(), decomposedArtistName);

            return knowledgeBase.Transforms.ArtistNamesMutation.Keys.Contains(recomposedArtistName)
                ? knowledgeBase.Transforms.ArtistNamesMutation[recomposedArtistName]
                : recomposedArtistName.Trim();
        }

        private ArtistType DetermineArtistType(List<string> decomposedArtistName)
        {

            //most of the people names start and end with a letter
            if (!char.IsLetter(decomposedArtistName[0][0]) || char.IsDigit(decomposedArtistName.Last()[0]))
                return ArtistType.Band;

            if (decomposedArtistName.Count >= knowledgeBase.Rules.MaxWordsPerArtist)
                return ArtistType.Band;
            if (knowledgeBase.Rules.BandStartWords.Any(w => w == decomposedArtistName[0]))
                return ArtistType.Band;

            if (decomposedArtistName.Any(w => knowledgeBase.Rules.BandWords.Contains(w)))
                return ArtistType.Band;
            return ArtistType.Artist;
        }

    }
}
