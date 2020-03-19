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

            simpleLatinLowerCaseProposedArtists = AssimilatePersonalTitles(simpleLatinLowerCaseProposedArtists);

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
            firstPassSplitParts = ApplyConditionalSplits(firstPassSplitParts);

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

        //probably need a bit of a different rule for composers e.g. Avey Tare, Panda Bear, Deakin & Geologist
        //and possibly for featuring artists not sure though
        private string[] ApplyConditionalSplits(string[] firstPassSplitParts)
        {
            foreach(var firstPassSplitPart in firstPassSplitParts)
            {
                foreach(var key in knowledgeBase.Spliters.ConditionalSplitters.Keys)
                {
                    if(knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions!=null &&
                        knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.Length.HasValue)
                    {
                        var result = TryApplyingLengthConditionalSplitters(firstPassSplitPart, key);
                        if (result != null)
                            return result;
                    }
                    else if(knowledgeBase.Spliters.ConditionalSplitters[key].Position.HasValue)
                    {
                        var result = TryApplyingPositionConditionalSplitters(firstPassSplitPart, key);
                        if (result != null)
                            return result;
                    }
                }
            }
            return firstPassSplitParts;
        }

        private string[] TryApplyingPositionConditionalSplitters(string input, string conditionIdentifier)
        {
            var wordsInInput = input.Split(new[] { knowledgeBase.Spliters.WordsSimpleSplitter }, 
                StringSplitOptions.RemoveEmptyEntries);
            if (wordsInInput.Length < knowledgeBase.Spliters.ConditionalSplitters[conditionIdentifier].Position)
                return null;

            if (wordsInInput[knowledgeBase.Spliters.ConditionalSplitters[conditionIdentifier].Position.Value] == conditionIdentifier)
                return null;
            if(wordsInInput.Any(w=>w==conditionIdentifier))
                return wordsInInput.Where(w => w != conditionIdentifier).ToArray();
            return null;
        }

        private string[] TryApplyingLengthConditionalSplitters(string input, string conditionIdentifier)
        {
            if (input.Contains(conditionIdentifier))
            {
                var trySplitAgain = input.Split(new string[] { conditionIdentifier },
                    StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < trySplitAgain.Length; i++)
                    trySplitAgain[i] = trySplitAgain[i].Trim();
                if (trySplitAgain.Length == 2)
                {
                    if (SplitBecauseOfLength(conditionIdentifier, trySplitAgain))
                    {
                        if (knowledgeBase.Spliters.ConditionalSplitters[conditionIdentifier].NonSplittingContent == null)
                            return trySplitAgain;
                        if (!trySplitAgain.Any(s => knowledgeBase.Spliters.ConditionalSplitters[conditionIdentifier].NonSplittingContent.Any(c => s.Contains(c))))
                            return trySplitAgain;
                    }
                }
            }
            return null;
        }

        private bool SplitBecauseOfLength(string key, string[] trySplitAgain)
        {
            if(knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.AppliedTo==Applicability.Both)
                return trySplitAgain[0].Split(new char[] { knowledgeBase.Spliters.WordsSimpleSplitter }, StringSplitOptions.RemoveEmptyEntries).Length
                                            >= knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.Length
                       && trySplitAgain[1].Split(new char[] { knowledgeBase.Spliters.WordsSimpleSplitter }, StringSplitOptions.RemoveEmptyEntries).Length
                                            >= knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.Length;
            if (knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.AppliedTo == Applicability.Any)
                return trySplitAgain[0].Split(new char[] { knowledgeBase.Spliters.WordsSimpleSplitter }, StringSplitOptions.RemoveEmptyEntries).Length
                                            >= knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.Length
                       || trySplitAgain[1].Split(new char[] { knowledgeBase.Spliters.WordsSimpleSplitter }, StringSplitOptions.RemoveEmptyEntries).Length
                                            >= knowledgeBase.Spliters.ConditionalSplitters[key].LengthConditions.Length;
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
