namespace Sciendo.MusicClassifier.KnowledgeBaseProvider
{
    public class FeaturedRules
    {
        public string[] NonTitledInformationFromTitle { get; set; }

        public string[] PossibleAreasForFeaturedArtistsMarkers { get; set; }

        public string[] FeatureMustContainWords { get; set; }

        public string[] WordsSeparatorsGlobal { get; set; }

        public string FeaturedArtistSanityCheckRegEx { get; set; }
    }
}