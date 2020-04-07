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
            firstPassSplitParts = CleanNoiseWordsFromStrings(firstPassSplitParts, isFeatured);

            firstPassSplitParts = ApplyConditionalSplits(firstPassSplitParts);

            var otherBandsAndArtists= ExtractBandsAndArtists(firstPassSplitParts, isComposer, isFeatured);
            if (otherBandsAndArtists != null)
                artists.AddRange(otherBandsAndArtists);
            return artists;
        }

        private string[] CleanNoiseWordsFromStrings(string[] firstPassSplitParts, bool isFeatured)
        {
            var wordsSeparatorsGlobal = (isFeatured) ? knowledgeBase.FeaturedRules.WordsSeparatorsGlobal : knowledgeBase.Excludes.WordsSeparatorsGlobal;
            List<string> cleanArtists = new List<string>();
            foreach (var firstPassSplitPart in firstPassSplitParts)
            {
                cleanArtists.AddRange(CleanNoiseWordsFromString(firstPassSplitPart, wordsSeparatorsGlobal, isFeatured));
            }
            return cleanArtists.ToArray();
        }

        private IEnumerable<string> CleanNoiseWordsFromString(string input, string[] wordsSeparatorsGlobal, bool isFeatured)
        {
            string[] wordParts = SplitOnWords(input);
            var decomposedArtistName = new List<string>();
            string cleanArtist = "";
            foreach (var wordPart in wordParts)
            {
                if (!wordsSeparatorsGlobal.Contains(wordPart))
                {
                    decomposedArtistName.Add(wordPart);
                }
                else
                {
                    if (decomposedArtistName.Count > 0)
                    {
                        cleanArtist = ComposeArtistName(decomposedArtistName);
                        decomposedArtistName = new List<string>();
                        if (!string.IsNullOrEmpty(cleanArtist))
                            yield return cleanArtist;
                    }
                }
            }

            cleanArtist = ComposeArtistName(decomposedArtistName);
            if (!string.IsNullOrEmpty(cleanArtist))
                yield return cleanArtist;
        }

        private IEnumerable<Artist> ExtractBandsAndArtists(string[] firstPassSplitParts, bool isComposer, bool isFeatured)
        {
            List<Artist> artists = new List<Artist>();

            foreach (var firstPassSplitPart in firstPassSplitParts)
            {
                artists.Add(ComposeArtistAndType(firstPassSplitPart, DetermineArtistType(SplitOnWords(firstPassSplitPart)), isComposer, isFeatured));
            }
            return artists;
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
            var americanNumber = GetAmericanNumber(input);
            var numberStartingIndex = input.IndexOf(americanNumber);
            int index = 0;
            for(int i=index;i<input.Length;i++)
            {
                if(i==numberStartingIndex && !string.IsNullOrEmpty(americanNumber))
                {
                    accumulator.Append(americanNumber);
                    i += americanNumber.Length-1;
                }
                else
                {
                    if (CharacterIsASplitter(input[i], i, input))
                    {
                        if (accumulator.Length > 0)
                            result.Add(accumulator.ToString().Trim());
                        accumulator.Clear();
                    }
                    else
                    {
                        accumulator.Append(input[i]);
                    }
                }
            }
            result.Add(accumulator.ToString().Trim());
            return result.ToArray();
        }

        private string GetAmericanNumber(string input)
        {
            var number = Regex.Match(input, knowledgeBase.Excludes.WellFormattedNumbers);
            if (number.Success)
                return number.Value;
            return string.Empty;
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
                    var tempResult = accumulator.ToString().Trim();
                    if (!string.IsNullOrEmpty(tempResult))
                    {
                        result.Add(tempResult);
                        accumulator.Clear();
                    }
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
                    if (knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.AvoidSplitForLength.HasValue
                        && setOfWords.Length == knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.AvoidSplitForLength
                        && position!=0
                        && position != setOfWords.Length-1)
                        return true;
                    if (position > knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.WordsPerPart
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
                    if (knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.AvoidSplitForLength.HasValue
                        && setOfWords.Length == knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.AvoidSplitForLength
                        && position != 0
                        && position != setOfWords.Length - 1)
                        return true;
                    if (position > knowledgeBase.Spliters.ConditionalSplitters[word].SplitPartsLengthCondition.WordsPerPart
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
            var result = simpleLatinLowerCaseProposedArtists;
            foreach(var key in knowledgeBase.Transforms.PersonalTitlesAssimilations.Keys)
            {
                result = result
                    .Replace(key, knowledgeBase.Transforms.PersonalTitlesAssimilations[key]);
            }
            return result;
        }

        private Artist ComposeArtistAndType(string artistName, ArtistType artistType,  bool isComposer, bool isFeatured)
        {
            var artist = new Artist
            {
                Name = artistName,
                Type = artistType,
                IsComposer=isComposer,
                IsFeatured=isFeatured
            };
            return artist;
        }

        private string ComposeArtistName(List<string> decomposedArtistName)
        {
            var recomposedArtistName =
                string.Join(knowledgeBase.Spliters.WordsSimpleSplitter.ToString(), decomposedArtistName);

            return knowledgeBase.Transforms.ArtistNamesMutation.Keys.Contains(recomposedArtistName)
                ? knowledgeBase.Transforms.ArtistNamesMutation[recomposedArtistName]
                : recomposedArtistName.Trim();
        }

        private ArtistType DetermineArtistType(string[] decomposedArtistName)
        {

            //most of the people names start and end with a letter
            if (!char.IsLetter(decomposedArtistName[0][0]) 
                || !char.IsLetter(decomposedArtistName.Last()[0]))
                return ArtistType.Band;
            
            if (decomposedArtistName.Any(w => knowledgeBase.Rules.ArtistWords.Contains(w)))
                return ArtistType.Artist;

            if (decomposedArtistName.Length >= knowledgeBase.Rules.MaxWordsPerArtist)
                return ArtistType.Band;
            
            if (knowledgeBase.Rules.BandStartWords.Any(w => w == decomposedArtistName[0]))
                return ArtistType.Band;

            if (decomposedArtistName.Any(w => knowledgeBase.Rules.BandWords.Contains(w)))
                return ArtistType.Band;
            return ArtistType.Artist;
        }

    }
}
