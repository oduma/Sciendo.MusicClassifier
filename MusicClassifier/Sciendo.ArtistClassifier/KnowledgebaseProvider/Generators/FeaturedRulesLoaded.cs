namespace Sciendo.MusicClassifier.KnowledgeBaseProvider.Generators
{
    public class FeaturedRulesLoaded
    {
        public char[] FeatureMarkers = new[]
        {
            '(',
            ')'
        };

        public string[] NonTitledInformationFromTitle = new[]
        {
            @"\(.?\)",
            @"\(part \d*\)",
            @"\(\d*\)",
            "(live)",
            "(instrumental)",
            "(original mix)"
        };

        public string[] PossibleAreasForFeaturedArtistsMarkers = new[]
        {
            @"\((.*?)\)",
            @"\[(.*?)\]"
        };

        public string[] FeatureMustContainWords = new[]
        {
            "arr.",
            "edit",
            "feat",
            "featuring",
            "feat.",
            "ft.",
            "mix",
            "mixed",
            "orch.",
            "presents",
            "pres.",
            "prod.",
            "remix",
            "remixed",
            "vocals",
            "with"
        };

        public string[] WordsSeparatorsGlobal = new[]
        {
            "&",
            "and,",    
            "arr.",
            "by",
            "ch",
            "con",
            "demo",
            "edit",
            "extended",
            "feat",
            "featuring",
            "feat.",
            "ft.",
            "instrumental",
            "main",
            "mix",
            "mixed",
            "original",
            "orch.",
            "part",
            "presents",
            "pres.",
            "prod.",
            "radio",
            "remix",
            "remixed",
            "theme",
            "version",
            "vocal",
            "vocals",
            "vs",
            "vs.",
            "with",
            "x",
            "✖",
        };

        public string FeaturedArtistSanityCheckRegEx = "[a-z]{1}";
    }
}