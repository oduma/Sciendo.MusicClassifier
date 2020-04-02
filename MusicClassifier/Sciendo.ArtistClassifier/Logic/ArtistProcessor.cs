using Sciendo.ArtistClassifier.Contracts;
using Sciendo.ArtistClassifier.Contracts.DataTypes;
using Sciendo.MusicClassifier.KnowledgeBaseProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public IEnumerable<Artist> GetArtists(string proposedArtist, bool isFeaturedArtist, bool isComposer)
        {
            #region Sanitation
            if (string.IsNullOrEmpty(proposedArtist))
                throw new ArgumentNullException(nameof(proposedArtist));
            #endregion

            var simpleLatinLowerCaseProposedArtists = GetSimpleLatinLowerCaseString(proposedArtist);

            if(simpleLatinLowerCaseProposedArtists!=knowledgeBase.Excludes.PlaceholderAlbumArtists)
            {
                simpleLatinLowerCaseProposedArtists = AssimilatePersonalTitles(simpleLatinLowerCaseProposedArtists);


                if (!isFeaturedArtist)
                    return GetArtistsFromString(simpleLatinLowerCaseProposedArtists, isComposer, false);
                else
                    return GetArtistsFromTitleString(simpleLatinLowerCaseProposedArtists.ReplaceAll(knowledgeBase.FeaturedRules.NonTitledInformationFromTitle, string.Empty));

            }
            return null;
        }


        private IEnumerable<Artist> GetArtistsFromTitleString(string simpleLatinLowerCaseProposedArtists)
        {
            List<Artist> artists = new List<Artist>();

            foreach (var possibleAreaForFeaturedArtistMarkers in knowledgeBase.FeaturedRules.PossibleAreasForFeaturedArtistsMarkers)
            {
                Regex r = new Regex(possibleAreaForFeaturedArtistMarkers, RegexOptions.IgnoreCase);
                foreach( Match match in r.Matches(simpleLatinLowerCaseProposedArtists))
                {
                    var proposedFeaturedArtists = match.Groups[1].Value;
                    if(ALikelyFeaturedArtist(proposedFeaturedArtists))
                    {
                        var featuredArtists = (proposedFeaturedArtists.Length == 0)
                        ? null
                        : GetArtistsFromString(proposedFeaturedArtists, false, true);
                        if (featuredArtists != null)
                            artists.AddRange(featuredArtists);
                    }
                }
            }
            return artists;
        }

        private bool ALikelyFeaturedArtist(string proposedFeaturedArtists)
        {
            if (string.IsNullOrEmpty(proposedFeaturedArtists.Trim()))
                return false;
            if (!Regex.IsMatch(proposedFeaturedArtists, knowledgeBase.FeaturedRules.FeaturedArtistSanityCheckRegEx))
                return false;
            return knowledgeBase.FeaturedRules.FeatureMustContainWords.AnyMatch(proposedFeaturedArtists);
        }

        private IEnumerable<Artist> GetArtistsFromString(string simpleLatinLowerCaseProposedArtists, bool isComposer, bool isFeatured)
        {
            List<Artist> artists = new List<Artist>();

            artists= ExtractKnownArtists(ref simpleLatinLowerCaseProposedArtists, isComposer, isFeatured);
            if (string.IsNullOrEmpty(simpleLatinLowerCaseProposedArtists))
                return artists;

            artists.AddRange(ExtractKnownBands(ref simpleLatinLowerCaseProposedArtists, isComposer, isFeatured));
            if (string.IsNullOrEmpty(simpleLatinLowerCaseProposedArtists))
                return artists;

            string[] firstPassSplitParts = TryComplexWordSeparators(simpleLatinLowerCaseProposedArtists);
            firstPassSplitParts = ApplyConditionalSplits(firstPassSplitParts);

            var otherBandsAndArtists= ExtractBandsAndArtists(firstPassSplitParts, isComposer, isFeatured);
            if (otherBandsAndArtists != null)
                artists.AddRange(otherBandsAndArtists);
            return artists;
        }

        private IEnumerable<Artist> ExtractBandsAndArtists(string[] firstPassSplitParts, bool isComposer, bool isFeatured)
        {
            var wordsSeparatorsGlobal = (isFeatured) ? knowledgeBase.FeaturedRules.WordsSeparatorsGlobal : knowledgeBase.Excludes.WordsSeparatorsGlobal;

            foreach (var firstPassSplitPart in firstPassSplitParts)
            {
                string[] wordParts = SplitOnWords(firstPassSplitPart);
                var decomposedArtistName = new List<string>();
                Artist artist;
                foreach (var wordPart in wordParts)
                {
                    if (!wordsSeparatorsGlobal.Contains(wordPart))
                    {
                        decomposedArtistName.Add(wordPart);
                    }
                    //else
                    //{
                    //    artist = ComposeArtistAndType(ref decomposedArtistName, isComposer, isFeatured);
                    //    if (artist != null)
                    //        yield return artist;
                    //}
                }

                artist = ComposeArtistAndType(ref decomposedArtistName, isComposer, isFeatured);
                if (artist != null)
                    yield return artist;
            }
        }

        private List<Artist> ExtractKnownBands(ref string simpleLatinLowerCaseProposedArtists, bool isComposer, bool isFeatured)
        {
            List<Artist> artists = new List<Artist>();
            foreach (var bandExcludedFromSplitting in knowledgeBase.Excludes.BandsForSplitting)
            {
                var bandExcludedFromSplittingLowerTrimmed = bandExcludedFromSplitting.ToLower().Trim();
                if (simpleLatinLowerCaseProposedArtists.Contains(bandExcludedFromSplittingLowerTrimmed))
                {
                    simpleLatinLowerCaseProposedArtists =
                        simpleLatinLowerCaseProposedArtists.Replace(bandExcludedFromSplittingLowerTrimmed,
                            string.Empty);
                    artists.Add(new Artist
                    {
                        Name = knowledgeBase.Transforms.ArtistNamesMutation.Keys.Contains(bandExcludedFromSplittingLowerTrimmed)
                            ? knowledgeBase.Transforms.ArtistNamesMutation[bandExcludedFromSplittingLowerTrimmed]
                            : bandExcludedFromSplittingLowerTrimmed,
                        Type = ArtistType.Band,
                        IsComposer=isComposer,
                        IsFeatured=isFeatured
                    });
                }
            }

            return artists;
        }

        private List<Artist> ExtractKnownArtists(ref string simpleLatinLowerCaseProposedArtists, bool isComposer, bool isFeatured)
        {
            List<Artist> artists = new List<Artist>();
            foreach (var artistExcludedFromSplitting in knowledgeBase.Excludes.ArtistsForSplitting)
            {
                var artistExcludedFromSplittingLowerTrimmed = artistExcludedFromSplitting.ToLower().Trim();
                if (simpleLatinLowerCaseProposedArtists.Contains(artistExcludedFromSplittingLowerTrimmed))
                {
                    simpleLatinLowerCaseProposedArtists =
                        simpleLatinLowerCaseProposedArtists.Replace(artistExcludedFromSplittingLowerTrimmed,
                            string.Empty);
                    artists.Add( new Artist
                    { 
                        Name = artistExcludedFromSplittingLowerTrimmed, 
                        Type = ArtistType.Artist,
                        IsComposer=isComposer,
                        IsFeatured=isFeatured 
                    });
                }
            }
            return artists;
        }

        private string[] TryComplexWordSeparators(string input)
        {
            List<string> result = new List<string>();
            StringBuilder accumulator = new StringBuilder("");
            int index = 0;
            foreach(char inputPart in input)
            {
                if (CharacterIsASplitter(inputPart, index++, input))
                {
                    if (accumulator.Length > 0)
                        result.Add(accumulator.ToString().Trim());
                    accumulator.Clear();
                }
                else
                {
                    accumulator.Append(inputPart);
                }
            }
            result.Add(accumulator.ToString().Trim());
            return result.ToArray();
        }

        private bool CharacterIsASplitter(char character, int position, string input)
        {
            if (!knowledgeBase.Spliters.ConditionalWordsSplitters.ContainsKey(character))
                return false;
            if (knowledgeBase.Spliters.ConditionalWordsSplitters[character] == null)
                return true;
            if (!knowledgeBase.Spliters.ConditionalWordsSplitters[character].AnyMatch(input.Substring(0, position))
                && !knowledgeBase.Spliters.ConditionalWordsSplitters[character].AnyMatch(input.Substring(position + 1, input.Length - position - 1)))
                return true;
            return false;
        }

        private string[] SplitOnWords(string firstPassSplitPart)
        {
            return firstPassSplitPart.Split(new[] { knowledgeBase.Spliters.WordsSimpleSplitter },
                StringSplitOptions.RemoveEmptyEntries);
        }

        //probably need a bit of a different rule for composers e.g. Avey Tare, Panda Bear, Deakin & Geologist
        //and possibly for featuring artists not sure though
        private string[] ApplyConditionalSplits(string[] firstPassSplitParts)
        {
            List<string> parts = new List<string>();
            foreach(var firstPassSplitPart in firstPassSplitParts)
            {
                parts.AddRange(ParseString(firstPassSplitPart.Trim()));
            }

            return parts.ToArray();
        }

        private string[] ParseString(string workString)
        {
            var workStringParts = SplitOnWords(workString);
            int index = 0;
            List<string> result = new List<string>();
            StringBuilder accumulator = new StringBuilder("");
            foreach (var workStringPart in workStringParts)
            {
                if (WordInPositionPreventsSplitting(workStringPart, index, workStringParts.Length-1))
                {
                    return new[] { workString };
                }
                if (WordIsNotaSplitterInContext(workStringPart, workStringParts, index++))
                {
                    accumulator.Append(workStringPart + knowledgeBase.Spliters.WordsSimpleSplitter);
                }
                else
                {
                    result.Add(accumulator.ToString().Trim());
                    accumulator.Clear();
                }
            }
            result.Add(accumulator.ToString().Trim());
            return result.ToArray();
        }

        private bool WordInPositionPreventsSplitting(string word, int position, int maxNoOfWords)
        {
            if (!knowledgeBase.Spliters.ConditionalSplitters.ContainsKey(word))
                return false;
            if (knowledgeBase.Spliters.ConditionalSplitters[word].ExceptionPositionDefinition == null)
                return false;
            switch(knowledgeBase.Spliters.ConditionalSplitters[word].ExceptionPositionDefinition.Position)
            {
                case Position.None:
                    return false;
                case Position.First:
                    if (position == 0)
                        return true;
                    else
                        return false;
                case Position.Last:
                    if (position == maxNoOfWords)
                        return true;
                    else
                        return false;
                case Position.Any:
                    return true;
                default:
                    return false;
            }
        }

        private bool WordIsNotaSplitterInContext(string word, string[] setOfWords, int position)
        {
            if(!knowledgeBase.Spliters.ConditionalSplitters.ContainsKey(word))
                return true;
            if (knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition == null)
                return false;
            switch(knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.LengthAppliesToSplitParts)
            {
                case Applicability.All:
                    if (position -1 > knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.WordsPerPart
                        && setOfWords.Length - position - 1 > knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.WordsPerPart)
                        return true;
                    else
                        if(knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.ExceptIfAnyPartsEqualRegex==null)
                            return false;
                        if (knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.ExceptIfAnyPartsEqualRegex.AnyMatch(setOfWords.JoinTo(position, knowledgeBase.Spliters.WordsSimpleSplitter))
                        || knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.ExceptIfAnyPartsEqualRegex.AnyMatch(setOfWords.JoinFrom(position, knowledgeBase.Spliters.WordsSimpleSplitter)))
                            return true;
                        else
                            return false;
                case Applicability.Any:
                    if (position -1 > knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.WordsPerPart
                        || setOfWords.Length - position -1 > knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.WordsPerPart)
                        return true;
                    else
                        if (knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.ExceptIfAnyPartsEqualRegex == null)
                            return false;
                        if (knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.ExceptIfAnyPartsEqualRegex.AnyMatch(setOfWords.JoinTo(position, knowledgeBase.Spliters.WordsSimpleSplitter))
                        || knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.ExceptIfAnyPartsEqualRegex.AnyMatch(setOfWords.JoinFrom(position, knowledgeBase.Spliters.WordsSimpleSplitter)))
                            return true;
                        else
                            return false;
                default:
                    return false;
            }
        }
        private string AssimilatePersonalTitles(string simpleLatinLowerCaseProposedArtists)
        {
            var result = string.Empty;
            foreach(var key in knowledgeBase.Transforms.PersonalTitlesAssimilations.Keys)
            {
                result = simpleLatinLowerCaseProposedArtists
                    .Replace(key, knowledgeBase.Transforms.PersonalTitlesAssimilations[key]);
            }
            return result;
        }

        private Artist ComposeArtistAndType(ref List<string> decomposedArtistName, bool isComposer, bool isFeatured)
        {
            if (decomposedArtistName.Count > 0)
            {

                var artist = new Artist
                {
                    Name = ComposeArtistName(decomposedArtistName),
                    Type = DetermineArtistType(decomposedArtistName),
                    IsComposer=isComposer,
                    IsFeatured=isFeatured
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
            if (!char.IsLetter(decomposedArtistName[0][0]) 
                || !char.IsLetter(decomposedArtistName.Last()[0]))
                return ArtistType.Band;
            
            if (decomposedArtistName.Any(w => knowledgeBase.Rules.ArtistWords.Contains(w)))
                return ArtistType.Artist;

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
