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
            var possibleArtistsFeatures = Regex.Matches(simpleLatinLowerCaseProposedArtists,
                knowledgeBase.FeaturedRules.FeaturedArtistsInTheTitle);
            if (possibleArtistsFeatures.Count > 0)
            {
                List<Artist> artists = new List<Artist>();
                foreach (var possibleArtistName in possibleArtistsFeatures)
                {
                    var proposedFeaturedArtists = possibleArtistName.ToString()
                        .Split(knowledgeBase.FeaturedRules.FeatureMarkers, StringSplitOptions.RemoveEmptyEntries);
                    if (ALikelyFeaturedArtist(proposedFeaturedArtists))
                    {
                        var featuredArtists = (proposedFeaturedArtists.Length == 0)
                        ? null
                        : GetArtistsFromString(proposedFeaturedArtists[0], false, true);
                        if (featuredArtists != null)
                            artists.AddRange(featuredArtists);
                    }
                }
                return artists;
            }
            return null;
        }

        private bool ALikelyFeaturedArtist(string[] proposedFeaturedArtists)
        {
            if (proposedFeaturedArtists == null || proposedFeaturedArtists.Length != 1)
                return false;
            return Regex.IsMatch(proposedFeaturedArtists[0], knowledgeBase.FeaturedRules.FeaturedArtistSanityCheckRegEx);
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
            foreach(var key in knowledgeBase.Spliters.ConditionalWordsSplitters.Keys)
            {
                if(input.Contains(key))
                {
                    if (knowledgeBase.Spliters.ConditionalWordsSplitters[key] == null 
                        || StringShouldBeSplit(input, key,knowledgeBase.Spliters.ConditionalWordsSplitters[key].ToArray()))
                    {
                        result.AddRange(input.Split(new[] { key }, StringSplitOptions.RemoveEmptyEntries).Select(s=>s.Trim()));
                    }
                    else 
                    {
                        result = new List<string>();
                        continue;
                    }
                }
            }
            if (result.Any())
                return result.ToArray();
            return new[] { input };
        }

        private bool StringShouldBeSplit(string input, char wordsSplitter, params ExceptionDefinition[] exceptionDefintions)
        {
            var trySplittingAgain = input.Split(new[] { wordsSplitter }, StringSplitOptions.RemoveEmptyEntries);

            if (trySplittingAgain.Length==1)
                return false;
            if (!exceptionDefintions.Any(e=>e!=null))
                return false;
            foreach(var exceptionDefition in exceptionDefintions)
            {
                if (exceptionDefition.Position == Position.First
                    && WordPreventSplitting(trySplittingAgain[0], exceptionDefition.RegexTemplates))
                    return false;
                if (exceptionDefition.Position == Position.Last
                    && WordPreventSplitting(trySplittingAgain[trySplittingAgain.Length - 1], exceptionDefition.RegexTemplates))
                    return false;
                if (exceptionDefition.Position == Position.Any
                    && trySplittingAgain.Any(w => WordPreventSplitting(w, exceptionDefition.RegexTemplates)))
                    return false;
            }
            return true;
        }

        private bool WordPreventSplitting(string word, IEnumerable<string> regExTemplates)
        {
            foreach (var regExTemplate in regExTemplates)
                if (Regex.IsMatch(word,regExTemplate))
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
                var workString = firstPassSplitPart;

                var notAffectedByAnyKeys = true;
                foreach(var key in knowledgeBase.Spliters.ConditionalSplitters.Keys)
                {
                    if (workString.Contains(key))
                    {
                        if (knowledgeBase.Spliters.ConditionalSplitters[key].SplitPartsLengthCondition != null &&
                        knowledgeBase.Spliters.ConditionalSplitters[key].SplitPartsLengthCondition.WordsPerPart.HasValue
                        && StringShouldBeSplit(workString, key,
                            knowledgeBase.Spliters.ConditionalSplitters[key].SplitPartsLengthCondition))
                        {
                            var result = workString.Split(new[] { key }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(s => !string.IsNullOrEmpty(s)).Select(s=>s.Trim()).ToArray();
                            if (result != null)
                            {
                                parts.AddRange(ApplyConditionalSplits(result));
                                foreach(var partResult in result)
                                {
                                    workString = workString
                                        .Replace(partResult, string.Empty)
                                        .Replace(key,string.Empty)
                                        .Trim();
                                    
                                }
                            }
                        }
                        else if (knowledgeBase
                            .Spliters.ConditionalSplitters[key] != null &&
                            knowledgeBase
                            .Spliters.ConditionalSplitters[key].ExceptionPositionDefinition != null)
                        {

                            var result = TryApplyingPositionConditionalSplitters(workString, key, knowledgeBase
                            .Spliters.ConditionalSplitters[key].ExceptionPositionDefinition).Select(s=>s.Trim()).ToArray();
                            if (result != null)
                            {
                                parts.AddRange(ApplyConditionalSplits(result));
                                foreach(var partResult in result)
                                {
                                    workString = workString
                                        .Replace(partResult, string.Empty)
                                        .Replace(key, string.Empty)
                                        .Trim();
                                    ;
                                }
                            }
                        }
                        
                        else
                        {
                            parts.Add(workString);
                            workString = "";
                        }
                        notAffectedByAnyKeys = false;
                        if (string.IsNullOrEmpty(workString))
                            continue;
                    }
                }
                if (notAffectedByAnyKeys)
                    parts.Add(workString);
            }

            return parts.ToArray();
        }

        private string[] TryApplyingPositionConditionalSplitters(string input, string wordSplitter, 
            ExceptionDefinition exceptionPositionDefinition)
        {
            var wordsInInput = input.Split(new[] { knowledgeBase.Spliters.WordsSimpleSplitter },
                StringSplitOptions.RemoveEmptyEntries);
            if (wordsInInput.Length == 0)
                return null;
            switch(exceptionPositionDefinition.Position)
            {
                case Position.First:
                    return AnalyseWordAtPoistion(wordsInInput, 0, wordSplitter);
                case Position.Last:
                    return AnalyseWordAtPoistion(wordsInInput, wordsInInput.Length-1, wordSplitter);
                case Position.Any:
                    break;
                case Position.None:
                    break;
            }
            return null;
        }

        private string[] AnalyseWordAtPoistion(string[] wordsInInput, int position, string wordSplitter)
        {
            if (wordsInInput[position] == wordSplitter)
                return null;
            if (wordsInInput.Any(w => w == wordSplitter))
                return wordsInInput.Where(w => w != wordSplitter).ToArray();
            return null;
        }

        private bool StringShouldBeSplit(string input, 
            string conditionIdentifier, 
            SplitPartsLengthConditon splitPartsLengthConditon)
        {
            if (input.Contains(conditionIdentifier))
            {
                var trySplitAgain = input.Split(new string[] { conditionIdentifier },
                    StringSplitOptions.RemoveEmptyEntries).Select(p=>p.Trim()).ToArray();
                //take any whitespace in the string out
                var trySplitAgainAvoidEmpty = input.Split(new string[] { conditionIdentifier },
                    StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim())
                    .Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (trySplitAgainAvoidEmpty.Length > 1)
                {
                    if (SplitBecauseOfLength(conditionIdentifier, trySplitAgain, splitPartsLengthConditon))
                    {
                        if (splitPartsLengthConditon.ExceptIfAnyPartsEqualRegex==null || 
                            !splitPartsLengthConditon.ExceptIfAnyPartsEqualRegex.Any())
                            return true;
                        if (!trySplitAgain.Any(s => 
                            splitPartsLengthConditon.ExceptIfAnyPartsEqualRegex.Any(c => Regex.IsMatch(s,c))
                            ))
                            return true;
                    }
                }
                if (trySplitAgain.Length > trySplitAgainAvoidEmpty.Length)
                    return true;
                if (trySplitAgainAvoidEmpty.Length == 1 && input.Length != trySplitAgainAvoidEmpty[0].Length)
                    return true;
            }
            return false;
        }

        private bool SplitBecauseOfLength(string key, string[] trySplitAgain, SplitPartsLengthConditon splitPartsLengthConditon)
        {
            if (splitPartsLengthConditon.LengthAppliesToSplitParts == Applicability.All)
                return trySplitAgain.All(p => SplitOnWords(p).Length >= splitPartsLengthConditon.WordsPerPart);

            if (splitPartsLengthConditon.LengthAppliesToSplitParts == Applicability.Any)
                return trySplitAgain.Any(p => SplitOnWords(p).Length >= splitPartsLengthConditon.WordsPerPart);

            return true;
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
